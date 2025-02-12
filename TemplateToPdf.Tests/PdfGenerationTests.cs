using Microsoft.Extensions.Logging;
using Moq;
using TemplateToPdf.Interfaces;
using TemplateToPdf.Services;
using Xunit;
using System.Globalization;
using System.Text.Json;
using TemplateToPdf.Models;

namespace TemplateToPdf.Tests;

public class PdfGenerationTests
{
    private readonly Mock<ILogger<HandlebarsTemplateRenderer>> _loggerMock;
    private readonly Mock<ILogger<PdfGenerationService>> _serviceLoggerMock;
    private readonly Mock<IHtmlToPdfConverter> _pdfConverterMock;
    private readonly Mock<IHtmlSanitizer> _htmlSanitizerMock;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly PdfGenerationService _pdfService;

    public PdfGenerationTests(
        Mock<ILogger<HandlebarsTemplateRenderer>>? loggerMock = null,
        Mock<ILogger<PdfGenerationService>>? serviceLoggerMock = null,
        Mock<IHtmlToPdfConverter>? pdfConverterMock = null,
        Mock<IHtmlSanitizer>? htmlSanitizerMock = null)
    {
        _loggerMock = loggerMock ?? new();
        _serviceLoggerMock = serviceLoggerMock ?? new();
        _pdfConverterMock = pdfConverterMock ?? new();
        _htmlSanitizerMock = htmlSanitizerMock ?? new();

        // Setup HTML sanitizer to preserve structure while removing dangerous content
        _htmlSanitizerMock
            .Setup(x => x.Sanitize(It.IsAny<string>()))
            .Returns((string html) =>
            {
                // Remove dangerous elements and attributes while preserving structure
                var sanitized = html
                    .Replace("<script>", "")
                    .Replace("</script>", "")
                    .Replace("javascript:", "")
                    .Replace(" onerror=", " data-removed-onerror=")
                    .Replace(" onclick=", " data-removed-onclick=");

                // Ensure proper HTML structure is maintained
                if (!sanitized.Contains("<html>") && !sanitized.Contains("</html>"))
                {
                    sanitized = $"<html><head></head><body>{sanitized}</body></html>";
                }

                return sanitized;
            });

        _templateRenderer = new HandlebarsTemplateRenderer(_loggerMock.Object, _htmlSanitizerMock.Object);
        _pdfService = new PdfGenerationService(
            _pdfConverterMock.Object,
            _templateRenderer,
            _htmlSanitizerMock.Object,
            _serviceLoggerMock.Object
        );

        // Setup mock PDF converter to return some dummy bytes
        _pdfConverterMock
            .Setup(x => x.ConvertHtmlToPdfAsync(It.IsAny<string>(), It.IsAny<PageSize>()))
            .ReturnsAsync(new byte[] { 1, 2, 3, 4, 5 });
    }

