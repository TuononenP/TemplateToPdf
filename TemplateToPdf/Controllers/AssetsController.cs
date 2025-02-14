using Microsoft.AspNetCore.Mvc;
using TemplateToPdf.Models;
using TemplateToPdf.Services;

namespace TemplateToPdf.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController(IAssetsService assetsService) : ControllerBase
{
    private readonly IAssetsService _assetsService = assetsService;

    // GET: api/assets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asset>>> GetAssets(
        [FromQuery] string? name,
        [FromQuery] AssetType? type,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string sort = "updatedAt",
        [FromQuery] string order = "DESC")
    {
        var (assets, totalCount) = await _assetsService.GetAssetsAsync(name, type, page, perPage, sort, order);
        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

        return Ok(assets);
    }

    // GET: api/assets/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Asset>> GetAsset(int id)
    {
        var asset = await _assetsService.GetAssetAsync(id);
        if (asset == null)
        {
            return NotFound();
        }

        return asset;
    }

    // GET: api/assets/type/image
    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<Asset>>> GetAssetsByType(AssetType type)
    {
        var assets = await _assetsService.GetAssetsByTypeAsync(type);
        return Ok(assets);
    }

    // POST: api/assets
    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
    public async Task<ActionResult<Asset>> CreateAsset(Asset asset)
    {
        var created = await _assetsService.CreateAssetAsync(asset);
        return CreatedAtAction(nameof(GetAsset), new { id = created.Id }, created);
    }

    // PUT: api/assets/5
    [HttpPut("{id}")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
    public async Task<ActionResult<Asset>> UpdateAsset(int id, Asset asset)
    {
        try
        {
            await _assetsService.UpdateAssetAsync(id, asset);
            var updated = await _assetsService.GetAssetAsync(id);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    // DELETE: api/assets/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        try
        {
            await _assetsService.DeleteAssetAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 