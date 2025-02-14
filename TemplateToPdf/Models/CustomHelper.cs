using System.ComponentModel.DataAnnotations;

namespace TemplateToPdf.Models;

public class CustomHelper
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = default!;
    
    [Required]
    public string Description { get; set; } = default!;
    
    [Required]
    public string FunctionBody { get; set; } = default!;
    
    public bool IsEnabled { get; set; } = true;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
} 