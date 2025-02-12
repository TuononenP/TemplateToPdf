using TemplateToPdf.Models;

namespace TemplateToPdf.Services;

public interface ITemplatesService
{
    Task<(IEnumerable<Template> Templates, int TotalCount)> GetTemplatesAsync(string? name, int page, int perPage, string sort, string order);
    Task<Template?> GetTemplateAsync(int id);
    Task<Template> CreateTemplateAsync(Template template);
    Task UpdateTemplateAsync(int id, Template template);
    Task DeleteTemplateAsync(int id);
} 