using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using TemplateToPdf.Interfaces;
using HandlebarsDotNet.Extension.Json;
using Ganss.Xss;
using IHtmlSanitizer = TemplateToPdf.Interfaces.IHtmlSanitizer;
using TemplateToPdf.Data.Repositories;
using TemplateToPdf.Models;
using HandlebarsDotNet.Runtime;
using System;
using System.Linq;
using HandlebarsDotNet.StringUtils;
using System.IO;

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

    private IHandlebars CreateHandlebars()
    {
        var handlebars = Handlebars.Create(new HandlebarsConfiguration
        {
            NoEscape = true
        });

        // Enable JSON support
        handlebars.Configuration.UseJson();

        // Register built-in helpers
        HandlebarsHelpers.RegisterHelpers(handlebars);

        // Register asset helpers
        RegisterAssetHelpers(handlebars);

        // Register custom helpers from database
        _customHelperService.RegisterHelpers(handlebars);

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
        handlebars.RegisterHelper("css", (writer, context, arguments) =>
        {
            if (arguments.Length == 0 || arguments[0] == null)
                return;

            var referenceName = arguments[0]?.ToString() ?? string.Empty;
            var asset = _assetRepository.GetAssetByReferenceNameAsync(referenceName).Result;

            if (asset == null || asset.Type != AssetType.Css)
                return;

            // Inject CSS directly without :root scoping to ensure it works in PDF
            writer.WriteSafeString($@"
                <style type='text/css' data-asset='{referenceName}'>
                    {asset.Content}
                </style>");
        });

        // Helper for including images
        handlebars.RegisterHelper("image", (writer, context, arguments) =>
        {
            if (arguments.Length == 0 || arguments[0] == null)
            {
                _logger.LogWarning("No reference name provided for image helper");
                return;
            }

            var referenceName = arguments[0]?.ToString() ?? string.Empty;
            var asset = _assetRepository.GetAssetByReferenceNameAsync(referenceName).Result;

            if (asset == null || asset.Type != AssetType.Image)
            {
                _logger.LogWarning("Image not found or invalid type: {ReferenceName}", referenceName);
                return;
            }

            if (asset.BinaryContent == null)
            {
                _logger.LogWarning("Image has no binary content: {ReferenceName}", referenceName);
                return;
            }

            var base64Content = Convert.ToBase64String(asset.BinaryContent);
            writer.WriteSafeString($"<img src=\"data:{asset.MimeType};base64,{base64Content}\" alt=\"{asset.Name}\" />");
        });

        // Helper for including fonts
        handlebars.RegisterHelper("font", (writer, context, arguments) =>
        {
            if (arguments.Length == 0 || arguments[0] == null)
            {
                _logger.LogWarning("No reference name provided for font helper");
                return;
            }

            var referenceName = arguments[0]?.ToString() ?? string.Empty;
            var asset = _assetRepository.GetAssetByReferenceNameAsync(referenceName).Result;

            if (asset == null)
            {
                _logger.LogWarning("Font asset not found: {ReferenceName}", referenceName);
                return;
            }

            if (asset.Type != AssetType.Font)
            {
                _logger.LogWarning("Asset is not a font: {ReferenceName}, Type: {Type}", referenceName, asset.Type);
                return;
            }

            if (asset.BinaryContent == null)
            {
                _logger.LogWarning("Font has no binary content: {ReferenceName}", referenceName);
                return;
            }

            if (asset.MimeType == null)
            {
                _logger.LogWarning("Font has no MIME type: {ReferenceName}", referenceName);
                return;
            }

            var format = asset.MimeType.ToLowerInvariant() switch
            {
                "font/woff2" => "woff2",
                "font/woff" => "woff",
                "font/ttf" => "truetype",
                "font/otf" => "opentype",
                "application/x-font-ttf" => "truetype",
                "application/x-font-otf" => "opentype",
                _ => "woff2" // default to woff2
            };

            var base64Content = Convert.ToBase64String(asset.BinaryContent);
            writer.WriteSafeString($@"
                <style>
                    @font-face {{
                        font-family: '{asset.ReferenceName}' !important;
                        src: url('data:{asset.MimeType};base64,{base64Content}') format('{format}');
                        font-weight: normal;
                        font-style: normal;
                    }}
                </style>
            ");
            _logger.LogInformation("Font loaded successfully: {ReferenceName}, Format: {Format}", referenceName, format);
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
        // Create a new Handlebars instance for each render to ensure latest helpers
        var handlebars = CreateHandlebars();
        var compiledTemplate = handlebars.Compile(template);
        var result = compiledTemplate(model);
        
        // Only sanitize the final result if sanitize is true
        if (sanitize)
        {
            result = _htmlSanitizer.Sanitize(result);
        }
        
        return result;
    }
}