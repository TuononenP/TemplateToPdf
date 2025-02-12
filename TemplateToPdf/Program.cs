using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Runtime.InteropServices;
using TemplateToPdf.Services;
using TemplateToPdf.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Disable default port bindings
builder.WebHost.UseUrls(); // This clears default URLs, allowing only environment-specified ones

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

// Configure Kestrel for large responses and increased timeouts
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; // 100MB
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    options.Limits.MaxResponseBufferSize = null; // Unlimited
    options.AllowSynchronousIO = true; // For large file operations
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = new[]
    {
        "application/pdf",
        "application/json"
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition"); // Allow clients to read the filename
        });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Template to PDF API", 
        Version = "v1",
        Description = "API for generating PDFs from Handlebars templates"
    });
    
    c.EnableAnnotations();
    c.ExampleFilters();
    
    // Include XML comments in Swagger UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// Register PDF generation services
builder.Services.AddScoped<IHtmlToPdfConverter, PuppeteerPdfConverter>();
builder.Services.AddScoped<ITemplateRenderer, HandlebarsTemplateRenderer>();
builder.Services.AddScoped<IHtmlSanitizer, HtmlSanitizerWrapper>();
builder.Services.AddScoped<IPdfGenerationService, PdfGenerationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Template to PDF API V1");
        c.RoutePrefix = ""; // Serve the Swagger UI at the root URL
    });
}

// Enable response compression and CORS
app.UseResponseCompression();
app.UseCors("AllowedOrigins");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting Template to PDF API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
} 