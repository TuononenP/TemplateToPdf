using Wkhtmltopdf.NetCore;
using TemplateToPdf.Interfaces;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements PDF conversion using wkhtmltopdf
/// </summary>
public class WkhtmlToPdfConverter : IHtmlToPdfConverter
{
    private readonly IGeneratePdf _pdfGenerator;

    public WkhtmlToPdfConverter(IGeneratePdf pdfGenerator)
    {
        _pdfGenerator = pdfGenerator;
    }

    public async Task<byte[]> ConvertHtmlToPdfAsync(string html)
    {
        return await _pdfGenerator.GetByteArrayViewInHtml(html);
    }
} 