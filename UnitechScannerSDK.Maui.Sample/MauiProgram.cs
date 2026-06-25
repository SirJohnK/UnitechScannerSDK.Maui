using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui;

namespace UnitechScannerSDK.Maui.Sample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseBarcodeGenerator()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialFontFamily");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //Add Services
            builder.Services.AddUnitechScannerSDK();
            builder.Services.AddSingleton<IUnitechScannerManager, UnitechScannerManager>();

            //Add Views
            builder.Services.AddTransient<MainView>();

            //Add ViewModels
            builder.Services.AddTransient<MainViewModel>();

            //Add Popups
            builder.Services.AddTransientPopup<ScannerPairingPopupView, ScannerPairingPopupViewModel>();
            builder.Services.AddTransientPopup<BluetoothAddressPopupView, BluetoothAddressPopupViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}