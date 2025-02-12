using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace TemplateToPdf.Models;

/// <summary>
/// Request model for generating a PDF from an HTML template
/// </summary>
public class PdfFromHtmlRequest
{
    /// <summary>
    /// The HTML template with Handlebars expressions
    /// </summary>
    public required string Template { get; set; }

    /// <summary>
    /// The data model to bind to the template
    /// </summary>
    public required object Model { get; set; }

    /// <summary>
    /// The filename for the generated PDF (without extension)
    /// </summary>
    public string Filename { get; set; } = "ExamplePDF";

    /// <summary>
    /// Configuration options for PDF generation
    /// </summary>
    public PdfConfiguration Options { get; set; } = new();
}

/// <summary>
/// Provides Swagger examples for the PdfFromHtmlRequest
/// </summary>
public class PdfFromHtmlRequestExample : IMultipleExamplesProvider<PdfFromHtmlRequest>
{
    public IEnumerable<SwaggerExample<PdfFromHtmlRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Simple Example",
            "Basic example with a simple template and model",
            new PdfFromHtmlRequest
            {
                Template = @"<html><body><h1>{{title}}</h1><p>{{content}}</p></body></html>",
                Model = JsonSerializer.Deserialize<JsonElement>(@"{
                    ""title"": ""Hello"",
                    ""content"": ""World!""
                }"),
                Filename = "simple-example",
                Options = new PdfConfiguration
                {
                    Sanitize = true,
                    PageSize = PageSize.A4
                }
            }
        );

        yield return SwaggerExample.Create(
            "Invoice Example",
            "Complex example showing an invoice template with nested objects and arrays",
            new PdfFromHtmlRequest
            {
                Template = @"<html>
                                <body>
                                    <h1>{{company.name}}</h1>
                                    <h2>Invoice #{{invoiceNumber}}</h2>
                                    
                                    <div class='customer-info'>
                                        <p>Customer: {{customer.name}}</p>
                                        <p>Date: {{formatDate date 'MM/dd/yyyy'}}</p>
                                    </div>

                                    <table>
                                        <thead>
                                            <tr>
                                                <th>Item</th>
                                                <th>Quantity</th>
                                                <th>Price</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {{#each items}}
                                            <tr>
                                                <td>{{name}}</td>
                                                <td>{{quantity}}</td>
                                                <td>${{price}}</td>
                                            </tr>
                                            {{/each}}
                                        </tbody>
                                        <tfoot>
                                            <tr>
                                                <td colspan='2'>Total:</td>
                                                <td>${{total}}</td>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </body>
                            </html>",
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
                Filename = "invoice-example",
                Options = new PdfConfiguration
                {
                    Sanitize = true,
                    PageSize = PageSize.Letter
                }
            }
        );

        yield return SwaggerExample.Create(
            "Raw HTML Example",
            "Example with sanitization disabled to allow raw HTML content",
            new PdfFromHtmlRequest
            {
                Template = @"<html><body><h1>{{title}}</h1><div>{{{rawHtmlContent}}}</div></body></html>",
                Model = JsonSerializer.Deserialize<JsonElement>(@"{
                    ""title"": ""Raw HTML Example"",
                    ""rawHtmlContent"": ""<div style='color: red;'><strong>This HTML</strong> will not be sanitized</div>""
                }"),
                Filename = "raw-html-example",
                Options = new PdfConfiguration
                {
                    Sanitize = false,
                    PageSize = PageSize.A4
                }
            }
        );
    }
} 