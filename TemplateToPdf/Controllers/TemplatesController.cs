using Microsoft.AspNetCore.Mvc;
using TemplateToPdf.Models;
using TemplateToPdf.Services;

namespace TemplateToPdf.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController(ITemplatesService templatesService) : ControllerBase
{
    private readonly ITemplatesService _templatesService = templatesService;

    // GET: api/templates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Template>>> GetTemplates(
        [FromQuery] string? name,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string sort = "updatedAt",
        [FromQuery] string order = "DESC")
    {
        var (templates, totalCount) = await _templatesService.GetTemplatesAsync(name, page, perPage, sort, order);
        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");

        return Ok(templates);
    }

    // GET: api/templates/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Template>> GetTemplate(int id)
    {
        var template = await _templatesService.GetTemplateAsync(id);
        if (template == null)
        {
            return NotFound();
        }

        return template;
    }

    // POST: api/templates
    [HttpPost]
    public async Task<ActionResult<Template>> CreateTemplate(Template template)
    {
        var created = await _templatesService.CreateTemplateAsync(template);
        return CreatedAtAction(nameof(GetTemplate), new { id = created.Id }, created);
    }

    // PUT: api/templates/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTemplate(int id, Template template)
    {
        try
        {
            await _templatesService.UpdateTemplateAsync(id, template);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    // DELETE: api/templates/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        try
        {
            await _templatesService.DeleteTemplateAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 