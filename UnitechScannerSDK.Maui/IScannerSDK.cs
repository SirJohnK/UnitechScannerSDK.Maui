namespace UnitechScannerSDK;

public interface IScannerSDK
{
    event EventHandler<BarcodeData>? BarcodeScanned;

    event EventHandler<(sbyte Percentage, bool IsCharging)>? BatteryPercentageChanged;

    event EventHandler<BatteryState>? BatteryStateChanged;

    event EventHandler<(ConnectState State, UnitechDevice Device)>? ConnectionChanged;

    event EventHandler<UnitechDevice>? DeviceFound;

    event EventHandler<UnitechDevice>? PairedDeviceFound;

    event EventHandler<UnitechDevice>? DevicePaired;

    event EventHandler? DiscoveryFinished;

    UnitechScannerSDKOptions Options { get; }

    Task<bool> ConnectAsync(UnitechDevice device, TimeSpan? timeout = null);

    Task<bool> DisconnectAsync(TimeSpan? timeout = null);

    void StartDiscovery(bool keepDetecting = false);

    void StopDiscovery();

    string GetPairingBarcode(string? macAdress = null);
}