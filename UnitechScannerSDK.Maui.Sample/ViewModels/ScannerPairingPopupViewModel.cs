using AsyncAwaitBestPractices;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
public partial class ScannerPairingPopupViewModel(IUnitechScannerManager scannerManager, IPopupService popupService) : ObservableObject
{
    private const string BluetoothMacAddress = nameof(BluetoothMacAddress);

    [ObservableProperty]
    private string? macAddress;

    [ObservableProperty]
    private string? pairingBarcode;

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
        if (!scannerManager.IsMacAddressRequired)
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
        scannerManager.DisableDetection();

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