    private const string SimpleTemplate = "<html><body><h1>{{title}}</h1><p>{{content}}</p></body></html>";
    private const string ComplexTemplate = @"<html>
        <head>
            <style>
                body { font-family: Arial, sans-serif; padding: 20px; }
                .customer-info { margin: 20px 0; }
                table { width: 100%; border-collapse: collapse; margin-top: 20px; }
                th, td { padding: 8px; text-align: left; border: 1px solid #ddd; }
                th { background-color: #f5f5f5; font-weight: bold; }
                .total { font-weight: bold; background-color: #f9f9f9; }
                h1, h2 { margin-bottom: 10px; }
                .price { text-align: right; }
                .quantity { text-align: center; }
            </style>
        </head>
        <body>
            <h1>{{company.name}}</h1>
            <h2>Invoice #{{invoiceNumber}}</h2>
            
            <div class='customer-info'>
                <p><strong>Customer:</strong> {{customer.name}}</p>
                <p><strong>Date:</strong> {{formatDate date 'MM.dd.yyyy'}}</p>
            </div>

            <table>
                <thead>
                    <tr>
                        <th>Item</th>
                        <th class='quantity'>Quantity</th>
                        <th class='price'>Price</th>
                    </tr>
                </thead>
                <tbody>
                    {{#each items}}
                    <tr>
                        <td>{{name}}</td>
                        <td class='quantity'>{{quantity}}</td>
                        <td class='price'>${{price}}</td>
                    </tr>
                    {{/each}}
                </tbody>
                <tfoot>
                    <tr class='total'>
                        <td colspan='2'><strong>Total:</strong></td>
                        <td class='price'>${{total}}</td>
                    </tr>
                </tfoot>
            </table>
        </body>
    </html>";

    private static readonly object SimpleModel = new
    {
        title = "Hello",
        content = "World!"
    };

    private static readonly string SimpleJsonModel = @"{
        ""title"": ""Hello"",
        ""content"": ""World!""
    }";

    private static readonly object ComplexModel = new
    {
        company = new { name = "ACME Corp" },
        invoiceNumber = "INV-2024-001",
        customer = new { name = "John Doe" },
        date = DateTime.Parse("2024-03-14"),
        items = new[]
        {
            new { name = "Widget A", quantity = 2, price = 19.99m },
            new { name = "Widget B", quantity = 1, price = 29.99m }
        },
        total = 69.97m
    };

    private static readonly string ComplexJsonModel = @"{
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
    }";

    private const string MaliciousTemplate = @"<html>
        <body>
            <h1>{{title}}</h1>
            <script>alert('xss');</script>
            <img src='x' onerror='alert(1)'>
            <a href='javascript:alert(2)'>Click me</a>
            <div onclick='alert(3)'>Click me</div>
            <p>{{content}}</p>
        </body>
    </html>";

    private static readonly object MaliciousModel = new
    {
        title = "<script>alert('xss in title')</script>Hello",
        content = @"<img src='x' onerror='alert(1)'>World!
                   <a href='javascript:alert(2)'>Click me</a>"
    };

    private static readonly string MaliciousJsonModel = @"{
        ""title"": ""<script>alert('xss in title')</script>Hello"",
        ""content"": ""<img src='x' onerror='alert(1)'>World!<a href='javascript:alert(2)'>Click me</a>""
    }";

    private async Task VerifyPdfGeneration(string template, object model, PdfConfiguration options, params string[] expectedContent)
    {
        // Act
        var result = await _pdfService.GeneratePdfFromTemplateAsync(template, model, options);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
        
        _pdfConverterMock.Verify(
            x => x.ConvertHtmlToPdfAsync(It.Is<string>(html => 
                expectedContent.All(content => html.Contains(content))), options.PageSize),
            Times.Once);
    }

    private async Task VerifyHtmlSanitization(string template, object model, PdfConfiguration options, string[] expectedContent, string[] unexpectedContent)
    {
        // Act
        var result = await _pdfService.GeneratePdfFromTemplateAsync(template, model, options);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);

        // Verify HTML sanitizer was called
        _htmlSanitizerMock.Verify(x => x.Sanitize(It.IsAny<string>()), Times.Once);
        
        _pdfConverterMock.Verify(
            x => x.ConvertHtmlToPdfAsync(It.Is<string>(html => 
                expectedContent.All(content => html.Contains(content)) &&
                unexpectedContent.All(content => !html.Contains(content))), options.PageSize),
            Times.Once);
    }

    [Fact]
    public Task GeneratePdf_SimpleExample_ShouldRenderCorrectly()
    {
        return VerifyPdfGeneration(
            SimpleTemplate,
            SimpleModel,
            new PdfConfiguration { Sanitize = true, PageSize = PageSize.A4 },
            "Hello",
            "World!"
        );
    }

    [Fact]
    public Task GeneratePdf_SimpleExampleWithJson_ShouldRenderCorrectly()
    {
        var jsonModel = JsonSerializer.Deserialize<JsonElement>(SimpleJsonModel);
        return VerifyPdfGeneration(
            SimpleTemplate,
            jsonModel,
            new PdfConfiguration { Sanitize = true, PageSize = PageSize.A4 },
            "Hello",
            "World!"
        );
    }

    [Fact]
    public Task GeneratePdf_ComplexInvoiceExample_ShouldRenderCorrectly()
    {
        return VerifyPdfGeneration(
            ComplexTemplate,
            ComplexModel,
            new PdfConfiguration { Sanitize = true, PageSize = PageSize.Letter },
            "ACME Corp",
            "INV-2024-001",
            "John Doe",
            "03.14.2024",
            "Widget A",
            "Widget B",
            "19,99",
            "29,99",
            "69,97"
        );
    }

    [Fact]
    public Task GeneratePdf_ComplexInvoiceExampleWithJson_ShouldRenderCorrectly()
    {
        var jsonModel = JsonSerializer.Deserialize<JsonElement>(ComplexJsonModel);
        return VerifyPdfGeneration(
            ComplexTemplate,
            jsonModel,
            new PdfConfiguration { Sanitize = true, PageSize = PageSize.Letter },
            "ACME Corp",
            "INV-2024-001",
            "John Doe",
            "03.14.2024",
            "Widget A",
            "Widget B",
            "19,99",
            "29,99",
            "69,97"
        );
    }

    [Fact]
    public Task GeneratePdf_WithMaliciousTemplate_ShouldSanitizeHtml()
    {
        return VerifyHtmlSanitization(
            MaliciousTemplate,
            SimpleModel,
            new PdfConfiguration { Sanitize = true, PageSize = PageSize.A4 },
            // Expected content (safe)
            ["Hello", "World!", "<h1>", "<p>", "<body>"],
            // Unexpected content (unsafe)
            [ 
                "<script>", "alert(", "javascript:", 
                "onerror=", "onclick=", 
                "alert('xss')", "alert(1)", "alert(2)", "alert(3)" 
            ]
        );
    }

    [Fact]
    public Task GeneratePdf_WithMaliciousModel_ShouldSanitizeContent()
    {
        return VerifyHtmlSanitization(
            SimpleTemplate,
            MaliciousModel,
            new PdfConfiguration { Sanitize = true, PageSize = PageSize.A4 },
            // Expected content (safe)
            ["Hello", "World!", "<h1>", "<p>"],
            // Unexpected content (unsafe)
            [ 
                "<script>", "alert(", "javascript:", 
                "onerror=", "onclick=",
                "alert('xss in title')", "alert(1)", "alert(2)" 
            ]
        );
    }

    [Fact]
    public Task GeneratePdf_WithMaliciousJsonModel_ShouldSanitizeContent()
    {
        var jsonModel = JsonSerializer.Deserialize<JsonElement>(MaliciousJsonModel);
        return VerifyHtmlSanitization(
            SimpleTemplate,
            jsonModel,
            new PdfConfiguration { Sanitize = true, PageSize = PageSize.A4 },
            // Expected content (safe)
            ["Hello", "World!", "<h1>", "<p>"],
            // Unexpected content (unsafe)
            [ 
                "<script>", "alert(", "javascript:", 
                "onerror=", "onclick=",
                "alert('xss in title')", "alert(1)", "alert(2)" 
            ]
        );
    }
} 