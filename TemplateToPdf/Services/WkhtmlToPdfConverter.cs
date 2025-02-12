using Wkhtmltopdf.NetCore;
using TemplateToPdf.Interfaces;
using TemplateToPdf.Models;

namespace TemplateToPdf.Services;

/// <summary>
/// Implements PDF conversion using wkhtmltopdf
/// </summary>
public class WkhtmlToPdfConverter(IGeneratePdf pdfGenerator) : IHtmlToPdfConverter
{
    private readonly IGeneratePdf _pdfGenerator = pdfGenerator;

    public async Task<byte[]> ConvertHtmlToPdfAsync(string html, PageSize pageSize)
    {
        return await _pdfGenerator.GetByteArrayViewInHtml(html);
    }
} 