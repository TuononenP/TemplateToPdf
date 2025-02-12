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

        // Headings and text
        sanitizer.AllowedTags.Add("h1");
        sanitizer.AllowedTags.Add("h2");
        sanitizer.AllowedTags.Add("h3");
        sanitizer.AllowedTags.Add("h4");
        sanitizer.AllowedTags.Add("h5");
        sanitizer.AllowedTags.Add("h6");
        sanitizer.AllowedTags.Add("p");
        sanitizer.AllowedTags.Add("div");
        sanitizer.AllowedTags.Add("span");
        sanitizer.AllowedTags.Add("pre");
        sanitizer.AllowedTags.Add("br");
        sanitizer.AllowedTags.Add("hr");
        sanitizer.AllowedTags.Add("b");
        sanitizer.AllowedTags.Add("strong");
        sanitizer.AllowedTags.Add("i");
        sanitizer.AllowedTags.Add("em");
        sanitizer.AllowedTags.Add("u");
        sanitizer.AllowedTags.Add("s");
        sanitizer.AllowedTags.Add("strike");
        sanitizer.AllowedTags.Add("sub");
        sanitizer.AllowedTags.Add("sup");
        sanitizer.AllowedTags.Add("small");
        sanitizer.AllowedTags.Add("big");
        sanitizer.AllowedTags.Add("tt");
        sanitizer.AllowedTags.Add("code");
        sanitizer.AllowedTags.Add("kbd");
        sanitizer.AllowedTags.Add("var");
        sanitizer.AllowedTags.Add("samp");

        // Lists
        sanitizer.AllowedTags.Add("ul");
        sanitizer.AllowedTags.Add("ol");
        sanitizer.AllowedTags.Add("li");
        sanitizer.AllowedTags.Add("dl");
        sanitizer.AllowedTags.Add("dt");
        sanitizer.AllowedTags.Add("dd");

        // Tables
        sanitizer.AllowedTags.Add("table");
        sanitizer.AllowedTags.Add("caption");
        sanitizer.AllowedTags.Add("thead");
        sanitizer.AllowedTags.Add("tbody");
        sanitizer.AllowedTags.Add("tfoot");
        sanitizer.AllowedTags.Add("tr");
        sanitizer.AllowedTags.Add("th");
        sanitizer.AllowedTags.Add("td");
        sanitizer.AllowedTags.Add("col");
        sanitizer.AllowedTags.Add("colgroup");

        // Style and metadata
        sanitizer.AllowedTags.Add("style");
        sanitizer.AllowedTags.Add("meta");

        // Global attributes
        sanitizer.AllowedAttributes.Add("id");
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("style");
        sanitizer.AllowedAttributes.Add("title");
        sanitizer.AllowedAttributes.Add("lang");
        sanitizer.AllowedAttributes.Add("dir");

        // Table attributes
        sanitizer.AllowedAttributes.Add("colspan");
        sanitizer.AllowedAttributes.Add("rowspan");
        sanitizer.AllowedAttributes.Add("headers");
        sanitizer.AllowedAttributes.Add("scope");
        sanitizer.AllowedAttributes.Add("align");
        sanitizer.AllowedAttributes.Add("valign");
        sanitizer.AllowedAttributes.Add("border");
        sanitizer.AllowedAttributes.Add("cellpadding");
        sanitizer.AllowedAttributes.Add("cellspacing");

        // Allow all CSS properties
        sanitizer.AllowedCssProperties.Add("*");

        // Allow data attributes
        sanitizer.AllowedAttributes.Add("data-*");

        // Configure to keep text content intact
        sanitizer.KeepChildNodes = true;

        return sanitizer;
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return html;
        }

        // Sanitize HTML without preserving Handlebars expressions
        return _sanitizer.Sanitize(html);
    }
} 