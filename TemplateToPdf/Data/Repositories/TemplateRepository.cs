using Microsoft.EntityFrameworkCore;
using TemplateToPdf.Models;

namespace TemplateToPdf.Data.Repositories;

public class TemplateRepository(TemplateDbContext context) : ITemplateRepository
{
    private readonly TemplateDbContext _context = context;

    public async Task<IEnumerable<Template>> GetTemplatesAsync(string? name, int skip, int take, string sort, string order)
    {
        var query = _context.Templates.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(t => t.Name.Contains(name));
        }

        query = sort switch
        {
            var s when s.Equals("name", StringComparison.OrdinalIgnoreCase) => 
                order.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(t => t.Name)
                    : query.OrderBy(t => t.Name),
            var s when s.Equals("createdat", StringComparison.OrdinalIgnoreCase) =>
                order.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),
            _ => order.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(t => t.UpdatedAt)
                : query.OrderBy(t => t.UpdatedAt)
        };

        return await query.Skip(skip).Take(take).ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(string? name)
    {
        var query = _context.Templates.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(t => t.Name.Contains(name));
        }

        return await query.CountAsync();
    }

    public async Task<Template?> GetTemplateAsync(int id)
    {
        return await _context.Templates.FindAsync(id);
    }

    public async Task<Template> CreateTemplateAsync(Template template)
    {
        _context.Templates.Add(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task UpdateTemplateAsync(Template template)
    {
        _context.Entry(template).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTemplateAsync(int id)
    {
        var template = await GetTemplateAsync(id);
        if (template != null)
        {
            _context.Templates.Remove(template);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> TemplateExistsAsync(int id)
    {
        return await _context.Templates.AnyAsync(t => t.Id == id);
    }
} 