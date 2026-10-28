using System;
using QRCoder;

// This fixes Rider's warning and groups our code nicely
namespace QRCodeGenerator;

class Program
{
    static void Main(string[] args)
    {
        // The URL or text we want to turn into a QR code
        string targetUrl = "https://www.eatonville.wednet.edu";

        // Fully qualifying QRCoder.QRCodeGenerator prevents naming clashes
        QRCoder.QRCodeGenerator generator = new QRCoder.QRCodeGenerator();

        // Create the data object with our URL and error correction
        QRCodeData data = generator.CreateQrCode(
            targetUrl, 
            QRCoder.QRCodeGenerator.ECCLevel.Q);

        // Create an ASCII QR code to see it right in the terminal
        AsciiQRCode asciiQr = new AsciiQRCode(data);

        // Generate the graphic as a single formatted string
        string qrGraphic = asciiQr.GetGraphic(1);

        // Spit it out to the console
        Console.WriteLine(qrGraphic);
    }
}