using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media; // We need this to change text colors
using System;
using System.IO;
using QRCoder;

namespace HeathersQr.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Wire up the button click event to our custom method below
        GenerateButton.Click += GenerateButton_Click;
    }

    private void GenerateButton_Click(object? sender, RoutedEventArgs e)
    {
        // Get the URL from the text box
        string targetUrl = UrlTextBox.Text ?? "";

        // If she left it blank, give a friendly warning and stop
        if (string.IsNullOrWhiteSpace(targetUrl))
        {
            StatusText.Text = "Hold up! You need to enter a URL first.";
            StatusText.Foreground = Brushes.DarkMagenta;
            return;
        }

        try
        {
            // Generate the QR Code logic we already perfected
            QRCoder.QRCodeGenerator gen = new QRCoder.QRCodeGenerator();
            QRCodeData data = gen.CreateQrCode(
                targetUrl, 
                QRCoder.QRCodeGenerator.ECCLevel.Q);

            PngByteQRCode pngQr = new PngByteQRCode(data);
            byte[] qrCodeBytes = pngQr.GetGraphic(20);

            // Get the Desktop path and make a unique filename
            string desktopPath = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop);
            
            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"HeathersQr_{timeStamp}.png";
            
            string filePath = Path.Combine(desktopPath, fileName);

            // Write the file securely so the Sourcery bot stays happy
            using (FileStream stream = new FileStream(
                filePath, 
                FileMode.CreateNew, 
                FileAccess.Write))
            {
                stream.Write(qrCodeBytes, 0, qrCodeBytes.Length);
            }

            // Tell the UI it worked perfectly
            StatusText.Text = $"Success! Saved to Desktop: {fileName}";
            StatusText.Foreground = Brushes.LimeGreen;
        }
        catch (Exception ex)
        {
            // If something goes to shit, print the error to the screen
            StatusText.Text = "Damn it, something broke: " + ex.Message;
            StatusText.Foreground = Brushes.Purple;
        }
    }
}