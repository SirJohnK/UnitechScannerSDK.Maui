namespace UnitechScannerSDK.Maui.Sample;

/// <summary>
/// Represents a Unitech barcode scanner device and provides methods to manage its connection and properties.
/// </summary>
/// <remarks>
/// This interface defines the contract for interacting with a Unitech scanner, including retrieving device
/// information and managing the connection state.
/// </remarks>
public interface IUnitechScanner
{
    public string Id { get; }
    public string? Name { get; }
    public string? Description { get; }
    public string? ModelNumber { get; }
    public string ManufacturerName { get; }
    public bool IsConnected { get; set; }

    public Task<bool> Connect();

    public Task<bool> Disconnect();
}