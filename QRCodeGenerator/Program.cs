using System;
using System.IO; 
using QRCoder;

namespace QRCodeGenerator;

class Program
{
    static void Main(string[] args)
    {
        // The URL for the QR code
        string targetUrl = "https://drive.google.com/your-drive-link-here";

        QRCoder.QRCodeGenerator generator = new QRCoder.QRCodeGenerator();

        QRCodeData data = generator.CreateQrCode(
            targetUrl, 
            QRCoder.QRCodeGenerator.ECCLevel.Q);

        PngByteQRCode pngQr = new PngByteQRCode(data);
        byte[] qrCodeBytes = pngQr.GetGraphic(20);

        // Get the absolute path to your Mac's Desktop
        string desktopPath = Environment.GetFolderPath(
            Environment.SpecialFolder.Desktop);

        // Add a timestamp so the filename is always 100% unique
        string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"YourFileName_{timeStamp}.png";
        
        string filePath = Path.Combine(desktopPath, fileName);

        // FileMode.CreateNew guarantees we never follow a planted symlink 
        // or overwrite an existing file by accident.
        using (FileStream stream = new FileStream(
                   filePath, 
                   FileMode.CreateNew, 
                   FileAccess.Write))
        {
            stream.Write(qrCodeBytes, 0, qrCodeBytes.Length);
        }

        Console.WriteLine("Hell yeah. Secured QR Code saved at: " + filePath);
        
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}