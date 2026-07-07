namespace UnitechScannerSDK;

public sealed partial class ScannerSDK
{
    public event EventHandler<BarcodeData>? BarcodeScanned;

    public event EventHandler<(sbyte Percentage, bool IsCharging)>? BatteryPercentageChanged;

    public event EventHandler<BatteryState>? BatteryStateChanged;

    public event EventHandler<(ConnectState State, UnitechDevice Device)>? ConnectionChanged;

    public event EventHandler<UnitechDevice>? DeviceFound;

    public event EventHandler<UnitechDevice>? PairedDeviceFound;

    public event EventHandler<UnitechDevice>? DevicePaired;

    public event EventHandler? DiscoveryFinished;
}