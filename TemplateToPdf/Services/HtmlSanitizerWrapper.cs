using Ganss.Xss;
using IHtmlSanitizer = TemplateToPdf.Interfaces.IHtmlSanitizer;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements HTML sanitization using Ganss.Xss
/// </summary>
public partial class HtmlSanitizerWrapper : IHtmlSanitizer
{
    private readonly HtmlSanitizer _sanitizer = ConfigureHtmlSanitizer();

    private static HtmlSanitizer ConfigureHtmlSanitizer()
    {
        var sanitizer = new HtmlSanitizer();

        // Clear default allowed tags and attributes
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedCssProperties.Clear();
        sanitizer.AllowedSchemes.Clear();
        
        // Allow data URLs and other schemes
        sanitizer.AllowedSchemes.Add("data");
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("https");
        
        // Document structure
        sanitizer.AllowedTags.Add("html");
        sanitizer.AllowedTags.Add("head");
        sanitizer.AllowedTags.Add("body");
        sanitizer.AllowedTags.Add("main");
        sanitizer.AllowedTags.Add("header");
        sanitizer.AllowedTags.Add("footer");
        sanitizer.AllowedTags.Add("nav");
        sanitizer.AllowedTags.Add("article");
        sanitizer.AllowedTags.Add("section");
        sanitizer.AllowedTags.Add("aside");
        sanitizer.AllowedTags.Add("style");
        sanitizer.AllowedTags.Add("meta");
        sanitizer.AllowedTags.Add("img");

        // Text formatting
        sanitizer.AllowedTags.Add("h1");
        sanitizer.AllowedTags.Add("h2");
        sanitizer.AllowedTags.Add("h3");
        sanitizer.AllowedTags.Add("h4");
        sanitizer.AllowedTags.Add("h5");
        sanitizer.AllowedTags.Add("h6");
        sanitizer.AllowedTags.Add("p");
        sanitizer.AllowedTags.Add("div");
        sanitizer.AllowedTags.Add("span");
        sanitizer.AllowedTags.Add("br");
        sanitizer.AllowedTags.Add("hr");
        sanitizer.AllowedTags.Add("strong");
        sanitizer.AllowedTags.Add("em");
        sanitizer.AllowedTags.Add("u");
        sanitizer.AllowedTags.Add("s");

        // Tables
        sanitizer.AllowedTags.Add("table");
        sanitizer.AllowedTags.Add("thead");
        sanitizer.AllowedTags.Add("tbody");
        sanitizer.AllowedTags.Add("tfoot");
        sanitizer.AllowedTags.Add("tr");
        sanitizer.AllowedTags.Add("th");
        sanitizer.AllowedTags.Add("td");

        // Global attributes
        sanitizer.AllowedAttributes.Add("id");
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("style");
        sanitizer.AllowedAttributes.Add("title");
        sanitizer.AllowedAttributes.Add("lang");
        sanitizer.AllowedAttributes.Add("dir");
        sanitizer.AllowedAttributes.Add("http-equiv");
        sanitizer.AllowedAttributes.Add("content");

        // Table attributes
        sanitizer.AllowedAttributes.Add("colspan");
        sanitizer.AllowedAttributes.Add("rowspan");
        sanitizer.AllowedAttributes.Add("align");
        sanitizer.AllowedAttributes.Add("valign");
        sanitizer.AllowedAttributes.Add("border");
        sanitizer.AllowedAttributes.Add("cellpadding");
        sanitizer.AllowedAttributes.Add("cellspacing");

        // Image attributes
        sanitizer.AllowedAttributes.Add("src");
        sanitizer.AllowedAttributes.Add("alt");
        sanitizer.AllowedAttributes.Add("width");
        sanitizer.AllowedAttributes.Add("height");

        // Allow all CSS properties
        var commonCssProperties = new[]
        {
            // Layout
            "display", "position", "top", "right", "bottom", "left", "float", "clear",
            "visibility", "opacity", "z-index", "overflow", "clip",
            
            // Box model
            "margin", "margin-top", "margin-right", "margin-bottom", "margin-left",
            "padding", "padding-top", "padding-right", "padding-bottom", "padding-left",
            "width", "min-width", "max-width", "height", "min-height", "max-height",
            "box-sizing", "box-shadow",
            
            // Typography
            "font", "font-family", "font-size", "font-weight", "font-style",
            "text-align", "text-decoration", "text-transform", "text-indent",
            "line-height", "letter-spacing", "word-spacing", "white-space",
            "color", "background", "background-color", "background-image",
            "text-shadow",
            
            // Borders
            "border", "border-color", "border-style", "border-width",
            "border-top", "border-right", "border-bottom", "border-left",
            "border-radius", "border-collapse", "border-spacing",
            
            // Tables
            "table-layout", "caption-side", "empty-cells",
            
            // Lists
            "list-style", "list-style-type", "list-style-position", "list-style-image",
            
            // Images
            "max-width", "max-height", "object-fit", "object-position",
            
            // Flexbox
            "flex", "flex-basis", "flex-direction", "flex-flow", "flex-grow",
            "flex-shrink", "flex-wrap", "align-items", "align-content",
            "justify-content", "gap",
            
            // Grid
            "grid", "grid-template-columns", "grid-template-rows", "grid-gap",
            "grid-column", "grid-row",
            
            // Transforms
            "transform", "transform-origin", "rotate", "scale",
            
            // Transitions
            "transition", "transition-property", "transition-duration",
            "transition-timing-function", "transition-delay",
            
            // Others
            "cursor", "pointer-events", "user-select"
        };

        foreach (var property in commonCssProperties)
        {
            sanitizer.AllowedCssProperties.Add(property);
        }

        // Allow all CSS values
        sanitizer.KeepChildNodes = true;
        sanitizer.UriAttributes.Clear();  // Don't restrict URI attributes
        
        return sanitizer;
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return html;
        }

        return _sanitizer.Sanitize(html);
    }
} 