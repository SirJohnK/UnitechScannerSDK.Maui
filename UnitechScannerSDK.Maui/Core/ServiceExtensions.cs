using Microsoft.Extensions.DependencyInjection;

namespace UnitechScannerSDK;

public static class ServiceExtensions
{
    public static IServiceCollection AddUnitechScannerSDK(this IServiceCollection services)
    {
        return services.AddSingleton<IScannerService, ScannerService>();
    }
}