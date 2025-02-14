using TemplateToPdf.Models;

namespace TemplateToPdf.Services;

public interface IAssetsService
{
    Task<(IEnumerable<Asset> Assets, int TotalCount)> GetAssetsAsync(string? name, AssetType? type, int page, int perPage, string sort, string order);
    Task<Asset?> GetAssetAsync(int id);
    Task<Asset> CreateAssetAsync(Asset asset);
    Task UpdateAssetAsync(int id, Asset asset);
    Task DeleteAssetAsync(int id);
    Task<IEnumerable<Asset>> GetAssetsByTypeAsync(AssetType type);
} 