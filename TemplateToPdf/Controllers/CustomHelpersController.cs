using Microsoft.AspNetCore.Mvc;
using TemplateToPdf.Models;
using TemplateToPdf.Services;

namespace TemplateToPdf.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomHelpersController : ControllerBase
{
    private readonly ICustomHelperService _customHelperService;
    private readonly ILogger<CustomHelpersController> _logger;

    public CustomHelpersController(
        ICustomHelperService customHelperService,
        ILogger<CustomHelpersController> logger)
    {
        _customHelperService = customHelperService;
        _logger = logger;
    }

    // GET: api/customhelpers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomHelper>>> GetCustomHelpers()
    {
        var helpers = await _customHelperService.GetAllHelpersAsync();
        return Ok(helpers);
    }

    // GET: api/customhelpers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomHelper>> GetCustomHelper(int id)
    {
        var helper = await _customHelperService.GetHelperAsync(id);
        if (helper == null)
        {
            return NotFound();
        }

        return helper;
    }

    // POST: api/customhelpers
    [HttpPost]
    public async Task<ActionResult<CustomHelper>> CreateCustomHelper(CustomHelper helper)
    {
        try
        {
            var created = await _customHelperService.CreateHelperAsync(helper);
            return CreatedAtAction(nameof(GetCustomHelper), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid helper function");
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT: api/customhelpers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomHelper(int id, CustomHelper helper)
    {
        if (id != helper.Id)
        {
            return BadRequest();
        }

        try
        {
            await _customHelperService.UpdateHelperAsync(helper);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Invalid helper function");
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE: api/customhelpers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomHelper(int id)
    {
        await _customHelperService.DeleteHelperAsync(id);
        return NoContent();
    }

    // Example helper functions for documentation
    [HttpGet("examples")]
    public ActionResult<IEnumerable<object>> GetExampleHelpers()
    {
        var examples = new[]
        {
            new
            {
                Name = "repeat",
                Description = "Repeats a string n times",
                FunctionBody = @"
                    if (args.Length < 2) return string.Empty;
                    var text = args[0];
                    if (!int.TryParse(args[1], out var count)) return text;
                    return string.Concat(Enumerable.Repeat(text, count));",
                Example = "{{repeat \"Hello \" 3}} -> Hello Hello Hello"
            },
            new
            {
                Name = "padLeft",
                Description = "Pads a string to the left with a specified character",
                FunctionBody = @"
                    if (args.Length < 3) return args[0] ?? string.Empty;
                    var text = args[0] ?? string.Empty;
                    if (!int.TryParse(args[1], out var totalWidth)) return text;
                    var padChar = args[2]?[0] ?? ' ';
                    return text.PadLeft(totalWidth, padChar);",
                Example = "{{padLeft \"42\" 5 \"0\"}} -> 00042"
            },
            new
            {
                Name = "truncate",
                Description = "Truncates a string to a specified length and adds ellipsis",
                FunctionBody = @"
                    if (args.Length < 2) return args[0] ?? string.Empty;
                    var text = args[0] ?? string.Empty;
                    if (!int.TryParse(args[1], out var maxLength)) return text;
                    return text.Length <= maxLength ? text : text[..maxLength] + ""..."";",
                Example = "{{truncate \"Long text here\" 8}} -> Long tex..."
            }
        };

        return Ok(examples);
    }
} 