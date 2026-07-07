using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitechScannerSDK.Maui.Sample;

/// <summary>
/// Represents a Unitech barcode scanner device and provides methods to manage its connection and access its properties.
/// </summary>
/// <remarks>
/// This class serves as a high-level abstraction for interacting with Unitech Technologies barcode
/// scanners. It exposes device information and connection management functionality.
/// </remarks>
public partial class UnitechScanner(UnitechDevice device, IScannerSDK scannerSDK) : ObservableObject, IUnitechScanner
{
    public string Id => device.Address ?? "11:22:33:44:55:66";

    public string? Name => device.Name ?? ModelNumber ?? Id;

    public string? DeviceName;

    public string? Description => $"{ManufacturerName}, {DeviceName}, {Id}";

    public string? ModelNumber => $"{device.DeviceType}";

    public string ManufacturerName => "Unitech";

    [ObservableProperty]
    private bool isConnected = false;

    public async Task<bool> Connect()
    {
        try
        {
            //Attempt to Connect to Scanner!
            return IsConnected = await scannerSDK.ConnectAsync(device);
        }
        catch (Exception)
        {
            //Return failed state
            return IsConnected = false;
        }
    }

    public async Task<bool> Disconnect()
    {
        try
        {
            //Attempt to Disconnect from Scanner!
            return IsConnected = !await scannerSDK.DisconnectAsync();
        }
        catch (Exception)
        {
            //Return current state
            return IsConnected;
        }
    }
}