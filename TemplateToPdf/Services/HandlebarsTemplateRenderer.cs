using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using TemplateToPdf.Interfaces;
using HandlebarsDotNet.Extension.Json;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements template rendering using Handlebars.Net
/// </summary>
public class HandlebarsTemplateRenderer(ILogger<HandlebarsTemplateRenderer> logger) : ITemplateRenderer
{
    private readonly ILogger<HandlebarsTemplateRenderer> _logger = logger;
    private readonly IHandlebars _handlebars = CreateHandlebars();

    private static IHandlebars CreateHandlebars()
    {
        var handlebars = Handlebars.Create();
        handlebars.Configuration.UseJson();

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

    public string RenderTemplate<T>(string template, T model)
    {
        _logger.LogDebug("Template: {Template}", template);
        _logger.LogDebug("Model: {@Model}", model);

        var compiledTemplate = _handlebars.Compile(template);
        var result = compiledTemplate(model);
        
        _logger.LogDebug("Rendered result: {Result}", result);
        return result;
    }
} 