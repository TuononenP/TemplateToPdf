using TemplateToPdf.Models;

namespace TemplateToPdf.Data.Repositories;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetAssetsAsync(string? name, AssetType? type, int skip, int take, string sort, string order);
    Task<int> GetTotalCountAsync(string? name, AssetType? type);
    Task<Asset?> GetAssetAsync(int id);
    Task<Asset?> GetAssetByReferenceNameAsync(string referenceName);
    Task<Asset> CreateAssetAsync(Asset asset);
    Task UpdateAssetAsync(Asset asset);
    Task DeleteAssetAsync(int id);
    Task<bool> AssetExistsAsync(int id);
    Task<bool> ReferenceNameExistsAsync(string referenceName);
    Task<IEnumerable<Asset>> GetAssetsByTypeAsync(AssetType type);
} 