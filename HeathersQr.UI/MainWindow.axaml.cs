using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media; 
using System;
using System.IO;
using QRCoder;

namespace HeathersQr.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        GenerateButton.Click += GenerateButton_Click;
    }

    private void GenerateButton_Click(object? sender, RoutedEventArgs e)
    {
        string targetUrl = UrlTextBox.Text ?? "";

        // Validate input is an actual usable HTTP/HTTPS URL
        bool isValidUrl = Uri.TryCreate(
            targetUrl, 
            UriKind.Absolute, 
            out Uri? uriResult) 
            && (uriResult.Scheme == Uri.UriSchemeHttp 
            || uriResult.Scheme == Uri.UriSchemeHttps);

        if (!isValidUrl)
        {
            StatusText.Text = "Hold up! Enter a valid HTTP or HTTPS URL.";
            StatusText.Foreground = Brushes.Orange;
            return;
        }

        // Check that the Desktop path is not completely empty
        string desktopPath = Environment.GetFolderPath(
            Environment.SpecialFolder.Desktop);

        if (string.IsNullOrWhiteSpace(desktopPath))
        {
            StatusText.Text = "Error: Cannot locate your Desktop folder.";
            StatusText.Foreground = Brushes.Red;
            return;
        }

        // Ensure Desktop directory actually exists before writing
        if (!Directory.Exists(desktopPath))
        {
            Directory.CreateDirectory(desktopPath);
        }

        // Add milliseconds (_fff) to prevent same-second crashes
        string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string fileName = $"HeathersQr_{timeStamp}.png";
        
        string filePath = Path.Combine(desktopPath, fileName);

        try
        {
            QRCoder.QRCodeGenerator gen = new QRCoder.QRCodeGenerator();
            QRCodeData data = gen.CreateQrCode(
                targetUrl, 
                QRCoder.QRCodeGenerator.ECCLevel.Q);

            PngByteQRCode pngQr = new PngByteQRCode(data);
            byte[] qrCodeBytes = pngQr.GetGraphic(20);

            using (FileStream stream = new FileStream(
                filePath, 
                FileMode.CreateNew, 
                FileAccess.Write))
            {
                stream.Write(qrCodeBytes, 0, qrCodeBytes.Length);
            }

            StatusText.Text = $"Success! Saved to Desktop: {fileName}";
            StatusText.Foreground = Brushes.LightGreen;
        }
        catch (Exception ex)
        {
            // Delete the incomplete file if the write process fails
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            StatusText.Text = "Damn it, something broke: " + ex.Message;
            StatusText.Foreground = Brushes.Red;
        }
    }
}