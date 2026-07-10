using AsyncAwaitBestPractices;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.NFC;

namespace UnitechScannerSDK.Maui.Sample;

/// <summary>
/// Viewmodel for the scanner pairing popup, providing properties and commands to facilitate pairing
/// Unitech scanners via barcode and Bluetooth MAC address entry.
/// </summary>
/// <remarks>
/// Manages the workflow for pairing Unitech scanners, including handling Bluetooth MAC
/// address requirements, displaying pairing barcodes, and responding to scanner connection events. It exposes commands
/// for updating the Bluetooth address and canceling the pairing process. The view model subscribes to scanner events
/// when loaded and ensures proper cleanup when unloaded.</remarks>
/// <param name="scannerManager">The scanner manager used to control scanner detection and retrieve pairing barcodes.</param>
/// <param name="popupService">The popup service used to display dialogs and manage popup interactions.</param>
public partial class ScannerPairingPopupViewModel(IUnitechScannerManager scannerManager, INFC nfc, IPopupService popupService) : ObservableObject
{
    private const string BluetoothMacAddress = nameof(BluetoothMacAddress);

    [ObservableProperty]
    private string? macAddress;

    [ObservableProperty]
    private string? pairingBarcode;

    private string? deviceMacAddress = null;
    public bool IsMacAddressRequired => scannerManager.IsMacAddressRequired;
    public string SetFactoryDefaultsBarcode => ".A001$";
    public string SetBTSPPBarcode => ".E042$";

    private async Task<string?> GetBluetoothMacAddress(string? inputAddress = null, bool forceDialog = false)
    {
        //Init
        var macAddress = Preferences.Default.Get<string?>(BluetoothMacAddress, null);

        //Attempt to get mac address from settings
        if (forceDialog || macAddress is null)
        {
            //Request Bluetooth Mac Address from User'
            macAddress = (await popupService.ShowPopupAsync<BluetoothAddressPopupView, string?>(Shell.Current, shellParameters: new Dictionary<string, object>() { ["Input"] = macAddress! }))?.Result;
        }

        //Store Address in Settings
        if (!string.IsNullOrWhiteSpace(macAddress))
        {
            macAddress = macAddress.Replace(":", string.Empty);
            Preferences.Set(BluetoothMacAddress, macAddress);
        }

        //Return Address
        return macAddress;
    }

    public async Task OnLoadedAsync()
    {
        // Discovery requires the runtime BLUETOOTH_SCAN permission (Android 12+); without
        // it startDiscovery() throws a fatal SecurityException.
        if (!await BluetoothPermissions.EnsureGrantedAsync())
            return;

        //Enable Detection
        scannerManager.EnableDetection(true);

        //Subscribe to Scanner Events
        scannerManager.Connected += ScannerConnected;
        scannerManager.Appeared += ScannerAppeared;
        scannerManager.Paired += ScannerPaired;

        //Check if Bluetooth Mac Address is required
        if (scannerManager.IsMacAddressRequired)
        {
            //Get Bluetooth Address
            var macAddress = await GetBluetoothMacAddress();
            if (macAddress is not null) MacAddress = macAddress;
        }

        //Get Pairing Barcode
        if (!scannerManager.IsMacAddressRequired || !string.IsNullOrWhiteSpace(MacAddress))
            PairingBarcode = scannerManager.GetPairingBarcode(MacAddress);

        //Start NFC Listening, if supported
        if (nfc.IsAvailable && nfc.IsEnabled)
        {
            nfc.OnMessageReceived += Nfc_OnMessageReceived;
            nfc.StartListening();
        }
    }

    private void Nfc_OnMessageReceived(ITagInfo tagInfo)
    {
        // 0. Clear device mac address
        deviceMacAddress = null;

        // 1. Verify if the tag contains readable records
        if (tagInfo == null || tagInfo.Records == null || tagInfo.Records.Length == 0) return;

        // 2. Find the MAC address payload
        var macRecord = tagInfo.Records.FirstOrDefault(record => record.TypeFormat == NFCNdefTypeFormat.Mime && record.MimeType == "application/vnd.bluetooth.ep.oob");
        if (macRecord is not null)
        {
            // 3. Fetch the raw binary data
            var payload = macRecord.Payload;
            // Must have at least 2 bytes (length) + 6 bytes (MAC)
            if (payload is null || payload.Length < 8) return;

            // 4. Extract the 6 MAC address bytes (starting at offset index 2)
            byte[] macBytes = new byte[6];
            Array.Copy(payload, 2, macBytes, 0, 6);

            // 5. Reverse the array because Bluetooth OOB stores BD_ADDR in Little-Endian
            Array.Reverse(macBytes);

            // 6. Format into standard DC:0D:30:27:52:DB string presentation
            deviceMacAddress = BitConverter.ToString(macBytes).Replace("-", ":");

            // 7. If device is known, attempt to connect
            if (scannerManager.Scanners.TryGetValue(deviceMacAddress, out var scanner))
            {
                scannerManager.DisableDetection();
                scanner.Connect().SafeFireAndForget();
            }
        }
    }

    private void ScannerPaired(object? sender, IUnitechScanner scanner)
    {
        if (scannerManager.IsMacAddressRequired)
        {
            scannerManager.DisableDetection();
            scanner.Connect().SafeFireAndForget();
        }
    }

    private void ScannerAppeared(object? sender, IUnitechScanner scanner)
    {
        if (scanner.Id.Equals(deviceMacAddress))
        {
            scannerManager.DisableDetection();
            scanner.Connect().SafeFireAndForget();
        }
        else if (!scannerManager.IsMacAddressRequired)
        {
            var identifier = PairingBarcode?.Substring(6) ?? "N/A";
            if (scanner?.Name?.Contains(identifier) ?? false)
            {
                scannerManager.DisableDetection();
                scanner.Connect().SafeFireAndForget();
            }
        }
    }

    private void ScannerConnected(object? sender, IUnitechScanner scanner)
    {
        popupService.ClosePopupAsync(Shell.Current, scanner).SafeFireAndForget();
    }

    public Task OnUnloadedAsync()
    {
        //Clean up
        scannerManager.Connected -= ScannerConnected;
        scannerManager.Appeared -= ScannerAppeared;
        scannerManager.Paired -= ScannerPaired;
        scannerManager.DisableDetection();

        //Stop NFC Listening, if supported
        if (nfc.IsAvailable && nfc.IsEnabled)
        {
            nfc.OnMessageReceived -= Nfc_OnMessageReceived;
            nfc.StopListening();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Dialog CANCEL button command to close dialog. (Overrideable)
    /// </summary>
    /// <remarks>Will close dialog with <see langword="false"/> value respons.</remarks>
    /// <returns>Awaitable <see cref="Task"/>.</returns>
    [RelayCommand]
    public Task CancelButton() => popupService.ClosePopupAsync(Shell.Current);

    [RelayCommand]
    public async Task UpdateBluetoothAddress()
    {
        //Get Bluetooth Address
        var macAddress = await GetBluetoothMacAddress(MacAddress, true);
        if (macAddress is not null)
        {
            //Update Mac Address
            MacAddress = macAddress;

            //Get Pairing Barcode
            PairingBarcode = scannerManager.GetPairingBarcode(MacAddress);
        }
    }
}