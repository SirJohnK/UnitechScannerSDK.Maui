using ScannerSDKBinding;
using SDK = ScannerSDKBinding;

namespace UnitechScannerSDK.Platforms.iOS;

internal static class ModelExtensions
{
    /// <summary>
    /// Projects a native (MFI) <see cref="SDK.UnitechDevice"/> onto the app model.
    /// On iOS every reader is connected over MFI, and there is no MAC/Bluetooth
    /// address before connecting, so the stable <c>serialNumber</c> is used as the
    /// <see cref="UnitechDevice.Address"/>.
    /// </summary>
    internal static UnitechDevice ToAppModel(this SDK.UnitechDevice device)
    {
        return new UnitechDevice(
            deviceType: device.DeviceType.ToAppModel(),
            connectType: ConnectType.MFI,
            name: device.DeviceName,
            address: device.SerialNumber);
    }

    internal static DeviceType ToAppModel(this SDK.DeviceType deviceType)
    {
        // The native iOS enum exposes only the MFI-capable models and uses
        // different numeric values than the app enum, so map by name.
        return deviceType switch
        {
            SDK.DeviceType.Unknown => DeviceType.Unknown,
            SDK.DeviceType.MS852P => DeviceType.MS852P,
            SDK.DeviceType.MS652_Plus => DeviceType.MS652Plus,
            SDK.DeviceType.MS822B => DeviceType.MS822B,
            SDK.DeviceType.SL220 => DeviceType.SL220,
            SDK.DeviceType.MS633 => DeviceType.MS633,
            _ => DeviceType.Unknown
        };
    }

    internal static ConnectType ToAppModel(this SDK.ConnectType connectType)
    {
        return connectType switch
        {
            SDK.ConnectType.MFI => ConnectType.MFI,
            _ => ConnectType.Unknown
        };
    }

    /// <summary>
    /// Maps a native <see cref="SDK.ConnectState"/> to the app enum. The two
    /// enums share identical numeric values, so the value is reused directly with
    /// a guard against unexpected codes.
    /// </summary>
    internal static ConnectState ToAppModel(this SDK.ConnectState state)
    {
        return state switch
        {
            SDK.ConnectState.connected => ConnectState.Connected,
            SDK.ConnectState.connecting => ConnectState.Connecting,
            SDK.ConnectState.disconnected => ConnectState.Disconnected,
            SDK.ConnectState.disconnecting => ConnectState.Disconnecting,
            SDK.ConnectState.listen => ConnectState.Listen,
            _ => ConnectState.Disconnected
        };
    }

    /// <summary>
    /// Maps a native <see cref="SDK.BatteryState"/> to the app enum (identical
    /// numeric values, guarded).
    /// </summary>
    internal static BatteryState ToAppModel(this SDK.BatteryState state)
    {
        return state switch
        {
            SDK.BatteryState.Unknown => BatteryState.Unknown,
            SDK.BatteryState.Charging => BatteryState.Charging,
            SDK.BatteryState.FullCharged => BatteryState.FullCharged,
            SDK.BatteryState.Low => BatteryState.Low,
            SDK.BatteryState.Normal => BatteryState.Normal,
            SDK.BatteryState.VeryLow => BatteryState.VeryLow,
            _ => BatteryState.Unknown
        };
    }

    /// <summary>
    /// Maps a native <see cref="SDK.BarcodeID"/> to the app enum. Both enums are
    /// generated from the same symbology table and share numeric values, so the
    /// value is reused directly with a guard.
    /// </summary>
    internal static BarcodeID ToAppModel(this SDK.BarcodeID id)
    {
        long value = (long)id;
        return Enum.IsDefined(typeof(BarcodeID), (int)value)
            ? (BarcodeID)(int)value
            : BarcodeID.Unknown;
    }
}