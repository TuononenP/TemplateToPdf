using TemplateToPdf.Models;

namespace TemplateToPdf.Interfaces;

public interface IPdfGenerationService
{
    Task<byte[]> GeneratePdfFromTemplateAsync<T>(string templateHtml, T model, Configuration options);
} 