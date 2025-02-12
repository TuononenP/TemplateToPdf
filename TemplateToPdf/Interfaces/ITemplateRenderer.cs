namespace TemplateToPdf.Interfaces;

public interface ITemplateRenderer
{
    string RenderTemplate<T>(string template, T model, bool sanitize = true);
} 