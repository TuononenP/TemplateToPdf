using System.ComponentModel.DataAnnotations;

namespace TemplateToPdf.Models;

public class Template
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = default!;
    
    [Required]
    public string Content { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
} 