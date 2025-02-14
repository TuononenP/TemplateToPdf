using Microsoft.EntityFrameworkCore;
using TemplateToPdf.Models;

namespace TemplateToPdf.Data.Repositories;

public class AssetRepository(TemplateDbContext context) : IAssetRepository
{
    private readonly TemplateDbContext _context = context;

    public async Task<IEnumerable<Asset>> GetAssetsAsync(string? name, AssetType? type, int skip, int take, string sort, string order)
    {
        var query = _context.Assets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(t => t.Name.Contains(name));
        }

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        query = sort switch
        {
            var s when s.Equals("name", StringComparison.OrdinalIgnoreCase) => 
                order.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(t => t.Name)
                    : query.OrderBy(t => t.Name),
            var s when s.Equals("type", StringComparison.OrdinalIgnoreCase) =>
                order.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(t => t.Type)
                    : query.OrderBy(t => t.Type),
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

    public async Task<int> GetTotalCountAsync(string? name, AssetType? type)
    {
        var query = _context.Assets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(t => t.Name.Contains(name));
        }

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Asset?> GetAssetAsync(int id)
    {
        return await _context.Assets.FindAsync(id);
    }

    public async Task<Asset> CreateAssetAsync(Asset asset)
    {
        asset.GenerateReferenceName();
        asset.ConvertBase64ToBinary();
        asset.CreatedAt = DateTime.UtcNow;
        asset.UpdatedAt = DateTime.UtcNow;
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task UpdateAssetAsync(Asset asset)
    {
        asset.GenerateReferenceName();
        asset.ConvertBase64ToBinary();
        asset.UpdatedAt = DateTime.UtcNow;
        _context.Entry(asset).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAssetAsync(int id)
    {
        var asset = await GetAssetAsync(id);
        if (asset != null)
        {
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> AssetExistsAsync(int id)
    {
        return await _context.Assets.AnyAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Asset>> GetAssetsByTypeAsync(AssetType type)
    {
        return await _context.Assets
            .Where(a => a.Type == type)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Asset?> GetAssetByReferenceNameAsync(string referenceName)
    {
        return await _context.Assets.FirstOrDefaultAsync(a => a.ReferenceName == referenceName);
    }

    public async Task<bool> ReferenceNameExistsAsync(string referenceName)
    {
        return await _context.Assets.AnyAsync(a => a.ReferenceName == referenceName);
    }
} 