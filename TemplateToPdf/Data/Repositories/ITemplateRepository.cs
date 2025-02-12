using TemplateToPdf.Models;

namespace TemplateToPdf.Data.Repositories;

public interface ITemplateRepository
{
    Task<IEnumerable<Template>> GetTemplatesAsync(string? name, int skip, int take, string sort, string order);
    Task<int> GetTotalCountAsync(string? name);
    Task<Template?> GetTemplateAsync(int id);
    Task<Template> CreateTemplateAsync(Template template);
    Task UpdateTemplateAsync(Template template);
    Task DeleteTemplateAsync(int id);
    Task<bool> TemplateExistsAsync(int id);
} 