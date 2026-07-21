# UnitechScannerSDK.Maui <a href="https://www.buymeacoffee.com/sirjohnk" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" align="right" style="height: 37px !important;width: 170px !important;" ></a>

Enhanced .NET MAUI version of the Unitech Scanner SDK.

## NuGet

|Name|Info|
| ------------------- | :------------------: |
|UnitechScannerSDK.Maui|[![NuGet](https://img.shields.io/nuget/v/UnitechScannerSDK.Maui)](https://www.nuget.org/packages/UnitechScannerSDK.Maui/#versions-body-tab)|

## Background
When implementing support for a Unitech Scanner in a .NET MAUI application, I discovered that the ***Unitech Scanner SDK*** for Android is only available as a native Android library, so this library was created.

## What's included?
We have some enhanced and added features:
- One easy to use NuGet package.
- Easy setup with service collection extension.
- Pairing barcode data. (Both Mac Address and Rapid barcodes)
- Scanner configuration.
- New `IScannerSDK` interface registered for constructor injection with DI.

## Setup
Use the `AddUnitechScannerSDK` service collection extension method for library configuration.
```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    })

    //Add Services
    builder.Services.AddUnitechScannerSDK();
```
Configuration contains 3 options:
- **BeeperVolume** (Set Scanner Beep volume. Default: ***High***)
- **DataTerminator** (Set Scanner Barcode output terminator. Default: ***None***)
- **IsMacAddressRequired** (Control pairing barcode type. Device Mac Address required (***True***) or Rapid barcode (***False***). Default: ***True***)
```csharp
builder.Services.AddUnitechScannerSDK(config =>
{
    config.BeeperVolume.Value = BeeperVolumeParam.Low;
    config.DataTerminator.Value = DataTerminatorParam.CRLF;
    config.IsMacAddressRequired = false;
});
```

For further documentation, see the [Official Unitech Resources](https://www.ute.com/en) or the [Sample application](#sample).

## Sample
Look at the [Sample project](https://github.com/SirJohnK/UnitechScannerSDK.Maui/tree/master/UnitechScannerSDK.Maui.Sample) for a example of how to use this library in an .NET MAUI application.

[![Main View](https://github.com/SirJohnK/UnitechScannerSDK.Maui/blob/master/Docs/MainView.png)](https://github.com/SirJohnK/UnitechScannerSDK.Maui/tree/master/UnitechScannerSDK.Maui.Sample)
[![Pairing View](https://github.com/SirJohnK/UnitechScannerSDK.Maui/blob/master/Docs/PairingView.png)](https://github.com/SirJohnK/UnitechScannerSDK.Maui/tree/master/UnitechScannerSDK.Maui.Sample)

## Official Unitech Resources
- [Scanner SDK for Android (Supporting Software Section)](https://www.ute.com/en/products/detail/MS626Plus)
