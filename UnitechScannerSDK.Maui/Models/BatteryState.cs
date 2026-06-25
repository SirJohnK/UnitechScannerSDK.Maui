namespace UnitechScannerSDK;

public enum BatteryState
{
    Unknown = byte.MaxValue,
    Charging = 0,
    VeryLow = 1,
    Low = 2,
    Normal = 3,
    FullCharged = 4
}