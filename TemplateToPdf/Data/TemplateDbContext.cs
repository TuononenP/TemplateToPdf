using Microsoft.EntityFrameworkCore;
using TemplateToPdf.Models;

namespace TemplateToPdf.Data;

public class TemplateDbContext : DbContext
{
    public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
    {
    }

    public DbSet<Template> Templates => Set<Template>();
} 