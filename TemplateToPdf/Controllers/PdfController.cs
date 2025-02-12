using Microsoft.AspNetCore.Mvc;
using TemplateToPdf.Services;
using TemplateToPdf.Interfaces;
using TemplateToPdf.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Serilog;
using Microsoft.AspNetCore.ResponseCompression;

namespace TemplateToPdf.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PdfController : ControllerBase
{
    private readonly IPdfGenerationService _pdfService;
    private readonly ILogger<PdfController> _logger;

    public PdfController(IPdfGenerationService pdfService, ILogger<PdfController> logger)
    {
        _pdfService = pdfService;
        _logger = logger;
    }

    /// <summary>
    /// Generates a PDF document from a Handlebars template and data model
    /// </summary>
    /// <param name="request">The template, data model, and optional title for PDF generation</param>
    /// <returns>PDF file as a byte array</returns>
    /// <response code="200">Returns the PDF file</response>
    /// <response code="400">If the template or model is invalid</response>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [SwaggerOperation(
        Summary = "Generate PDF from template",
        Description = "Generates a PDF document using a Handlebars template and provided data model")]
    [ResponseCache(Duration = 60)] // Cache for 1 minute
    [RequestSizeLimit(100 * 1024 * 1024)] // 100MB limit
    [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
    public async Task<IActionResult> GeneratePdf([FromBody] PdfGenerationRequest request)
    {
        try
        {
            _logger.LogInformation("Starting PDF generation for file: {Filename}", request.Filename);
            
            var pdfBytes = await _pdfService.GeneratePdfFromTemplateAsync(request.Template, request.Model);
            
            _logger.LogInformation("Successfully generated PDF: {Filename}, Size: {Size} bytes", 
                request.Filename, pdfBytes.Length);

            Response.Headers.Append("Content-Disposition", $"attachment; filename={request.Filename}.pdf");
            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF: {Filename}", request.Filename);
            
            return BadRequest(new ProblemDetails
            {
                Title = "PDF Generation Failed",
                Detail = ex.Message
            });
        }
    }
} 