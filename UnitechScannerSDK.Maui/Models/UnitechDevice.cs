namespace UnitechScannerSDK;

public class UnitechDevice(
    DeviceType? deviceType = DeviceType.Unknown,
    ConnectType? connectType = null,
    string? name = null,
    string? address = null)
{
    public string? Address { get; } = address;
    public ConnectType? ConnectType { get; } = connectType;
    public DeviceType? DeviceType { get; } = deviceType;
    public string? Name { get; } = name;
}