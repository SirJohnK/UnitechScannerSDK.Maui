using Microsoft.Extensions.DependencyInjection;

namespace UnitechScannerSDK;

public static class ServiceExtensions
{
    public static IServiceCollection AddUnitechScannerSDK(this IServiceCollection services, Action<UnitechScannerSDKOptions>? configure = null)
    {
        // Create an instance of UnitechScannerSDKOptions and apply the configuration action if provided
        var options = new UnitechScannerSDKOptions();
        configure?.Invoke(options);

        // Register the options and the ScannerSDK service
        return services.AddSingleton(options).AddSingleton<IScannerSDK, ScannerSDK>();
    }
}

public class UnitechScannerSDKOptions
{
    /// <summary>
    /// Gets or Sets a value indicating whether a MAC address is required for generating a pairing barcode. (Default: true)
    /// </summary>
    public bool IsMacAddressRequired { get; set; } = true;

    public DeviceParam<DataTerminatorParam> DataTerminator { get; } = DeviceParam<DataTerminatorParam>.Create(DeviceParamID.DataTerminator, DataTerminatorParam.None);

    public DeviceParam<BeeperVolumeParam> BeeperVolume { get; } = DeviceParam<BeeperVolumeParam>.Create(DeviceParamID.BeeperVolume, BeeperVolumeParam.High);
}