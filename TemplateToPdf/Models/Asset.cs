using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TemplateToPdf.Models;

public enum AssetType
{
    Image,
    Css,
    Font,
    PartialTemplate
}

public partial class Asset
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = default!;
    
    [Required]
    [RegularExpression(@"^[a-z0-9-_]+$", ErrorMessage = "Reference name can only contain lowercase letters, numbers, hyphens and underscores")]
    public string ReferenceName { get; set; } = default!;
    
    // Content for text-based assets (CSS, HTML, etc.)
    public string? Content { get; set; }

    // Binary content for images and fonts
    public byte[]? BinaryContent { get; set; }
    
    [Required]
    public AssetType Type { get; set; }
    
    // MimeType is only required for images (e.g., image/jpeg, image/png)
    public string? MimeType { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    public string GetContentForDisplay()
    {
        if (Type == AssetType.Image && BinaryContent != null)
        {
            return $"data:{MimeType};base64,{Convert.ToBase64String(BinaryContent)}";
        }
        return Content ?? string.Empty;
    }

    public void GenerateReferenceName()
    {
        if (string.IsNullOrEmpty(ReferenceName))
        {
            // Convert name to lowercase, replace spaces with hyphens, remove special characters
            ReferenceName = Name.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("_", "-");
            
            // Remove any characters that aren't lowercase letters, numbers, or hyphens
            ReferenceName = NonAlphanumericRegex().Replace(ReferenceName, "");
            
            // Replace multiple consecutive hyphens with a single hyphen
            ReferenceName = MultipleHyphenRegex().Replace(ReferenceName, "-");
            
            // Trim hyphens from start and end
            ReferenceName = ReferenceName.Trim('-');
        }
    }

    public void ConvertBase64ToBinary()
    {
        if (Type is AssetType.Image or AssetType.Font && BinaryContent is null)
        {
            var base64Content = Content;
            if (!string.IsNullOrEmpty(base64Content))
            {
                BinaryContent = Convert.FromBase64String(base64Content);
                Content = null; // Clear the base64 string after conversion
            }
        }
    }

    [GeneratedRegex("[^a-z0-9-]")]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex("-{2,}")]
    private static partial Regex MultipleHyphenRegex();
} 