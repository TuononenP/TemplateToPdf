using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using TemplateToPdf.Interfaces;
using TemplateToPdf.Models;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements PDF conversion using PuppeteerSharp (Chrome/Chromium)
/// </summary>
public class PuppeteerPdfConverter(ILogger<PuppeteerPdfConverter> logger) : IHtmlToPdfConverter, IAsyncDisposable
{
    private readonly ILogger<PuppeteerPdfConverter> _logger = logger;
    private IBrowser? _browser;
    private bool _isInitialized;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);

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

    public async Task<byte[]> ConvertHtmlToPdfAsync(string html, PageSize pageSize)
    {
        await EnsureInitializedAsync();

        if (_browser == null)
        {
            throw new InvalidOperationException("Browser not initialized");
        }

        _logger.LogDebug("Creating new page for PDF generation");
        using var page = await _browser.NewPageAsync();

        // Set viewport based on the selected page size
        var viewport = GetViewportForPageSize(pageSize);
        _logger.LogDebug("Setting viewport for {PageSize}: {Width}x{Height} pixels", 
            pageSize, viewport.Width, viewport.Height);
        await page.SetViewportAsync(viewport);

        _logger.LogDebug("Setting page content");
        await page.SetContentAsync(html);

        // Wait for any fonts to load
        await page.WaitForTimeoutAsync(1000);

        // Ensure all content and styles are loaded
        await page.EvaluateExpressionAsync(@"
            new Promise((resolve) => {
                const interval = setInterval(() => {
                    const styleSheets = document.styleSheets;
                    let loaded = true;
                    for (let i = 0; i < styleSheets.length; i++) {
                        if (!styleSheets[i].cssRules) loaded = false;
                    }
                    if (loaded) {
                        clearInterval(interval);
                        resolve();
                    }
                }, 100);
                setTimeout(resolve, 2000); // Timeout after 2s
            })
        ");

        var paperFormat = GetPaperFormat(pageSize);

        _logger.LogDebug("Generating PDF");
        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = paperFormat,
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

    private static PaperFormat GetPaperFormat(PageSize pageSize)
    {
        return pageSize switch
        {
            PageSize.A0 => PaperFormat.A0,
            PageSize.A1 => PaperFormat.A1,
            PageSize.A2 => PaperFormat.A2,
            PageSize.A3 => PaperFormat.A3,
            PageSize.A4 => PaperFormat.A4,
            PageSize.A5 => PaperFormat.A5,
            PageSize.A6 => PaperFormat.A6,
            PageSize.Letter => PaperFormat.Letter,
            PageSize.Legal => PaperFormat.Legal,
            PageSize.Tabloid => PaperFormat.Tabloid,
            _ => PaperFormat.A4,
        };
    }

    private static ViewPortOptions GetViewportForPageSize(PageSize pageSize)
    {
        // Standard DPI for high-quality rendering
        const int dpi = 150;
        const double mmToPixel = dpi / 25.4; // 25.4mm = 1 inch

        // Get dimensions in pixels based on page size (sizes in mm)
        var (widthMm, heightMm) = pageSize switch
        {
            PageSize.A0 => (841, 1189),
            PageSize.A1 => (594, 841),
            PageSize.A2 => (420, 594),
            PageSize.A3 => (297, 420),
            PageSize.A4 => (210, 297),
            PageSize.A5 => (148, 210),
            PageSize.A6 => (105, 148),
            PageSize.Letter => (215.9, 279.4),
            PageSize.Legal => (215.9, 355.6),
            PageSize.Tabloid => (279.4, 431.8),
            _ => (210, 297) // Default to A4
        };

        return new ViewPortOptions
        {
            Width = (int)(widthMm * mmToPixel),
            Height = (int)(heightMm * mmToPixel),
            DeviceScaleFactor = 1.5  // Higher scale factor for sharper text
        };
    }
} 