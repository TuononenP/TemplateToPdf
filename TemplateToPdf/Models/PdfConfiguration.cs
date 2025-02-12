namespace TemplateToPdf.Models;

/// <summary>
/// Specifies the page size for PDF generation.
/// </summary>
public enum PageSize
{
    /// <summary>
    /// A0 page size (841 × 1189 millimeters)
    /// </summary>
    A0,

    /// <summary>
    /// A1 page size (594 × 841 millimeters)
    /// </summary>
    A1,

    /// <summary>
    /// A2 page size (420 × 594 millimeters)
    /// </summary>
    A2,

    /// <summary>
    /// A3 page size (297 × 420 millimeters)
    /// </summary>
    A3,

    /// <summary>
    /// A4 page size (210 × 297 millimeters)
    /// Most commonly used standard paper size worldwide.
    /// </summary>
    A4,

    /// <summary>
    /// A5 page size (148 × 210 millimeters)
    /// </summary>
    A5,

    /// <summary>
    /// A6 page size (105 × 148 millimeters)
    /// </summary>
    A6,

    /// <summary>
    /// US Letter page size (215.9 × 279.4 millimeters / 8.5 × 11 inches)
    /// Standard paper size in the United States and Canada.
    /// </summary>
    Letter,

    /// <summary>
    /// US Legal page size (215.9 × 355.6 millimeters / 8.5 × 14 inches)
    /// Commonly used for legal documents in the United States.
    /// </summary>
    Legal,

    /// <summary>
    /// Tabloid/Ledger page size (279.4 × 431.8 millimeters / 11 × 17 inches)
    /// Used for larger format printing such as newspapers and architectural drawings.
    /// </summary>
    Tabloid
}

/// <summary>
/// Configuration options for PDF generation.
/// </summary>
public class PdfConfiguration
{
    /// <summary>
    /// Gets or sets whether to sanitize HTML content before PDF generation.
    /// When true, potentially malicious content is removed.
    /// Defaults to true for security.
    /// </summary>
    public bool Sanitize { get; set; } = true;

    /// <summary>
    /// Gets or sets the page size for the generated PDF.
    /// Defaults to A4.
    /// </summary>
    public PageSize PageSize { get; set; } = PageSize.A4;
} 