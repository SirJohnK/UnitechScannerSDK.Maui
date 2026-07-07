using Android.OS;
using Com.Unitech.Lib.Model.Scanner;
using Com.Unitech.Lib.Types.Scanner;

namespace UnitechScannerSDK.Platforms.Android;

internal static class ModelExtensions
{
    internal static UnitechDevice ToAppModel(this Com.Unitech.Lib.Detect.UnitechDevice device)
    {
        return new UnitechDevice(
            deviceType: device.DeviceType?.ToAppModel(),
            connectType: device.ConnectType?.ToAppModel(),
            name: device.Name,
            address: device.Address);
    }

    internal static Com.Unitech.Lib.Detect.UnitechDevice ToNativeModel(this UnitechDevice device)
    {
        return new Com.Unitech.Lib.Detect.UnitechDevice(
            deviceType: device.DeviceType?.ToNativeModel(),
            connectType: device.ConnectType?.ToNativeModel(),
            name: device.Name,
            address: device.Address);
    }

    internal static DeviceType ToAppModel(this Com.Unitech.Lib.Types.DeviceType deviceType)
    {
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Unknown))
            return DeviceType.Unknown;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.MS626Plus))
            return DeviceType.MS626Plus;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Ms633))
            return DeviceType.MS633;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Ms633lr))
            return DeviceType.MS633LR;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.MS652Plus))
            return DeviceType.MS652Plus;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Ms822b))
            return DeviceType.MS822B;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Ms852blr))
            return DeviceType.MS852BLR;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Ms852p))
            return DeviceType.MS852P;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Rp300))
            return DeviceType.RP300;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Rt112))
            return DeviceType.RT112;
        if (deviceType.Equals(Com.Unitech.Lib.Types.DeviceType.Sl220))
            return DeviceType.SL220;

        return DeviceType.Unknown;
    }

    internal static Com.Unitech.Lib.Types.DeviceType? ToNativeModel(this DeviceType deviceType)
    {
        return deviceType switch
        {
            DeviceType.Unknown => Com.Unitech.Lib.Types.DeviceType.Unknown,
            DeviceType.MS626Plus => Com.Unitech.Lib.Types.DeviceType.MS626Plus,
            DeviceType.MS633 => Com.Unitech.Lib.Types.DeviceType.Ms633,
            DeviceType.MS633LR => Com.Unitech.Lib.Types.DeviceType.Ms633lr,
            DeviceType.MS652Plus => Com.Unitech.Lib.Types.DeviceType.MS652Plus,
            DeviceType.MS822B => Com.Unitech.Lib.Types.DeviceType.Ms822b,
            DeviceType.MS852BLR => Com.Unitech.Lib.Types.DeviceType.Ms852blr,
            DeviceType.MS852P => Com.Unitech.Lib.Types.DeviceType.Ms852p,
            DeviceType.RP300 => Com.Unitech.Lib.Types.DeviceType.Rp300,
            DeviceType.RT112 => Com.Unitech.Lib.Types.DeviceType.Rt112,
            DeviceType.SL220 => Com.Unitech.Lib.Types.DeviceType.Sl220,
            _ => Com.Unitech.Lib.Types.DeviceType.Unknown
        };
    }

    internal static ConnectType ToAppModel(this Com.Unitech.Lib.Types.ConnectType connectType)
    {
        if (connectType.Equals(Com.Unitech.Lib.Types.ConnectType.Unknown))
            return ConnectType.Unknown;
        if (connectType.Equals(Com.Unitech.Lib.Types.ConnectType.Bluetooth))
            return ConnectType.Bluetooth;
        if (connectType.Equals(Com.Unitech.Lib.Types.ConnectType.BluetoothLe))
            return ConnectType.BluetoothLE;

        return ConnectType.Unknown;
    }

    internal static Com.Unitech.Lib.Types.ConnectType? ToNativeModel(this ConnectType connectType)
    {
        return connectType switch
        {
            ConnectType.Unknown => Com.Unitech.Lib.Types.ConnectType.Unknown,
            ConnectType.Bluetooth => Com.Unitech.Lib.Types.ConnectType.Bluetooth,
            ConnectType.BluetoothLE => Com.Unitech.Lib.Types.ConnectType.BluetoothLe,
            _ => Com.Unitech.Lib.Types.ConnectType.Unknown
        };
    }

    internal static ConnectState ToAppModel(this Com.Unitech.Lib.Types.ConnectState connectState)
    {
        if (connectState.Equals(Com.Unitech.Lib.Types.ConnectState.Connected))
            return ConnectState.Connected;
        if (connectState.Equals(Com.Unitech.Lib.Types.ConnectState.Connecting))
            return ConnectState.Connecting;
        if (connectState.Equals(Com.Unitech.Lib.Types.ConnectState.Disconnected))
            return ConnectState.Disconnected;
        if (connectState.Equals(Com.Unitech.Lib.Types.ConnectState.Disconnecting))
            return ConnectState.Disconnecting;
        if (connectState.Equals(Com.Unitech.Lib.Types.ConnectState.Listen))
            return ConnectState.Listen;

        return ConnectState.Disconnected;
    }

    internal static BatteryState ToAppModel(this Com.Unitech.Lib.Types.BatteryState batteryState)
    {
        if (batteryState.Equals(Com.Unitech.Lib.Types.BatteryState.Unknown))
            return BatteryState.Unknown;
        if (batteryState.Equals(Com.Unitech.Lib.Types.BatteryState.Charging))
            return BatteryState.Charging;
        if (batteryState.Equals(Com.Unitech.Lib.Types.BatteryState.FullCharged))
            return BatteryState.FullCharged;
        if (batteryState.Equals(Com.Unitech.Lib.Types.BatteryState.Low))
            return BatteryState.Low;
        if (batteryState.Equals(Com.Unitech.Lib.Types.BatteryState.Normal))
            return BatteryState.Normal;
        if (batteryState.Equals(Com.Unitech.Lib.Types.BatteryState.VeryLow))
            return BatteryState.VeryLow;

        return BatteryState.Unknown;
    }

    internal static BarcodeID ToAppModel(this Com.Unitech.Lib.Types.Scanner.BarcodeID barcodeID)
    {
        // The native binding renames some members (e.g. Ean13, Gs1128, Iata,
        // Ocrb) and uses different numeric values than the app enum, so map by
        // identity against the native fields rather than by value.
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Unknown))
            return BarcodeID.Unknown;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Code39))
            return BarcodeID.Code39;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Codabar))
            return BarcodeID.Codabar;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Code128))
            return BarcodeID.Code128;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.D25))
            return BarcodeID.D25;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Iata))
            return BarcodeID.IATA;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Itf))
            return BarcodeID.ITF;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Code93))
            return BarcodeID.Code93;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Upca))
            return BarcodeID.UPCA;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Upce))
            return BarcodeID.UPCE;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Ean8))
            return BarcodeID.EAN8;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Ean13))
            return BarcodeID.EAN13;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Code11))
            return BarcodeID.Code11;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Code49))
            return BarcodeID.Code49;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Msi))
            return BarcodeID.MSI;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Gs1128))
            return BarcodeID.GS1128;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Upce1))
            return BarcodeID.UPCE1;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Pdf417))
            return BarcodeID.PDF417;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Code16K))
            return BarcodeID.Code16K;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Code39FullASCII))
            return BarcodeID.Code39FullASCII;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Upcd))
            return BarcodeID.UPCD;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Trioptic))
            return BarcodeID.Trioptic;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Bookland))
            return BarcodeID.Bookland;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CouponCode))
            return BarcodeID.CouponCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Nw7))
            return BarcodeID.NW7;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Isbt128))
            return BarcodeID.ISBT128;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.MicroPDF))
            return BarcodeID.MicroPDF;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.DataMatrix))
            return BarcodeID.DataMatrix;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.QRCode))
            return BarcodeID.QRCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.MicroPDFCCA))
            return BarcodeID.MicroPDFCCA;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.PostnetUS))
            return BarcodeID.PostnetUS;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.PlanetUS))
            return BarcodeID.PlanetUS;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Code32))
            return BarcodeID.Code32;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.ISBT128Concat))
            return BarcodeID.ISBT128Concat;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.PostalJapan))
            return BarcodeID.PostalJapan;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.PostalAustralia))
            return BarcodeID.PostalAustralia;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.PostalDutch))
            return BarcodeID.PostalDutch;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Maxicode))
            return BarcodeID.Maxicode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.PostbarCA))
            return BarcodeID.PostbarCA;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.PostalUK))
            return BarcodeID.PostalUK;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.MacroPDF417))
            return BarcodeID.MacroPDF417;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.MacroQRCode))
            return BarcodeID.MacroQRCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.MicroQRCode))
            return BarcodeID.MicroQRCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.AztecCode))
            return BarcodeID.AztecCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.AztecRuneCode))
            return BarcodeID.AztecRuneCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.FrenchLottery))
            return BarcodeID.FrenchLottery;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.GS1DataBar14))
            return BarcodeID.GS1DataBar14;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.GS1DataBarLimited))
            return BarcodeID.GS1DataBarLimited;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.GS1DataBarExpanded))
            return BarcodeID.GS1DataBarExpanded;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.FourStateUS))
            return BarcodeID.FourStateUS;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.FourStateUS4))
            return BarcodeID.FourStateUS4;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Issn))
            return BarcodeID.ISSN;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.ScanletWebcode))
            return BarcodeID.ScanletWebcode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CueCATCode))
            return BarcodeID.CueCATCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.UPCAAdd2))
            return BarcodeID.UPCAAdd2;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.UPCEAdd2))
            return BarcodeID.UPCEAdd2;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.EAN8Add2))
            return BarcodeID.EAN8Add2;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.EAN13Add2))
            return BarcodeID.EAN13Add2;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.UPCE1Add2))
            return BarcodeID.UPCE1Add2;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCAAddGS1128))
            return BarcodeID.CompositeCCAAddGS1128;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCAAddEAN13))
            return BarcodeID.CompositeCCAAddEAN13;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCAAddEAN8))
            return BarcodeID.CompositeCCAAddEAN8;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCAAddS1DataBarExpanded))
            return BarcodeID.CompositeCCAAddS1DataBarExpanded;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCAAddS1DataBarLimited))
            return BarcodeID.CompositeCCAAddS1DataBarLimited;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCAAddGS1DataBar14))
            return BarcodeID.CompositeCCAAddGS1DataBar14;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCAAddUPCA))
            return BarcodeID.CompositeCCAAddUPCA;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCAAddUPCE))
            return BarcodeID.CompositeCCAAddUPCE;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCCAddGS1128))
            return BarcodeID.CompositeCCCAddGS1128;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Tlc39))
            return BarcodeID.TLC39;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCBAddGS1128))
            return BarcodeID.CompositeCCBAddGS1128;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCBAddEAN13))
            return BarcodeID.CompositeCCBAddEAN13;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCBAddEAN8))
            return BarcodeID.CompositeCCBAddEAN8;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCBAddGS1DataBarExpanded))
            return BarcodeID.CompositeCCBAddGS1DataBarExpanded;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCBAddGS1DataBarLimited))
            return BarcodeID.CompositeCCBAddGS1DataBarLimited;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCBAddGS1DataBar14))
            return BarcodeID.CompositeCCBAddGS1DataBar14;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCBAddUPCA))
            return BarcodeID.CompositeCCBAddUPCA;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CompositeCCBAddUPCE))
            return BarcodeID.CompositeCCBAddUPCE;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Matrix2of5))
            return BarcodeID.Matrix2of5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.C2of5))
            return BarcodeID.C2of5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Korean3of5))
            return BarcodeID.Korean3of5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.UPCAAdd5))
            return BarcodeID.UPCAAdd5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.UPCEAdd5))
            return BarcodeID.UPCEAdd5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.EAN8Add5))
            return BarcodeID.EAN8Add5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.EAN13Add5))
            return BarcodeID.EAN13Add5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.UPCE1Add5))
            return BarcodeID.UPCE1Add5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.MacroMicroPDF))
            return BarcodeID.MacroMicroPDF;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Ocrb))
            return BarcodeID.OCRB;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.RSSGS1DatabarExpandedCoupon))
            return BarcodeID.RSSGS1DatabarExpandedCoupon;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.HanXin))
            return BarcodeID.HanXin;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.GS1DataMatrix))
            return BarcodeID.GS1DataMatrix;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Telepen))
            return BarcodeID.Telepen;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.NEC2of5))
            return BarcodeID.NEC2of5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Postal4i))
            return BarcodeID.Postal4i;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.Straight2of5))
            return BarcodeID.Straight2of5;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CodablockA))
            return BarcodeID.CodablockA;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.CodablockF))
            return BarcodeID.CodablockF;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.LabelCode))
            return BarcodeID.LabelCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.DotCode))
            return BarcodeID.DotCode;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.GridMatrix))
            return BarcodeID.GridMatrix;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.InfoMail))
            return BarcodeID.InfoMail;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.IntelligentMail))
            return BarcodeID.IntelligentMail;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.BritishPost))
            return BarcodeID.BritishPost;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.KoreaPost))
            return BarcodeID.KoreaPost;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.ChinaPost))
            return BarcodeID.ChinaPost;
        if (barcodeID.Equals(Com.Unitech.Lib.Types.Scanner.BarcodeID.KIXPost))
            return BarcodeID.KIXPost;

        return BarcodeID.Unknown;
    }

    internal static Com.Unitech.Lib.Types.Scanner.DeviceParamID? ToNativeModel(this DeviceParamID deviceParam)
    {
        return deviceParam switch
        {
            DeviceParamID.AutoConnecting => Com.Unitech.Lib.Types.Scanner.DeviceParamID.AutoConnecting,
            DeviceParamID.AutoPowerOff => Com.Unitech.Lib.Types.Scanner.DeviceParamID.AutoPowerOff,
            DeviceParamID.BatteryStatus => Com.Unitech.Lib.Types.Scanner.DeviceParamID.BatteryStatus,
            DeviceParamID.BeeperVolume => Com.Unitech.Lib.Types.Scanner.DeviceParamID.BeeperVolume,
            DeviceParamID.BluetoothMAC => Com.Unitech.Lib.Types.Scanner.DeviceParamID.BluetoothMAC,
            DeviceParamID.BluetoothName => Com.Unitech.Lib.Types.Scanner.DeviceParamID.BluetoothName,
            DeviceParamID.BluetoothSignalCheckingLevel => Com.Unitech.Lib.Types.Scanner.DeviceParamID.BluetoothSignalCheckingLevel,
            DeviceParamID.ControlCharacterFilter => Com.Unitech.Lib.Types.Scanner.DeviceParamID.ControlCharacterFilter,
            DeviceParamID.CradleAutoPair => Com.Unitech.Lib.Types.Scanner.DeviceParamID.CradleAutoPair,
            DeviceParamID.CradleAutoPresentationMode => Com.Unitech.Lib.Types.Scanner.DeviceParamID.CradleAutoPresentationMode,
            DeviceParamID.DataIndicator => Com.Unitech.Lib.Types.Scanner.DeviceParamID.DataIndicator,
            DeviceParamID.DataSendingInterBlockDelay => Com.Unitech.Lib.Types.Scanner.DeviceParamID.DataSendingInterBlockDelay,
            DeviceParamID.DataTerminator => Com.Unitech.Lib.Types.Scanner.DeviceParamID.DataTerminator,
            DeviceParamID.EndOfBatchSendingMessage => Com.Unitech.Lib.Types.Scanner.DeviceParamID.EndOfBatchSendingMessage,
            DeviceParamID.EngineModel => Com.Unitech.Lib.Types.Scanner.DeviceParamID.EngineModel,
            DeviceParamID.EngineVersion => Com.Unitech.Lib.Types.Scanner.DeviceParamID.EngineVersion,
            DeviceParamID.FindReplace => Com.Unitech.Lib.Types.Scanner.DeviceParamID.FindReplace,
            DeviceParamID.Firmware => Com.Unitech.Lib.Types.Scanner.DeviceParamID.Firmware,
            DeviceParamID.Gs1AI => Com.Unitech.Lib.Types.Scanner.DeviceParamID.Gs1AI,
            DeviceParamID.HidEncoding => Com.Unitech.Lib.Types.Scanner.DeviceParamID.HidEncoding,
            DeviceParamID.HidInterCharacterDelay => Com.Unitech.Lib.Types.Scanner.DeviceParamID.HidInterCharacterDelay,
            DeviceParamID.HidKeyboardCase => Com.Unitech.Lib.Types.Scanner.DeviceParamID.HidKeyboardCase,
            DeviceParamID.HidKeyboardLanguage => Com.Unitech.Lib.Types.Scanner.DeviceParamID.HidKeyboardLanguage,
            DeviceParamID.HidOutputType => Com.Unitech.Lib.Types.Scanner.DeviceParamID.HidOutputType,
            DeviceParamID.Indicator => Com.Unitech.Lib.Types.Scanner.DeviceParamID.Indicator,
            DeviceParamID.LedControl => Com.Unitech.Lib.Types.Scanner.DeviceParamID.LedControl,
            DeviceParamID.OperationMode => Com.Unitech.Lib.Types.Scanner.DeviceParamID.OperationMode,
            DeviceParamID.Reset => Com.Unitech.Lib.Types.Scanner.DeviceParamID.Reset,
            DeviceParamID.SaveBufferWhilePowerOff => Com.Unitech.Lib.Types.Scanner.DeviceParamID.SaveBufferWhilePowerOff,
            DeviceParamID.ScannerIndicator => Com.Unitech.Lib.Types.Scanner.DeviceParamID.ScannerIndicator,
            DeviceParamID.SerialNumber => Com.Unitech.Lib.Types.Scanner.DeviceParamID.SerialNumber,
            DeviceParamID.SettingByLabel => Com.Unitech.Lib.Types.Scanner.DeviceParamID.SettingByLabel,
            DeviceParamID.StartDecode => Com.Unitech.Lib.Types.Scanner.DeviceParamID.StartDecode,
            DeviceParamID.StopDecode => Com.Unitech.Lib.Types.Scanner.DeviceParamID.StopDecode,
            DeviceParamID.TriggerKeyStatus => Com.Unitech.Lib.Types.Scanner.DeviceParamID.TriggerKeyStatus,
            DeviceParamID.UnpairBluetooth => Com.Unitech.Lib.Types.Scanner.DeviceParamID.UnpairBluetooth,
            DeviceParamID.Vibrator => Com.Unitech.Lib.Types.Scanner.DeviceParamID.Vibrator,
            _ => null
        };
    }

    internal static BeeperVolume? ToNativeModel(this BeeperVolumeParam beeperVolume)
    {
        return beeperVolume switch
        {
            BeeperVolumeParam.Mute => BeeperVolume.Mute,
            BeeperVolumeParam.Low => BeeperVolume.Low,
            BeeperVolumeParam.Medium => BeeperVolume.Medium,
            BeeperVolumeParam.High => BeeperVolume.High,
            _ => BeeperVolume.High
        };
    }

    internal static DataTerminator? ToNativeModel(this DataTerminatorParam dataTerminator)
    {
        return dataTerminator switch
        {
            DataTerminatorParam.None => DataTerminator.None,
            DataTerminatorParam.CR => DataTerminator.Cr,
            DataTerminatorParam.LF => DataTerminator.Lf,
            DataTerminatorParam.CRLF => DataTerminator.Crlf,
            _ => DataTerminator.None
        };
    }
}