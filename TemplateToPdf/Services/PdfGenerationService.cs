using Microsoft.Extensions.Logging;
using TemplateToPdf.Interfaces;

namespace TemplateToPdf.Services;

public class PdfGenerationService(
    IHtmlToPdfConverter pdfConverter,
    ITemplateRenderer templateRenderer,
    IHtmlSanitizer htmlSanitizer,
    ILogger<PdfGenerationService> logger) : IPdfGenerationService
{
    private readonly IHtmlToPdfConverter _pdfConverter = pdfConverter;
    private readonly ITemplateRenderer _templateRenderer = templateRenderer;
    private readonly IHtmlSanitizer _htmlSanitizer = htmlSanitizer;
    private readonly ILogger<PdfGenerationService> _logger = logger;

    public async Task<byte[]> GeneratePdfFromTemplateAsync<T>(string templateHtml, T model)
    {
        ArgumentNullException.ThrowIfNull(templateHtml);
        
        /* _logger.LogDebug("Sanitizing HTML template. Template length: {Length}", templateHtml.Length);
        var sanitizedTemplate = _htmlSanitizer.Sanitize(templateHtml);
        ArgumentNullException.ThrowIfNull(sanitizedTemplate);
        _logger.LogDebug("Sanitized template length: {Length}", sanitizedTemplate.Length);
        */
        _logger.LogDebug("Rendering HTML content with model");
        var htmlContent = _templateRenderer.RenderTemplate(templateHtml, model);
        ArgumentNullException.ThrowIfNull(htmlContent);
        _logger.LogDebug("Rendered HTML content length: {Length}", htmlContent.Length);
        
        _logger.LogDebug("Converting HTML to PDF. HTML content: {Content}", htmlContent);
        return await _pdfConverter.ConvertHtmlToPdfAsync(htmlContent);
    }
} 