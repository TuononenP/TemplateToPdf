using Microsoft.AspNetCore.Mvc;
using TemplateToPdf.Services;
using TemplateToPdf.Interfaces;
using TemplateToPdf.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Serilog;
using Microsoft.AspNetCore.ResponseCompression;
using TemplateToPdf.Data;

namespace TemplateToPdf.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController(IPdfGenerationService pdfService, ILogger<PdfController> logger, TemplateDbContext context) : ControllerBase
{
    private readonly IPdfGenerationService _pdfService = pdfService;
    private readonly ILogger<PdfController> _logger = logger;
    private readonly TemplateDbContext _context = context;

    /// <summary>
    /// Generates a PDF document from a Handlebars template and data model
    /// </summary>
    /// <param name="request">The template, data model, and optional title for PDF generation</param>
    /// <returns>PDF file as a byte array</returns>
    /// <response code="200">Returns the PDF file</response>
    /// <response code="400">If the template or model is invalid</response>
    [HttpPost("generate/html")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [SwaggerOperation(
        Summary = "Generate PDF from HTML template",
        Description = "Generates a PDF document using a Handlebars HTML template and provided data model")]
    [ResponseCache(Duration = 60)] // Cache for 1 minute
    [RequestSizeLimit(100 * 1024 * 1024)] // 100MB limit
    [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
    public async Task<IActionResult> GeneratePdfFromHtml([FromBody] PdfFromHtmlRequest request)
    {
        try
        {
            _logger.LogInformation("Starting PDF generation from HTML for file: {Filename}", request.Filename);
            
            var pdfBytes = await _pdfService.GeneratePdfFromTemplateAsync(
                request.Template, 
                request.Model, 
                request.Options);
            
            _logger.LogInformation("Successfully generated PDF from HTML: {Filename}, Size: {Size} bytes", 
                request.Filename, pdfBytes.Length);

            Response.Headers.Append("Content-Disposition", $"attachment; filename={request.Filename}.pdf");
            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF from HTML: {Filename}", request.Filename);
            
            return BadRequest(new ProblemDetails
            {
                Title = "PDF Generation from HTML Failed",
                Detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Generates a PDF document from a saved template
    /// </summary>
    /// <param name="request">The request containing the template ID and data model</param>
    /// <returns>PDF file as a byte array</returns>
    /// <response code="200">Returns the PDF file</response>
    /// <response code="404">If the template with the specified ID is not found</response>
    [HttpPost("generate/template")]
    [ProducesResponseType(typeof(FileContentResult), 200)]  
    [ProducesResponseType(404)]
    [SwaggerOperation(
        Summary = "Generate PDF from saved template",
        Description = "Generates a PDF document using a saved template and provided data model")]
    [SwaggerRequestExample(typeof(PdfFromTemplateRequest), typeof(PdfFromTemplateRequestExample))]
    [ResponseCache(Duration = 60)] // Cache for 1 minute
    [RequestSizeLimit(100 * 1024 * 1024)] // 100MB limit
    [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
    public async Task<IActionResult> GeneratePdfFromTemplate([FromBody] PdfFromTemplateRequest request)
    {
        var template = await _context.Templates.FindAsync(request.TemplateId);
        if (template == null)
        {
            return NotFound();
        }

        try 
        {
            var pdfBytes = await _pdfService.GeneratePdfFromTemplateAsync(
                template.Content,
                request.Model, 
                request.Options);

            return File(pdfBytes, "application/pdf", $"{template.Name}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF from template {TemplateId}", request.TemplateId);
            return StatusCode(500, "An error occurred while generating the PDF");
        }
    }
} 