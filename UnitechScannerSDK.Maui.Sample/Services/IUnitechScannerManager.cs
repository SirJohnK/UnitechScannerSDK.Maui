namespace UnitechScannerSDK.Maui.Sample;

/// <summary>
/// Defines an interface for managing Unitech barcode scanners, including detection, connection events, and pairing barcode generation.
/// </summary>
/// <remarks>
/// Implementations of this interface provide mechanisms to detect Unitech scanners, handle scanner
/// connection and disconnection events, and generate pairing barcodes. The interface is
/// designed to support asynchronous operations and event-driven scanner management. All members are intended to be used
/// in environments where multiple scanners may be present and dynamically connected or disconnected.
/// </remarks>
public interface IUnitechScannerManager : IDisposable
{
    /// <summary>
    /// Enables scanner detection and will trigger scanner events.
    /// </summary>
    /// <param name="keepDetecting">Indicates whether the detection should continue after the first scanner is found.</param>
    void EnableDetection(bool keepDetecting = false);

    /// <summary>
    /// Disables scanner detection and will stop triggering of scanner events.
    /// </summary>
    void DisableDetection();

    /// <summary>
    /// Event Invoked when Scanner Appears.
    /// </summary>
    event EventHandler<IUnitechScanner>? Appeared;

    /// <summary>
    /// Event Invoked when Barcode being Scanned.
    /// </summary>
    event EventHandler<BarcodeData>? BarcodeScanned;

    /// <summary>
    /// Event Invoked when Scanner Connected.
    /// </summary>
    event EventHandler<IUnitechScanner>? Connected;

    /// <summary>
    /// Event Invoked when Scanner Disconnected.
    /// </summary>
    event EventHandler<string>? Disconnected;

    /// <summary>
    /// Gets a value indicating whether a MAC address is required for generating a pairing barcode.
    /// </summary>
    bool IsMacAddressRequired { get; }

    /// <summary>
    /// Generates a string representing the pairing barcode for a device.
    /// </summary>
    /// <param name="macAdress">The MAC address of the device to pair, or null to use rapid pairing barcode. If specified, must be a valid MAC address format.</param>
    /// <returns>
    /// A string representing the pairing barcode.
    /// </returns>
    string GetPairingBarcode(string? macAdress = null);
}