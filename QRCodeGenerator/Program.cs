using System;
using System.IO; 
using QRCoder;

namespace QRCodeGenerator;

class Program
{
    static void Main(string[] args)
    {
        // The URL for the QR code
        string targetUrl = "https://drive.google.com/file/d/1V3SoUEjjDZzhtUVXqT4OJmKCp0ZFRMPD/view?usp=sharing";

        // Initialize the generator
        QRCoder.QRCodeGenerator generator = new QRCoder.QRCodeGenerator();

        // Create the QR data object
        QRCodeData data = generator.CreateQrCode(
            targetUrl, 
            QRCoder.QRCodeGenerator.ECCLevel.Q);

        // Swap out ASCII for a PNG byte array generator
        PngByteQRCode pngQr = new PngByteQRCode(data);

        // Generate the image as bytes (20 pixels per block makes it crisp)
        byte[] qrCodeBytes = pngQr.GetGraphic(20);

        // Define the file name
        string filePath = "FootballSeniorNight.png";

        // Write the raw bytes directly to a PNG file
        File.WriteAllBytes(filePath, qrCodeBytes);

        // Let ourselves know where the file went and that it worked
        Console.WriteLine("Hell yeah. QR Code saved as: " + filePath);
        
        // Pause so the console doesn't instantly close
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}