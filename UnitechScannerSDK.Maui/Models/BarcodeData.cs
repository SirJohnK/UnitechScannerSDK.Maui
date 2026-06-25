using System.Text;

namespace UnitechScannerSDK;

public class BarcodeData(byte[] barcodeStream, BarcodeID barcodeID)
{
    public BarcodeID Type { get; } = barcodeID;

    public byte[] RawData { get; } = barcodeStream;

    public string Barcode { get; } = Encoding.UTF8.GetString(barcodeStream);
}