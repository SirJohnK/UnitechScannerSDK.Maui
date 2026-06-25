using System.Collections.Concurrent;
using System.Reflection.PortableExecutable;
using UnitechScannerSDK.Platforms.iOS;
using static CoreFoundation.DispatchSource;
using SDK = ScannerSDKBinding;

namespace UnitechScannerSDK;

/// <summary>
/// iOS implementation of <see cref="IScannerService"/>. Wraps the Unitech
/// ScannerSDK MFI binding behind the same clean, event-driven API exposed by the
/// Android service.
/// </summary>
/// <remarks>
/// On iOS readers are accessed through the MFI ExternalAccessory framework, which
/// differs from Android in two important ways:
/// <list type="bullet">
/// <item>Discovery is driven by accessory connect/disconnect notifications rather
/// than an explicit Bluetooth scan, so there is no "discovery finished" callback
/// (<see cref="DiscoveryFinished"/> is therefore never raised).</item>
/// <item>A native <see cref="SDK.UnitechDevice"/> can only be created from a live
/// <c>EAAccessory</c>; it cannot be rebuilt from the app model. The native devices
/// surfaced during detection are cached so <see cref="ConnectAsync"/> can resolve
/// them again by address.</item>
/// </list>
/// </remarks>
public sealed partial class ScannerService : NSObject,
    SDK.IDetectReaderDelegate,
    SDK.IReaderDelegate,
    SDK.IBarcodeDelegate,
    IScannerService
{
    private SDK.DetectReader? _detector;
    private SDK.BarcodeReader? _reader;

    // The ScannerSDK binding types its delegate properties as concrete model
    // classes (DetectReaderDelegate/ReaderDelegate/BarcodeDelegate) and exposes
    // no weak-delegate counterpart, so this NSObject — which implements the
    // protocols via its [Export]ed callbacks — cannot be assigned to them
    // directly. Attach it instead by key-value coding against the native (weak)
    // ObjC delegate properties.
    private static readonly NSString DetectDelegateKey = new("delegate");
    private static readonly NSString ReaderDelegateKey = new("readerDelegate");
    private static readonly NSString BarcodeDelegateKey = new("barcodeDelegate");

    // Native devices discovered during detection, keyed by their address
    // (serial number). ConnectAsync looks devices up here because a native
    // UnitechDevice cannot be reconstructed from the app model.
    private readonly ConcurrentDictionary<string, SDK.UnitechDevice> _detected = new();

    // Coordinates ConnectAsync/DisconnectAsync with the asynchronous
    // ConnectStateEvent callback. Guarded by _stateGate.
    private readonly object _stateGate = new();

    private TaskCompletionSource<bool>? _stateWaiter;
    private ConnectState? _awaitedState;

    // -------------------------------------------------------------------------
    // Discovery
    // -------------------------------------------------------------------------

    public void StartDiscovery(bool keepDetecting = false)
    {
        _detector ??= new SDK.DetectReader();
        _detector.SetValueForKey(this, DetectDelegateKey);
        _detector.StartDetect();
    }

    public void StopDiscovery()
    {
        _detector?.StopDetect();
        if (_detector is not null)
            _detector.SetValueForKey(null!, DetectDelegateKey);
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

        // The native device must have been seen during detection; it cannot be
        // recreated from the app model on iOS.
        string? key = device.Address ?? device.Name;
        if (key is null || !_detected.TryGetValue(key, out SDK.UnitechDevice? nativeDevice))
            return false;

        var reader = new SDK.BarcodeReader(nativeDevice);
        reader.SetValueForKey(this, ReaderDelegateKey);
        reader.SetValueForKey(this, BarcodeDelegateKey);
        _reader = reader;

        // Arm the waiter before kicking off the connection so a fast
        // ConnectStateEvent cannot fire before we are listening for it.
        Task<bool> reachedConnected = WaitForStateAsync(ConnectState.Connected, timeout);

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

        Task<bool> reachedDisconnected = WaitForStateAsync(ConnectState.Disconnected, timeout);

        await Task.Run(reader.Disconnect).ConfigureAwait(false);

        bool disconnected = await reachedDisconnected.ConfigureAwait(false);

        // Tear down only after the disconnected state has been observed (or the
        // wait timed out) so the final ConnectStateEvent still reaches us.
        reader.SetValueForKey(null!, ReaderDelegateKey);
        reader.SetValueForKey(null!, BarcodeDelegateKey);
        reader.Dispose();
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
    // IDetectReaderDelegate
    // -------------------------------------------------------------------------

    [Export("didConnectReader:")]
    public void DidConnectReader(SDK.UnitechDevice reader)
    {
        // Cache the native device so ConnectAsync can resolve it again, then
        // surface it to the app as a discovered device.
        UnitechDevice device = reader.ToAppModel();
        string? key = device.Address ?? device.Name;
        if (key is not null)
            _detected[key] = reader;

        DeviceFound?.Invoke(this, device);
    }

    [Export("didDisconnectReader:")]
    public void DidDisconnectReader(SDK.UnitechDevice reader)
    {
        // An MFI accessory was unplugged/powered off. Drop it from the cache; the
        // reader-level ConnectStateEvent is responsible for surfacing the
        // disconnected state to the app.
        UnitechDevice device = reader.ToAppModel();
        string? key = device.Address ?? device.Name;
        if (key is not null)
            _detected.TryRemove(key, out _);
    }

    // -------------------------------------------------------------------------
    // IReaderDelegate
    // -------------------------------------------------------------------------

    [Export("connectStateEvent:")]
    public void ConnectStateEvent(SDK.ConnectStateEventArgs connectStateEventArgs)
    {
        var state = connectStateEventArgs.ConnectState.ToAppModel();

        // Release any ConnectAsync/DisconnectAsync awaiting this state.
        TaskCompletionSource<bool>? waiter = null;
        lock (_stateGate)
        {
            if (_stateWaiter is not null && _awaitedState is not null && state == _awaitedState)
            {
                waiter = _stateWaiter;
                _stateWaiter = null;
                _awaitedState = null;
            }
        }
        waiter?.TrySetResult(true);

        var reader = connectStateEventArgs.Reader;
        var device = new UnitechDevice(reader?.DeviceType.ToAppModel(), reader?.ConnectType.ToAppModel(), reader?.DeviceName, reader?.Address);
        ConnectionChanged?.Invoke(this, (state, device));
    }

    [Export("batteryStateEvent:")]
    public void BatteryStateEvent(SDK.BatteryState batteryState) =>
        BatteryStateChanged?.Invoke(this, batteryState.ToAppModel());

    [Export("batteryPercentageEvent:charging:")]
    public void BatteryPercentageEvent(byte percentage, bool charging) =>
        BatteryPercentageChanged?.Invoke(this, (unchecked((sbyte)percentage), charging));

    [Export("notificationEvent:")]
    public void NotificationEvent(SDK.NotificationEventArgs notificationEventArgs)
    { }

    [Export("logEvent:")]
    public void LogEvent(SDK.LogEventArg logEventArgs)
    { }

    [Export("firmwareUpdateEvent:")]
    public void FirmwareUpdateEvent(SDK.FirmwareUpdateEventArgs firmwareUpdateEventArgs)
    { }

    [Export("triggerKeyStatusEvent:")]
    public void TriggerKeyStatusEvent(SDK.TriggerKeyStatusEventArgs triggerKeyStatusEventArgs)
    { }

    [Export("errorReceived:")]
    public void ErrorReceived(string message)
    { }

    // -------------------------------------------------------------------------
    // IBarcodeDelegate
    // -------------------------------------------------------------------------

    [Export("barcodeEvent:")]
    public void BarcodeEvent(SDK.BarcodeEventArgs barcodeEventArgs)
    {
        if ((barcodeEventArgs.Data?.Length ?? 0) > 0)
        {
            // The binding projects the native [UInt8] payload as NSNumber[], so unwrap
            // each element back into a byte before decoding.
            var barcodeData = new BarcodeData(Array.ConvertAll(barcodeEventArgs.Data!, n => n.ByteValue), barcodeEventArgs.BarcodeID.ToAppModel());
            BarcodeScanned?.Invoke(this, barcodeData);
        }
    }

    // -------------------------------------------------------------------------
    // Cleanup
    // -------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _detector?.StopDetect();
            if (_detector is not null)
                _detector.SetValueForKey(null!, DetectDelegateKey);
            _detector?.Dispose();
            _detector = null;

            // Synchronous teardown — Dispose can't await, and there's no point
            // waiting for a state callback while shutting down.
            CompleteStateWait(result: false);
            _reader?.Disconnect();
            _reader?.Dispose();
            _reader = null;

            _detected.Clear();
        }
        base.Dispose(disposing);
    }
}