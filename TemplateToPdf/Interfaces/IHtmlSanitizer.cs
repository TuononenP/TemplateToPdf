namespace TemplateToPdf.Interfaces;

public interface IHtmlSanitizer
{
    string Sanitize(string html);
} 