using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using TemplateToPdf.Interfaces;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements PDF conversion using PuppeteerSharp (Chrome/Chromium)
/// </summary>
public class PuppeteerPdfConverter : IHtmlToPdfConverter, IAsyncDisposable
{
    private readonly ILogger<PuppeteerPdfConverter> _logger;
    private IBrowser? _browser;
    private bool _isInitialized;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);

    public PuppeteerPdfConverter(ILogger<PuppeteerPdfConverter> logger)
    {
        _logger = logger;
    }

    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        await _initializationLock.WaitAsync();
        try
        {
            if (_isInitialized) return;

            _logger.LogInformation("Downloading Chrome browser");
            await new BrowserFetcher().DownloadAsync();

            _logger.LogInformation("Launching Chrome browser");
            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args =
                [
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-dev-shm-usage"
                ]
            });

            _isInitialized = true;
            _logger.LogInformation("Chrome browser initialized successfully");
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async Task<byte[]> ConvertHtmlToPdfAsync(string html)
    {
        await EnsureInitializedAsync();

        if (_browser == null)
        {
            throw new InvalidOperationException("Browser not initialized");
        }

        _logger.LogDebug("Creating new page for PDF generation");
        using var page = await _browser.NewPageAsync();

        _logger.LogDebug("Setting page content");
        await page.SetContentAsync(html);

        // Wait a bit to ensure all content is rendered
        await page.WaitForTimeoutAsync(2000);

        _logger.LogDebug("Generating PDF");
        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            PreferCSSPageSize = true,
            MarginOptions = new MarginOptions
            {
                Top = "1cm",
                Bottom = "1cm",
                Left = "1cm",
                Right = "1cm"
            }
        });

        _logger.LogDebug("PDF generated successfully, size: {Size} bytes", pdfBytes.Length);
        return pdfBytes;
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser != null)
        {
            _logger.LogInformation("Disposing Chrome browser");
            await _browser.DisposeAsync();
        }

        _initializationLock.Dispose();
        GC.SuppressFinalize(this);
    }
} 