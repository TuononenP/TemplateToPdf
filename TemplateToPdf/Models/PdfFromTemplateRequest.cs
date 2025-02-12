using System.Text.Json;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace TemplateToPdf.Models;

/// <summary>
/// Request model for generating a PDF from a saved template
/// </summary>
public class PdfFromTemplateRequest
{
    /// <summary>
    /// The ID of the template to use for PDF generation
    /// </summary>
    public int TemplateId { get; set; }

    /// <summary>
    /// The data model to bind to the template
    /// </summary>
    public object Model { get; set; } = default!;

    /// <summary>
    /// Configuration options for PDF generation
    /// </summary>
    public PdfConfiguration Options { get; set; } = new();
}

public class PdfFromTemplateRequestExample : IMultipleExamplesProvider<PdfFromTemplateRequest>
{
    public IEnumerable<SwaggerExample<PdfFromTemplateRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Invoice Example",
            "Example using template ID 1 with invoice data model",
            new PdfFromTemplateRequest
            {
                TemplateId = 1,
                Model = JsonSerializer.Deserialize<JsonElement>(@"{
                    ""company"": {
                        ""name"": ""ACME Corp""
                    },
                    ""invoiceNumber"": ""INV-2024-001"",
                    ""customer"": {
                        ""name"": ""John Doe""
                    },
                    ""date"": ""2024-03-14T00:00:00"",
                    ""items"": [
                        {
                            ""name"": ""Widget A"",
                            ""quantity"": 2,
                            ""price"": 19.99
                        },
                        {
                            ""name"": ""Widget B"",
                            ""quantity"": 1,
                            ""price"": 29.99
                        }
                    ],
                    ""total"": 69.97
                }"),
                Options = new PdfConfiguration
                {
                    Sanitize = true,
                    PageSize = PageSize.Letter
                }
            }
        );
    }
} 