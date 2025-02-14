using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using TemplateToPdf.Interfaces;
using HandlebarsDotNet.Extension.Json;
using Ganss.Xss;
using IHtmlSanitizer = TemplateToPdf.Interfaces.IHtmlSanitizer;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements template rendering using Handlebars.Net
/// </summary>
public class HandlebarsTemplateRenderer : ITemplateRenderer
{
    private readonly ILogger<HandlebarsTemplateRenderer> _logger;
    private readonly IHtmlSanitizer _htmlSanitizer;
    private readonly IHandlebars _handlebars;

    public HandlebarsTemplateRenderer(ILogger<HandlebarsTemplateRenderer> logger, IHtmlSanitizer htmlSanitizer)
    {
        _logger = logger;
        _htmlSanitizer = htmlSanitizer;
        _handlebars = CreateHandlebars();
    }

    private IHandlebars CreateHandlebars(bool sanitize = true)
    {
        var handlebars = Handlebars.Create();
        handlebars.Configuration.UseJson();

        // Register all custom helpers
        HandlebarsHelpers.RegisterHelpers(handlebars);

        if (sanitize)
        {
            // Register a global helper that will be automatically applied to all variables
            handlebars.RegisterHelper("*", (context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                    return string.Empty;
                    
                var value = arguments[0].ToString();
                return value == null ? string.Empty : _htmlSanitizer.Sanitize(value);
            });
        }

        // Helper for safely rendering HTML content
        handlebars.RegisterHelper("safeHtml", (context, args) =>
        {
            if (args.Length == 0 || args[0] == null)
                return "";
                
            var input = args[0].ToString();
            return input == null ? string.Empty : _htmlSanitizer.Sanitize(input);
        });

        // Helper for rendering URLs safely
        handlebars.RegisterHelper("safeUrl", (context, args) =>
        {
            if (args.Length == 0 || args[0] == null)
                return "";

            var url = args[0].ToString();
            return url == null ? string.Empty : Uri.EscapeDataString(url);
        });

        // Helper for safe number formatting
        handlebars.RegisterHelper("formatNumber", (context, args) =>
        {
            if (args.Length == 0 || args[0] == null)
                return "0";

            if (decimal.TryParse(args[0].ToString(), out decimal number))
            {
                return number.ToString("N2");
            }
            return "0";
        });

        handlebars.RegisterHelper("formatDate", (context, arguments) =>
        {
            if (arguments.Length != 2)
            {
                return "";
            }

            if (arguments[0] is DateTime date && arguments[1] is string format)
            {
                return date.ToString(format);
            }

            return "";
        });

        return handlebars;
    }

    public string RenderTemplate<T>(string template, T model, bool sanitize = true)
    {
        _logger.LogDebug("Template: {Template}", template);
        _logger.LogDebug("Model: {@Model}", model);

        var handlebars = CreateHandlebars(sanitize);
        var compiledTemplate = handlebars.Compile(template);
        var result = compiledTemplate(model);
        
        _logger.LogDebug("Rendered result: {Result}", result);
        return result;
    }
} 