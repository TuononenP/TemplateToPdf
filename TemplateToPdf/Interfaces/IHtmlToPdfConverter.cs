namespace TemplateToPdf.Interfaces;
using TemplateToPdf.Models;

public interface IHtmlToPdfConverter
{
    Task<byte[]> ConvertHtmlToPdfAsync(string html, PageSize pageSize);
} 