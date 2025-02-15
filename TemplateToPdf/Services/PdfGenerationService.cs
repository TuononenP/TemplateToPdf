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

    // Allowed HTML tags with their common attributes
    private static readonly string[] AllowedHtmlTags = 
    [
        // Document structure
        "html", "head", "body", "main", "header", "footer", "nav", "article", "section", "aside",

        // Headings and text
        "h1", "h2", "h3", "h4", "h5", "h6",
        "p", "div", "span", "pre", "br", "hr",
        "b", "strong", "i", "em", "u", "s", "strike",
        "sub", "sup", "small", "big", "tt", "code", "kbd", "var", "samp",
        "abbr", "acronym", "cite", "q", "blockquote", "address",
        "bdi", "mark", "ruby", "rt", "rp", "time", "wbr",

        // Lists
        "ul", "ol", "li", "dl", "dt", "dd", "dir", "menu", "menuitem",

        // Tables
        "table", "caption", "thead", "tbody", "tfoot",
        "tr", "th", "td", "col", "colgroup",

        // Forms and input
        "form", "input", "button", "select", "optgroup", "option",
        "textarea", "label", "fieldset", "legend", "datalist", "output",
        "progress", "meter", "keygen",

        // Media and content
        "img", "figure", "figcaption", "picture", "source",
        "map", "area",

        // Interactive elements
        "details", "summary",

        // Formatting and style
        "font", "center", "del", "ins",

        // Links
        "a",

        // Style and metadata
        "style", "link", "meta"
    ];

    // Common attributes that should be allowed
    private static readonly string[] AllowedAttributes =
    [
        // Global attributes
        "id", "class", "style", "title", "lang", "dir",
        "tabindex", "accesskey", "hidden", "translate",

        // Link and image attributes
        "href", "src", "alt", "target", "rel", "download",
        "width", "height", "border", "align", "valign",

        // Table attributes
        "colspan", "rowspan", "headers", "scope",
        "cellpadding", "cellspacing",

        // Form attributes
        "type", "name", "value", "placeholder", "required",
        "disabled", "readonly", "maxlength", "minlength",
        "min", "max", "pattern", "accept", "autocomplete",
        "autofocus", "form", "list", "multiple", "selected",
        "size", "step",

        // Media attributes
        "controls", "autoplay", "loop", "muted", "poster",
        "preload", "crossorigin",

        // Data attributes
        "data-*"
    ];

    [GeneratedRegex(@"<!\[CDATA\[.*?\]\]>|<!--.*?-->|<\?php.*?\?>|<%.*?%>", RegexOptions.Singleline)]
    private static partial Regex DangerousContentRegex();

    [GeneratedRegex(@"data:.*?base64", RegexOptions.IgnoreCase)]
    private static partial Regex Base64DataRegex();

    [GeneratedRegex(@"<(\/?)([\w-]+).*?>", RegexOptions.Singleline)]
    private static partial Regex HtmlTagRegex();

    private string SanitizeAndValidateHtml(string html, string source, bool validateTags = true)
    {
        _logger.LogDebug("Sanitizing {Source} HTML. Length: {Length}", source, html.Length);
        _logger.LogDebug("Sanitizing HTML: {Html}", html);
        var sanitized = _htmlSanitizer.Sanitize(html);
        _logger.LogDebug("Sanitized HTML: {Sanitized}", sanitized);
        ArgumentNullException.ThrowIfNull(sanitized);
        
        _logger.LogDebug("Sanitized {Source} length: {Length}", source, sanitized.Length);

        if (validateTags)
        {
            ValidateRemovedTags(html, sanitized, source);
        }

        return sanitized;
    }

    private void ValidateRemovedTags(string originalHtml, string sanitizedHtml, string source)
    {
        var originalTags = HtmlTagRegex().Matches(originalHtml)
            .Select(m => m.Groups[2].Value.ToLowerInvariant())
            .Distinct()
            .ToHashSet();

        var sanitizedTags = HtmlTagRegex().Matches(sanitizedHtml)
            .Select(m => m.Groups[2].Value.ToLowerInvariant())
            .Distinct()
            .ToHashSet();

        var removedTags = originalTags.Except(sanitizedTags)
            .Where(tag => AllowedHtmlTags.Contains(tag));

        foreach (var tag in removedTags)
        {
            _logger.LogWarning("HTML tag unexpectedly removed from {Source}: {Tag}", source, tag);
        }
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