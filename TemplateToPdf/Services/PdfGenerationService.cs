using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using TemplateToPdf.Interfaces;
using TemplateToPdf.Models;

namespace TemplateToPdf.Services;

public partial class PdfGenerationService(
    IHtmlToPdfConverter pdfConverter,
    ITemplateRenderer templateRenderer,
    IHtmlSanitizer htmlSanitizer,
    ILogger<PdfGenerationService> logger) : IPdfGenerationService
{
    private readonly IHtmlToPdfConverter _pdfConverter = pdfConverter;
    private readonly ITemplateRenderer _templateRenderer = templateRenderer;
    private readonly IHtmlSanitizer _htmlSanitizer = htmlSanitizer;
    private readonly ILogger<PdfGenerationService> _logger = logger;

    // Security limits
    private const int MaxTemplateSizeBytes = 5 * 1024 * 1024; // 5MB
    private const int MaxRenderedSizeBytes = 10 * 1024 * 1024; // 10MB

    [GeneratedRegex(@"<!\[CDATA\[.*?\]\]>|<!--.*?-->|<\?php.*?\?>|<%.*?%>", RegexOptions.Singleline)]
    private static partial Regex DangerousContentRegex();

    [GeneratedRegex(@"data:.*?base64", RegexOptions.IgnoreCase)]
    private static partial Regex Base64DataRegex();

    private string SanitizeAndValidateHtml(string html, string source)
    {
        _logger.LogDebug("Sanitizing {Source} HTML. Length: {Length}", source, html.Length);
        var sanitized = _htmlSanitizer.Sanitize(html);
        ArgumentNullException.ThrowIfNull(sanitized);
        
        _logger.LogDebug("Sanitized {Source} length: {Length}", source, sanitized.Length);

        return sanitized;
    }

    public async Task<byte[]> GeneratePdfFromTemplateAsync<T>(string templateHtml, T model, PdfConfiguration options)
    {
        try
        {
            // Input validation
            ArgumentNullException.ThrowIfNull(templateHtml);
            if (templateHtml.Length > MaxTemplateSizeBytes)
            {
                throw new ArgumentException($"Template size exceeds maximum allowed size of {MaxTemplateSizeBytes / 1024 / 1024}MB");
            }

            // Check for dangerous content patterns
            if (options.Sanitize && DangerousContentRegex().IsMatch(templateHtml))
            {
                _logger.LogWarning("Potentially dangerous content detected in template");
                throw new ArgumentException("Template contains potentially dangerous content");
            }

            // Check for base64 data URIs
            if (options.Sanitize && Base64DataRegex().IsMatch(templateHtml))
            {
                _logger.LogWarning("Base64 data URI detected in template");
                throw new ArgumentException("Template contains base64 data URIs which are not allowed");
            }

            // Render the template with the model
            _logger.LogDebug("Rendering HTML content with model");
            var htmlContent = _templateRenderer.RenderTemplate(templateHtml, model, options.Sanitize);
            ArgumentNullException.ThrowIfNull(htmlContent);

            // Sanitize the html to prevent malicious code execution
            htmlContent = options.Sanitize ? SanitizeAndValidateHtml(htmlContent, "template") : htmlContent;

            if (htmlContent.Length > MaxRenderedSizeBytes)
            {
                throw new ArgumentException($"Rendered HTML size exceeds maximum allowed size of {MaxRenderedSizeBytes / 1024 / 1024}MB");
            }

            // Add Content Security Policy meta tag with more permissive rules for HTML features
            var cspHeader = """
                <meta http-equiv="Content-Security-Policy" content="
                    default-src 'self';
                    img-src 'self' https: data:;
                    style-src 'self' 'unsafe-inline';
                    font-src 'self' https:;
                    form-action 'none';
                    frame-ancestors 'none';
                    base-uri 'none';
                    object-src 'none';">
                """;

            htmlContent = htmlContent.Replace("</head>", $"{cspHeader}</head>");

            // Ensure proper HTML structure
            if (!htmlContent.Contains("<html>"))
            {
                htmlContent = $"<html><head>{cspHeader}</head><body>{htmlContent}</body></html>";
            }

            _logger.LogDebug("Converting HTML to PDF. HTML content: {Content}", htmlContent);
            var pdfBytes = await _pdfConverter.ConvertHtmlToPdfAsync(htmlContent, options.PageSize);

            // Validate PDF output
            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                throw new InvalidOperationException("PDF generation failed - no content generated");
            }

            return pdfBytes;
        }
        catch (Exception ex) when (ex is not ArgumentNullException and not ArgumentException)
        {
            _logger.LogError(ex, "PDF generation failed");
            throw new InvalidOperationException("PDF generation failed", ex);
        }
    }
} 