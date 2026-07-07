namespace UnitechScannerSDK;

public enum DeviceParamID
{
    AutoConnecting = 0,
    AutoPowerOff = 1,
    BatteryStatus = 2,
    BeeperVolume = 3,
    BluetoothMAC = 4,
    BluetoothName = 5,
    BluetoothSignalCheckingLevel = 6,
    ControlCharacterFilter = 7,
    CradleAutoPair = 8,
    CradleAutoPresentationMode = 9,
    DataIndicator = 10,
    DataSendingInterBlockDelay = 11,
    DataTerminator = 12,
    EndOfBatchSendingMessage = 13,
    EngineModel = 14,
    EngineVersion = 15,
    FindReplace = 16,
    Firmware = 17,
    Gs1AI = 18,
    HidEncoding = 19,
    HidInterCharacterDelay = 20,
    HidKeyboardCase = 21,
    HidKeyboardLanguage = 22,
    HidOutputType = 23,
    Indicator = 24,
    LedControl = 25,
    OperationMode = 26,
    Reset = 27,
    SaveBufferWhilePowerOff = 28,
    ScannerIndicator = 29,
    SerialNumber = 30,
    SettingByLabel = 31,
    StartDecode = 32,
    StopDecode = 33,
    TriggerKeyStatus = 34,
    UnpairBluetooth = 35,
    Vibrator = 36
}

public interface IDeviceParam<T> where T : Enum
{
    DeviceParamID Param { get; }
    T Value { get; }
}

public class DeviceParam<T>(DeviceParamID param, T value) : IDeviceParam<T> where T : Enum
{
    public static DeviceParam<T> Create<T>(DeviceParamID param, T value) where T : Enum => new DeviceParam<T>(param, value);

    public DeviceParamID Param { get; } = param;
    public T Value { get; set; } = value;
}

public enum BeeperVolumeParam
{
    Mute = 0,
    Low = 1,
    Medium = 2,
    High = 3
}

public enum DataTerminatorParam
{
    None = 0,
    CR = 1,
    LF = 2,
    CRLF = 3,
    Tab = 4,
}