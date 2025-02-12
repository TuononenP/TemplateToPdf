using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace TemplateToPdf.Models;

public class PdfGenerationRequest
{
    public required string Template { get; set; }
    public required object Model { get; set; }
    public string Filename { get; set; } = "ExamplePDF";
}

public class PdfGenerationRequestExample : IMultipleExamplesProvider<PdfGenerationRequest>
{
    public IEnumerable<SwaggerExample<PdfGenerationRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Simple Example",
            "Basic example with a simple template and model",
            new PdfGenerationRequest
            {
                Template = @"<html><body><h1>{{title}}</h1><p>{{content}}</p></body></html>",
                Model = JsonSerializer.Deserialize<JsonElement>(@"{
                    ""title"": ""Hello"",
                    ""content"": ""World!""
                }"),
                Filename = "simple-example"
            }
        );

        yield return SwaggerExample.Create(
            "Invoice Example",
            "Complex example showing an invoice template with nested objects and arrays",
            new PdfGenerationRequest
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
                Filename = "invoice-example"
            }
        );
    }
} 