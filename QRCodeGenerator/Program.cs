using System;
using System.IO; 
using QRCoder;

namespace QRCodeGenerator;

class Program
{
    static void Main(string[] args)
    {
        // The URL for the QR code
        string targetUrl = "https://drive.google.com/file/d/14DPTsqgTKO258yBwhklDh2P1wSZdJmrH/view?usp=sharing";

        QRCoder.QRCodeGenerator generator = new QRCoder.QRCodeGenerator();

        QRCodeData data = generator.CreateQrCode(
            targetUrl, 
            QRCoder.QRCodeGenerator.ECCLevel.Q);

        PngByteQRCode pngQr = new PngByteQRCode(data);
        byte[] qrCodeBytes = pngQr.GetGraphic(20);

        // Get the absolute path to your Mac's Desktop
        string desktopPath = Environment.GetFolderPath(
            Environment.SpecialFolder.Desktop);

        // Combine the Desktop path with our desired file name safely
        string filePath = Path.Combine(
            desktopPath, 
            "FootballSeniorNight.png");

        // Write the raw bytes directly to the absolute path
        File.WriteAllBytes(filePath, qrCodeBytes);

        // Print the full absolute path so we know exactly where it went
        Console.WriteLine("Hell yeah. QR Code saved at: " + filePath);
        
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}