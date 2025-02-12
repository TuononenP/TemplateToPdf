namespace TemplateToPdf.Interfaces;

public interface IHtmlToPdfConverter
{
    Task<byte[]> ConvertHtmlToPdfAsync(string html);
} 