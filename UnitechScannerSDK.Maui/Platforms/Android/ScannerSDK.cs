using Android.Content;
using Com.Unitech.Lib.Detect;
using Com.Unitech.Lib.Event;
using Com.Unitech.Lib.Event.Scanner;
using Com.Unitech.Lib.Model.Scanner;
using Com.Unitech.Lib.Reader;
using Com.Unitech.Lib.Types;
using System.Security.Cryptography;
using UnitechScannerSDK.Platforms.Android;
using UnitechBarcodeID = Com.Unitech.Lib.Types.Scanner.BarcodeID;
using UnitechBatteryState = Com.Unitech.Lib.Types.BatteryState;
using UnitechConnectState = Com.Unitech.Lib.Types.ConnectState;

namespace UnitechScannerSDK;

/// <summary>
/// Wraps the Unitech Scanner SDK binding with a clean, event-driven API
/// for use from a .NET MAUI application.
/// </summary>
public sealed partial class ScannerSDK(UnitechScannerSDKOptions options) : Java.Lang.Object,
    IDetectReaderEventListener,
    IReaderEventListener,
    IBarcodeEventListener,
    IParameterEventListener,
    IScannerSDK
{
    private readonly Context _context = Application.Context;
    private DetectReader? _detector;
    private BarcodeReader? _reader;

    // Coordinates ConnectAsync/DisconnectAsync with the asynchronous
    // ConnectStateEvent callback. Guarded by _stateGate.
    private readonly object _stateGate = new();

    private TaskCompletionSource<bool>? _stateWaiter;
    private ConnectState? _awaitedState;

    public UnitechScannerSDKOptions Options => options;

    // -------------------------------------------------------------------------
    // Discovery
    // -------------------------------------------------------------------------

    public void StartDiscovery(bool keepDetecting = false)
    {
        _detector ??= new DetectReader(_context);
        _detector.KeepDetecting = keepDetecting;
        _detector.ClearListener();
        _detector.AddListener(this);
        _detector.StartDetect();
    }

    public void StopDiscovery()
    {
        _detector?.StopDetect();
        _detector?.ClearListener();
        _detector?.Dispose();
        _detector = null;
    }

    // -------------------------------------------------------------------------
    // Connection
    // -------------------------------------------------------------------------

    /// <summary>
    /// Connects to <paramref name="device"/> and completes once the SDK reports
    /// <see cref="ConnectState.Connected"/> via <see cref="ConnectStateEvent"/>.
    /// </summary>
    /// <param name="timeout">
    /// Optional maximum time to wait for the connected state. When it elapses
    /// (or the native <c>connect()</c> call returns <c>false</c>) the method
    /// returns <c>false</c>. Pass <c>null</c> to wait indefinitely.
    /// </param>
    public async Task<bool> ConnectAsync(UnitechDevice device, TimeSpan? timeout = null)
    {
        await DisconnectAsync(timeout).ConfigureAwait(false);

        var reader = new BarcodeReader(device.ToNativeModel());
        reader.AddListener(this);
        _reader = reader;

        // Arm the waiter before kicking off the connection so a fast
        // ConnectStateEvent cannot fire before we are listening for it.
        Task<bool> reachedConnected = WaitForStateAsync(ConnectState.Connected!, timeout);

        // connect() may block until the handshake completes, so run it off the
        // calling thread to keep this method genuinely asynchronous.
        if (!await Task.Run(reader.Connect).ConfigureAwait(false))
        {
            // The native layer refused to even start connecting.
            CompleteStateWait(result: false);
            return false;
        }

        return await reachedConnected.ConfigureAwait(false);
    }

    /// <summary>
    /// Disconnects the active reader and completes once the SDK reports
    /// <see cref="ConnectState.Disconnected"/> (or <paramref name="timeout"/>
    /// elapses). Returns <c>true</c> if the disconnected state was observed.
    /// </summary>
    public async Task<bool> DisconnectAsync(TimeSpan? timeout = null)
    {
        var reader = _reader;
        if (reader is null)
            return true;

        Task<bool> reachedDisconnected = WaitForStateAsync(ConnectState.Disconnected!, timeout);

        await Task.Run(reader.Disconnect).ConfigureAwait(false);

        bool disconnected = await reachedDisconnected.ConfigureAwait(false);

        // Tear down only after the disconnected state has been observed (or the
        // wait timed out) so the final ConnectStateEvent still reaches us.
        reader.Destroy();
        if (ReferenceEquals(_reader, reader))
            _reader = null;

        return disconnected;
    }

    /// <summary>
    /// Arms <see cref="_stateWaiter"/> for <paramref name="target"/> and awaits
    /// it, returning <c>false</c> if the optional <paramref name="timeout"/>
    /// elapses first.
    /// </summary>
    private async Task<bool> WaitForStateAsync(ConnectState target, TimeSpan? timeout)
    {
        var waiter = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        lock (_stateGate)
        {
            // Abandon any previously pending wait.
            _stateWaiter?.TrySetResult(false);
            _stateWaiter = waiter;
            _awaitedState = target;
        }

        using CancellationTokenSource? cts = timeout is { } t ? new CancellationTokenSource(t) : null;
        using CancellationTokenRegistration reg =
            cts?.Token.Register(static w => ((TaskCompletionSource<bool>)w!).TrySetResult(false), waiter)
            ?? default;

        bool reached = await waiter.Task.ConfigureAwait(false);

        lock (_stateGate)
        {
            if (ReferenceEquals(_stateWaiter, waiter))
            {
                _stateWaiter = null;
                _awaitedState = null;
            }
        }

        return reached;
    }

    /// <summary>Completes the pending state wait, if any, with the given result.</summary>
    private void CompleteStateWait(bool result)
    {
        TaskCompletionSource<bool>? waiter;
        lock (_stateGate)
        {
            waiter = _stateWaiter;
            _stateWaiter = null;
            _awaitedState = null;
        }
        waiter?.TrySetResult(result);
    }

    // -------------------------------------------------------------------------
    // IDetectReaderEventListener
    // -------------------------------------------------------------------------

    public void OnDetectReader(Com.Unitech.Lib.Detect.UnitechDevice? device)
    { if (device is not null) DeviceFound?.Invoke(this, device.ToAppModel()); }

    public void OnDetectPairedReader(Com.Unitech.Lib.Detect.UnitechDevice? device)
    { if (device is not null) PairedDeviceFound?.Invoke(this, device.ToAppModel()); }

    public void OnDetectNewPairedReader(Com.Unitech.Lib.Detect.UnitechDevice? device)
    { if (device is not null) DevicePaired?.Invoke(this, device.ToAppModel()); }

    public void OnDetectFinished() =>
        DiscoveryFinished?.Invoke(this, EventArgs.Empty);

    // -------------------------------------------------------------------------
    // IReaderEventListener
    // -------------------------------------------------------------------------

    public void ConnectStateEvent(BaseReader? reader, UnitechConnectState? nativeState, Java.Lang.Object? param)
    {
        if (nativeState is not null)
        {
            var state = nativeState.ToAppModel();
            if (state == ConnectState.Connected)
            {
                // The scan engine is only created once the reader has connected and
                // identified the device, so BaseEngine is null before Connect(). The
                // IBarcodeEventListener can only be attached to the engine, so it must
                // be registered here — registering earlier silently no-ops and no
                // BarcodeEvent (and therefore no BarcodeScanned) is ever raised.
                reader?.BaseEngine?.AddListener(this);

                // Apply the configured options to the reader. This is done here rather than
                // earlier to ensure the reader is fully initialized.
                var barcodeReader = reader as BarcodeReader;
                barcodeReader?.SetParameter(new DeviceParam(options.DataTerminator.Param.ToNativeModel(), options.DataTerminator.Value.ToNativeModel()), this);
                barcodeReader?.SetParameter(new DeviceParam(options.BeeperVolume.Param.ToNativeModel(), options.BeeperVolume.Value.ToNativeModel()), this);
            }

            // Release any ConnectAsync/DisconnectAsync awaiting this state.
            TaskCompletionSource<bool>? waiter = null;
            lock (_stateGate)
            {
                if (_stateWaiter is not null && _awaitedState is not null && state.Equals(_awaitedState))
                {
                    waiter = _stateWaiter;
                    _stateWaiter = null;
                    _awaitedState = null;
                }
            }
            waiter?.TrySetResult(true);

            var device = new UnitechDevice(reader?.DeviceType?.ToAppModel(), reader?.ConnectType?.ToAppModel(), reader?.DeviceName, reader?.Address);
            ConnectionChanged?.Invoke(this, (state, device));
        }
    }

    public void BatteryPercentageEvent(BaseReader? reader, sbyte percentage, bool charging, Java.Lang.Object? param) =>
        BatteryPercentageChanged?.Invoke(this, (percentage, charging));

    public void BatteryStateEvent(BaseReader? reader, UnitechBatteryState? state, Java.Lang.Object? param)
    {
        if (state is not null)
            BatteryStateChanged?.Invoke(this, state.ToAppModel());
    }

    public void NotificationEvent(BaseReader? reader, NotificationID? id, Java.Lang.Object? param)
    { }

    // -------------------------------------------------------------------------
    // IBarcodeEventListener
    // -------------------------------------------------------------------------

    public void BarcodeEvent(BaseReader? reader, UnitechBarcodeID? symbology, byte[]? data, Java.Lang.Object? param)
    {
        if (data is not null)
        {
            var barcodeData = new BarcodeData(data, symbology?.ToAppModel() ?? BarcodeID.Unknown);
            BarcodeScanned?.Invoke(this, barcodeData);
        }
    }

    /// <summary>
    /// Generates a barcode image used for device pairing.
    /// </summary>
    /// <param name="macAdress">The MAC address of the device to pair, or null to use the default device. If specified, must be a valid MAC address format.</param>
    /// <returns>
    /// An ImageSource representing the pairing barcode, or null if the barcode could not be generated.
    /// </returns>
    public string GetPairingBarcode(string? macAdress = null)
    {
        //Verify Mac Address Requirement
        if (options.IsMacAddressRequired)
        {
            //Mac Address Supplied?
            if (string.IsNullOrEmpty(macAdress)) throw new ArgumentNullException(nameof(macAdress), "GetPairingBarcode: Mac address is required!");

            //Get Pairing Barcode with Mac Address
            return $"SPP{macAdress?.Replace(":", "").ToUpper()}";
        }
        else
        {
            //Get Rapd Pairing Barcode
            var identifier = new string([.. Enumerable.Range(0, 12).Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[RandomNumberGenerator.GetInt32(36)])]);
            return $"//.STC{identifier}";
        }
    }

    // -------------------------------------------------------------------------
    // Cleanup
    // -------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _detector?.Destroy();
            _detector = null;

            // Synchronous teardown — Dispose can't await, and there's no point
            // waiting for a state callback while shutting down.
            CompleteStateWait(result: false);
            _reader?.Disconnect();
            _reader?.Destroy();
            _reader = null;
        }
        base.Dispose(disposing);
    }

    public void OnFail(int id, ResultCode? result)
    {
        var paramId = id;
        var resultCode = result;
    }

    public void OnFinish(int id)
    {
        var paramId = id;
    }

    public void OnReceive(int id, BaseParam? param)
    {
        var paramId = id;
        var baseParam = param;
    }
}