using System.Security.Cryptography;

namespace UnitechScannerSDK.Maui.Sample;

/// <summary>
/// Managing Unitech barcode scanners, including detection, connection events, and pairing barcode generation.
/// </summary>
/// <remarks>
/// Provides mechanisms to detect Unitech scanners, handle scanner connection and disconnection events,
/// and generate pairing barcodes. Supports asynchronous operations and event-driven scanner management.
/// All members are intended to be used in environments where multiple scanners may be present and dynamically connected or disconnected.
/// </remarks>
public class UnitechScannerManager : IUnitechScannerManager, IDisposable
{
    private readonly IScannerSDK scannerSDK;
    private readonly IDictionary<string, IUnitechScanner> scanners;

    /// <summary>
    /// Event Invoked when Scanner Appears.
    /// </summary>
    public event EventHandler<IUnitechScanner>? Appeared;

    /// <summary>
    /// Event Invoked when Scanner Paired.
    /// </summary>
    public event EventHandler<IUnitechScanner>? Paired;

    /// <summary>
    /// Event Invoked when Barcode being Scanned.
    /// </summary>
    public event EventHandler<BarcodeData>? BarcodeScanned;

    /// <summary>
    /// Event Invoked when Scanner Connected.
    /// </summary>
    public event EventHandler<IUnitechScanner>? Connected;

    /// <summary>
    /// Event Invoked when Scanner Disconnected.
    /// </summary>
    public event EventHandler<string>? Disconnected;

    public UnitechScannerManager(IScannerSDK scannerSDK)
    {
        //Init
        this.scannerSDK = scannerSDK;
        scanners = new Dictionary<string, IUnitechScanner>();

        //Setup Event handlers
        scannerSDK.BarcodeScanned += ScannerBarcodeData;
        scannerSDK.DeviceFound += ScannerAppeared;
        scannerSDK.PairedDeviceFound += ScannerAppeared;
        scannerSDK.DevicePaired += ScannerPaired;
        scannerSDK.ConnectionChanged += ConnectionChanged;
    }

    private void ScannerPaired(object? sender, UnitechDevice device)
    {
        //Update scanners dictionary
        var scanner = new UnitechScanner(device, scannerSDK);
        scanners[scanner.Id] = scanner;

        //Invoke Paired Event
        Paired?.Invoke(this, scanner);
    }

    private void ConnectionChanged(object? sender, (ConnectState State, UnitechDevice Device) e)
    {
        var scanner = new UnitechScanner(e.Device, scannerSDK);
        if (e.State == ConnectState.Connected)
        {
            scanner.IsConnected = true;
            scanners[scanner.Id] = scanner;
            Connected?.Invoke(this, scanner);
        }
        else if (e.State == ConnectState.Disconnected)
        {
            scanners.Remove(scanner.Id);
            Disconnected?.Invoke(this, scanner.Id);
        }
    }

    private void ScannerAppeared(object? sender, UnitechDevice device)
    {
        //Update scanners dictionary
        var scanner = new UnitechScanner(device, scannerSDK);
        scanners[scanner.Id] = scanner;

        //Invoke Appeared Event
        Appeared?.Invoke(this, scanner);
    }

    private void ScannerBarcodeData(object? sender, BarcodeData barcodeData)
    {
        //Trigger BarcodeScanned Event
        BarcodeScanned?.Invoke(this, barcodeData);
    }

    /// <summary>
    /// Gets or Sets a value indicating whether a MAC address is required for generating a pairing barcode.
    /// </summary>
    public bool IsMacAddressRequired => scannerSDK.Options.IsMacAddressRequired;

    /// <summary>
    /// Generates a barcode image used for device pairing.
    /// </summary>
    /// <param name="macAdress">The MAC address of the device to pair, or null to use the default device. If specified, must be a valid MAC address format.</param>
    /// <returns>
    /// An ImageSource representing the pairing barcode, or null if the barcode could not be generated.
    /// </returns>
    public string GetPairingBarcode(string? macAdress = null) => scannerSDK.GetPairingBarcode(macAdress);

    /// <summary>
    /// Enables scanner detection and will trigger scanner events.
    /// </summary>
    public void EnableDetection(bool keepDetecting = false)
    {
        scannerSDK.StartDiscovery(keepDetecting);
    }

    /// <summary>
    /// Disables scanner detection and will stop triggering of scanner events.
    /// </summary>
    public void DisableDetection()
    {
        scannerSDK.StopDiscovery();
    }

    public void Dispose()
    {
        //Clean up
        DisableDetection();
        scannerSDK.BarcodeScanned -= ScannerBarcodeData;
        scannerSDK.DeviceFound -= ScannerAppeared;
        scannerSDK.PairedDeviceFound -= ScannerAppeared;
        scannerSDK.DevicePaired -= ScannerPaired;
        scannerSDK.ConnectionChanged -= ConnectionChanged;
    }
}