using TemplateToPdf.Data.Repositories;
using TemplateToPdf.Models;

namespace TemplateToPdf.Services;

public class AssetsService(IAssetRepository repository) : IAssetsService
{
    private readonly IAssetRepository _repository = repository;

    public async Task<(IEnumerable<Asset> Assets, int TotalCount)> GetAssetsAsync(
        string? name, AssetType? type, int page, int perPage, string sort, string order)
    {
        var totalCount = await _repository.GetTotalCountAsync(name, type);
        var skip = (page - 1) * perPage;
        var assets = await _repository.GetAssetsAsync(name, type, skip, perPage, sort, order);
        
        return (assets, totalCount);
    }

    public async Task<Asset?> GetAssetAsync(int id)
    {
        return await _repository.GetAssetAsync(id);
    }

    public async Task<Asset> CreateAssetAsync(Asset asset)
    {
        return await _repository.CreateAssetAsync(asset);
    }

    public async Task UpdateAssetAsync(int id, Asset asset)
    {
        if (id != asset.Id)
        {
            throw new ArgumentException("Asset ID mismatch", nameof(id));
        }

        var exists = await _repository.AssetExistsAsync(id);
        if (!exists)
        {
            throw new KeyNotFoundException($"Asset with ID {id} not found");
        }

        await _repository.UpdateAssetAsync(asset);
    }

    public async Task DeleteAssetAsync(int id)
    {
        var asset = await _repository.GetAssetAsync(id);
        if (asset == null)
        {
            throw new KeyNotFoundException($"Asset with ID {id} not found");
        }

        await _repository.DeleteAssetAsync(id);
    }

    public async Task<IEnumerable<Asset>> GetAssetsByTypeAsync(AssetType type)
    {
        return await _repository.GetAssetsByTypeAsync(type);
    }
} 