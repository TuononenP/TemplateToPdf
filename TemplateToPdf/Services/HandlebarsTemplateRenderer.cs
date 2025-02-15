using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using TemplateToPdf.Interfaces;
using HandlebarsDotNet.Extension.Json;
using Ganss.Xss;
using IHtmlSanitizer = TemplateToPdf.Interfaces.IHtmlSanitizer;
using TemplateToPdf.Data.Repositories;
using TemplateToPdf.Models;
using HandlebarsDotNet.Runtime;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements template rendering using Handlebars.Net
/// </summary>
public class HandlebarsTemplateRenderer : ITemplateRenderer
{
    private readonly ILogger<HandlebarsTemplateRenderer> _logger;
    private readonly IHtmlSanitizer _htmlSanitizer;
    private readonly IAssetRepository _assetRepository;
    private readonly ICustomHelperService _customHelperService;

    public HandlebarsTemplateRenderer(
        ILogger<HandlebarsTemplateRenderer> logger, 
        IHtmlSanitizer htmlSanitizer,
        IAssetRepository assetRepository,
        ICustomHelperService customHelperService)
    {
        _logger = logger;
        _htmlSanitizer = htmlSanitizer;
        _assetRepository = assetRepository;
        _customHelperService = customHelperService;
    }

    private IHandlebars CreateHandlebars(bool sanitize = true)
    {
        var handlebars = Handlebars.Create();
        // Register built-in helpers
        HandlebarsHelpers.RegisterHelpers(handlebars);

        // Register asset helpers
        RegisterAssetHelpers(handlebars);

        // Register custom helpers from database
        _customHelperService.RegisterHelpers(handlebars);

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

    private void RegisterAssetHelpers(IHandlebars handlebars)
    {
        // Helper for including CSS assets
        handlebars.RegisterHelper("css", (context, arguments) =>
        {
            if (arguments.Length == 0 || arguments[0] == null)
                return "";

            var referenceName = arguments[0]?.ToString() ?? string.Empty;
            var asset = _assetRepository.GetAssetByReferenceNameAsync(referenceName).Result;
            
            if (asset == null || asset.Type != AssetType.Css)
                return "";

            return $"<style>{asset.Content}</style>";
        });

        // Helper for including images
        handlebars.RegisterHelper("image", (context, arguments) =>
        {
            if (arguments.Length == 0 || arguments[0] == null)
                return "";

            var referenceName = arguments[0]?.ToString() ?? string.Empty;
            var asset = _assetRepository.GetAssetByReferenceNameAsync(referenceName).Result;
            
            if (asset == null || asset.Type != AssetType.Image)
                return "";

            return $"<img src=\"{asset.GetContentForDisplay()}\" alt=\"{asset.Name}\" />";
        });

        // Helper for including fonts
        handlebars.RegisterHelper("font", (context, arguments) =>
        {
            if (arguments.Length == 0 || arguments[0] == null)
                return "";

            var referenceName = arguments[0]?.ToString() ?? string.Empty;
            var asset = _assetRepository.GetAssetByReferenceNameAsync(referenceName).Result;
            
            if (asset == null || asset.Type != AssetType.Font)
                return "";

            var fontName = asset.Name.Replace(" ", "_");
            return $@"
                <style>
                    @font-face {{
                        font-family: '{fontName}';
                        src: url('{asset.Content}') format('woff2');
                    }}
                </style>";
        });

        // Register partial templates
        var partialTemplates = _assetRepository.GetAssetsByTypeAsync(AssetType.PartialTemplate).Result;
        foreach (var partial in partialTemplates)
        {
            try
            {
                if (!string.IsNullOrEmpty(partial.Content))
                {
                    _logger.LogDebug("Registering partial template: {PartialName}", partial.ReferenceName);
                    handlebars.RegisterTemplate(partial.ReferenceName, partial.Content);
                }
                else
                {
                    _logger.LogWarning("Partial template {PartialName} has no content", partial.ReferenceName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register partial template {PartialName}", partial.ReferenceName);
            }
        }
    }

    public string RenderTemplate<T>(string template, T model, bool sanitize = true)
    {
        _logger.LogDebug("Template: {Template}", template);
        _logger.LogDebug("Model: {@Model}", model);

        // Create a new Handlebars instance for each render to ensure latest helpers
        var handlebars = CreateHandlebars(sanitize);
        var compiledTemplate = handlebars.Compile(template);
        var result = compiledTemplate(model);
        
        _logger.LogDebug("Rendered result: {Result}", result);
        return result;
    }
} 