using Ganss.Xss;
using IHtmlSanitizer = TemplateToPdf.Interfaces.IHtmlSanitizer;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements HTML sanitization using Ganss.Xss
/// </summary>
public partial class HtmlSanitizerWrapper : IHtmlSanitizer
{
    private readonly HtmlSanitizer _sanitizer = ConfigureHtmlSanitizer();
    private const string HandlebarsPlaceholder = "___HANDLEBARS_TEMPLATE_PLACEHOLDER_TOKEN___";
    
    [GeneratedRegex(@"\{{2,3}.*?\}{2,3}")]  // Matches both {{...}} and {{{...}}}
    private static partial Regex HandlebarsRegex();

    [GeneratedRegex("___HANDLEBARS_TEMPLATE_PLACEHOLDER_TOKEN___")]
    private static partial Regex PlaceholderRegex();

    private static HtmlSanitizer ConfigureHtmlSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        
        // Allow additional tags not allowed by default
        sanitizer.AllowedTags.Add("style");
        sanitizer.AllowedTags.Add("link");
        sanitizer.AllowedTags.Add("meta");
        sanitizer.AllowedTags.Add("title");

        // Allow data attributes and placeholders
        sanitizer.AllowedAttributes.Add("data-*");
        sanitizer.AllowedAttributes.Add(HandlebarsPlaceholder);

        return sanitizer;
    }

    public string Sanitize(string html)
    {
        // Store Handlebars expressions
        var expressions = new List<string>();
        var preservedHtml = HandlebarsRegex().Replace(html, match =>
        {
            expressions.Add(match.Value);
            return HandlebarsPlaceholder;
        });

        // Sanitize HTML
        var sanitizedHtml = _sanitizer.Sanitize(preservedHtml);

        // Restore Handlebars expressions
        var expressionIndex = 0;
        return PlaceholderRegex().Replace(sanitizedHtml, _ => 
            expressionIndex < expressions.Count ? expressions[expressionIndex++] : string.Empty);
    }
} 