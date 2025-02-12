using Microsoft.EntityFrameworkCore;
using TemplateToPdf.Data.Repositories;
using TemplateToPdf.Models;

namespace TemplateToPdf.Services;

public class TemplatesService(ITemplateRepository repository) : ITemplatesService
{
    private readonly ITemplateRepository _repository = repository;

    public async Task<(IEnumerable<Template> Templates, int TotalCount)> GetTemplatesAsync(
        string? name, int page, int perPage, string sort, string order)
    {
        var totalCount = await _repository.GetTotalCountAsync(name);
        var skip = (page - 1) * perPage;
        var templates = await _repository.GetTemplatesAsync(name, skip, perPage, sort, order);
        
        return (templates, totalCount);
    }

    public async Task<Template?> GetTemplateAsync(int id)
    {
        return await _repository.GetTemplateAsync(id);
    }

    public async Task<Template> CreateTemplateAsync(Template template)
    {
        template.CreatedAt = DateTime.UtcNow;
        template.UpdatedAt = DateTime.UtcNow;
        return await _repository.CreateTemplateAsync(template);
    }

    public async Task UpdateTemplateAsync(int id, Template template)
    {
        if (id != template.Id)
        {
            throw new ArgumentException("Template ID mismatch", nameof(id));
        }

        var exists = await _repository.TemplateExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Template with ID {id} not found");
        }

        template.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateTemplateAsync(template);
    }

    public async Task DeleteTemplateAsync(int id)
    {
        var template = await _repository.GetTemplateAsync(id);
        if (template == null)
        {
            throw new KeyNotFoundException($"Template with ID {id} not found");
        }

        await _repository.DeleteTemplateAsync(id);
    }
} 