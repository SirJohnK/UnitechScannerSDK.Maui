namespace UnitechScannerSDK.Maui.Sample;

/// <summary>
/// Requests the Android 12+ (API 31) runtime Bluetooth permissions the Unitech Scanner
/// SDK needs for device discovery. <c>BLUETOOTH_SCAN</c> in particular must be granted
/// before <c>DetectReader</c>/<c>BarcodeReader</c> start discovery, otherwise the platform
/// throws a fatal <c>SecurityException</c> ("Need android.permission.BLUETOOTH_SCAN ...")
/// from <c>BluetoothAdapter.startDiscovery()</c>.
///
/// The built-in <see cref="Permissions.Bluetooth"/> does not reliably prompt for
/// SCAN/CONNECT, so we declare the exact runtime permissions here.
/// </summary>
public class BluetoothPermissions : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
    [
        (global::Android.Manifest.Permission.BluetoothScan, true),
        (global::Android.Manifest.Permission.BluetoothConnect, true),
    ];

    /// <summary>
    /// Ensures the runtime Bluetooth permissions required for discovery are granted,
    /// prompting the user if necessary. Returns <c>true</c> only when granted.
    /// Must be called on the UI thread.
    /// </summary>
    public static async Task<bool> EnsureGrantedAsync()
    {
        var status = await Permissions.CheckStatusAsync<BluetoothPermissions>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<BluetoothPermissions>();
        return status == PermissionStatus.Granted;
    }
}
