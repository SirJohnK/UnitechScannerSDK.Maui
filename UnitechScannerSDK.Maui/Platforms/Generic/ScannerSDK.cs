namespace UnitechScannerSDK;

public sealed partial class ScannerSDK(UnitechScannerSDKOptions options) : IScannerSDK
{
    public UnitechScannerSDKOptions Options => options;

    public Task<bool> ConnectAsync(UnitechDevice device, TimeSpan? timeout = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DisconnectAsync(TimeSpan? timeout = null)
    {
        throw new NotImplementedException();
    }

    public void StartDiscovery(bool keepDetecting = false)
    {
        throw new NotImplementedException();
    }

    public void StopDiscovery()
    {
        throw new NotImplementedException();
    }

    public string GetPairingBarcode(string? macAdress = null)
    {
        throw new NotImplementedException();
    }
}