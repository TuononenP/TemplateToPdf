using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TemplateToPdf.Data;
using TemplateToPdf.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace TemplateToPdf.Services;

public interface ICustomHelperService
{
    Task<IEnumerable<CustomHelper>> GetAllHelpersAsync();
    Task<CustomHelper?> GetHelperAsync(int id);
    Task<CustomHelper> CreateHelperAsync(CustomHelper helper);
    Task UpdateHelperAsync(CustomHelper helper);
    Task DeleteHelperAsync(int id);
    void RegisterHelpers(IHandlebars handlebars);
}

public class CustomHelperService : ICustomHelperService
{
    private readonly TemplateDbContext _context;
    private readonly ILogger<CustomHelperService> _logger;
    private static readonly ScriptOptions ScriptOptions = ScriptOptions.Default
        .WithImports("System", "System.Linq", "System.Collections.Generic")
        .WithOptimizationLevel(Microsoft.CodeAnalysis.OptimizationLevel.Release);

    public CustomHelperService(TemplateDbContext context, ILogger<CustomHelperService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<CustomHelper>> GetAllHelpersAsync()
    {
        return await _context.CustomHelpers
            .Where(h => h.IsEnabled)
            .OrderBy(h => h.Name)
            .ToListAsync();
    }

    public async Task<CustomHelper?> GetHelperAsync(int id)
    {
        return await _context.CustomHelpers.FindAsync(id);
    }

    public async Task<CustomHelper> CreateHelperAsync(CustomHelper helper)
    {
        // Validate the helper function
        await ValidateHelperFunctionAsync(helper.FunctionBody);

        helper.CreatedAt = DateTime.UtcNow;
        helper.UpdatedAt = DateTime.UtcNow;
        
        _context.CustomHelpers.Add(helper);
        await _context.SaveChangesAsync();
        
        return helper;
    }

    public async Task UpdateHelperAsync(CustomHelper helper)
    {
        // Validate the helper function
        await ValidateHelperFunctionAsync(helper.FunctionBody);

        helper.UpdatedAt = DateTime.UtcNow;
        _context.Entry(helper).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteHelperAsync(int id)
    {
        var helper = await _context.CustomHelpers.FindAsync(id);
        if (helper != null)
        {
            _context.CustomHelpers.Remove(helper);
            await _context.SaveChangesAsync();
        }
    }

    public void RegisterHelpers(IHandlebars handlebars)
    {
        var helpers = _context.CustomHelpers
            .Where(h => h.IsEnabled)
            .AsNoTracking()
            .ToList();

        foreach (var helper in helpers)
        {
            try
            {
                _logger.LogDebug("Registering custom helper: {HelperName}", helper.Name);
                handlebars.RegisterHelper(helper.Name, (context, arguments) =>
                {
                    try
                    {
                        // Convert HandlebarsDotNet arguments to string array
                        var args = arguments.Select(arg => arg?.ToString() ?? string.Empty).ToArray();
                        
                        // Execute the helper function using Roslyn
                        var result = ExecuteHelperFunction(helper.FunctionBody, args);
                        return result?.ToString() ?? string.Empty;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing custom helper {HelperName}", helper.Name);
                        return $"Error in helper {helper.Name}: {ex.Message}";
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register custom helper {HelperName}", helper.Name);
            }
        }
    }

    private object? ExecuteHelperFunction(string functionBody, string[] args)
    {
        var code = $@"
            using System;
            using System.Linq;
            using System.Collections.Generic;
            
            public class HelperFunction
            {{
                public static object? Execute(string[] args)
                {{
                    {functionBody}
                }}
            }}";

        try
        {
            var result = CSharpScript.EvaluateAsync<object>(code + $"\nHelperFunction.Execute(new string[] {{ {string.Join(", ", args.Select(a => $"\"{a}\""))} }});").Result;
            return result;
        }
        catch (CompilationErrorException ex)
        {
            _logger.LogError(ex, "Compilation error in helper function");
            throw new ArgumentException($"Invalid helper function: {ex.Message}");
        }
    }

    private async Task ValidateHelperFunctionAsync(string functionBody)
    {
        try
        {
            // Create a delegate that takes HandlebarsValue[] as parameter
            var function = await CSharpScript.EvaluateAsync<Delegate>(
                $"new Func<object[], string>(args => {{ {functionBody} }})",
                ScriptOptions);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid helper function: {ex.Message}", nameof(functionBody));
        }
    }
} 