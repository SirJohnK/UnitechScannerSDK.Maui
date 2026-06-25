namespace UnitechScannerSDK;

public sealed partial class ScannerService : IScannerService
{
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
}