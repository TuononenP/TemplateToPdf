# Template to PDF API

A .NET 8 Web API for generating PDFs from Handlebars templates using wkhtmltopdf.

## Prerequisites

- .NET 8 SDK
- wkhtmltopdf

### Installing wkhtmltopdf

#### Windows

1. Download the installer from the [official releases page](https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-2/wkhtmltox-0.12.6-2.msi)
2. Run the installer (wkhtmltox-0.12.6-2.msi)
3. The default installation path will be `C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe`
4. Add the installation directory to your system's PATH environment variable

#### macOS

Run the following command in Terminal:

```bash
/bin/zsh -c "cd /tmp && curl -L https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-2/wkhtmltox-0.12.6-2.macos-cocoa.pkg -o wkhtmltopdf.pkg && sudo installer -pkg wkhtmltopdf.pkg -target / && ls -l /usr/local/bin/wkhtmltopdf"
```

This will:
1. Download the wkhtmltopdf package
2. Install it to your system
3. Verify the installation by checking the binary location

#### Linux (Debian/Ubuntu)

```bash
# Install dependencies
sudo apt-get update
sudo apt-get install -y wget xfonts-75dpi xfonts-base

# Download and install wkhtmltopdf
wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-2/wkhtmltox_0.12.6-2.jammy_amd64.deb
sudo dpkg -i wkhtmltox_0.12.6-2.jammy_amd64.deb
sudo apt-get install -f

# Verify installation
which wkhtmltopdf
```

For other Linux distributions:
- RHEL/CentOS/Fedora: Use the `.rpm` package from the releases page
- Arch Linux: Install via `pacman -S wkhtmltopdf`
- Alpine: Install via `apk add wkhtmltopdf`

## Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
```bash
dotnet run --project TemplateToPdf/TemplateToPdf.csproj
```
4. The API will be available at:
   - HTTP: http://localhost:6050
   - HTTPS: https://localhost:6051

## Features

- Template Management
  - Create, edit, and delete HTML templates
  - Preview templates with sample data
  - Generate PDFs from templates
  - Built-in and custom Handlebars helpers
  - Template versioning with creation and update timestamps
- Asset Management
  - Support for multiple asset types:
    - Images (stored as binary data)
    - CSS stylesheets
    - Fonts (WOFF2 format)
    - Partial templates
  - Automatic reference name generation
  - Content type validation
  - Easy integration with templates
- PDF Generation
  - Multiple page sizes (A0-A6, Letter, Legal, Tabloid)
  - Custom margins and styling
  - HTML sanitization for security
  - Support for custom fonts
  - Caching for improved performance
- User Interface
  - Modern React-based interface
  - Light and dark themes
  - Multi-language support (English, Finnish, Swedish)
  - Template preview and testing
  - Asset library management
  - Responsive design
- Security
  - HTML content sanitization
  - Safe evaluation of custom helpers
  - HTTPS support
  - Input validation
  - Error handling and logging

## Built-in Handlebars Helpers

### String Helpers
```handlebars
{{uppercase "text"}}
{{lowercase "TEXT"}}
{{substring "text" 0 2}}
```

### Number Helpers
```handlebars
{{formatNumber 1234.5678}}  <!-- 1,234.57 -->
{{formatCurrency 42.99 "USD"}}  <!-- $42.99 -->
```

### Math Helpers
```handlebars
{{sum 1 2 3 4}}  <!-- 10 -->
{{multiply 5 3}}  <!-- 15 -->
{{divide 10 2}}  <!-- 5 -->
{{round 3.14159 2}}  <!-- 3.14 -->
```

### Array Helpers
```handlebars
{{length items}}
{{join items ", "}}
```

### Date Helpers
```handlebars
{{formatDate date "MM/dd/yyyy"}}
{{now "yyyy-MM-dd"}}
{{addDays date 7 "MM/dd/yyyy"}}
```

### Conditional Helpers
```handlebars
{{#if (eq value1 value2)}}Equal{{/if}}
{{#if (gt number1 number2)}}Greater{{/if}}
{{#if (lt number1 number2)}}Less{{/if}}
```

### Asset Helpers
```handlebars
{{css "main-styles"}}  <!-- Include CSS -->
{{image "company-logo"}}  <!-- Include image -->
{{font "open-sans"}}  <!-- Include font -->
{{partial "header"}}  <!-- Include partial template -->
```

## Asset Management

Assets can be managed through the API and are automatically available in templates. Each asset type has specific handling:

### Images
- Stored as binary data
- Automatically converted to base64 for display
- Supported formats: JPEG, PNG, GIF, SVG
```bash
curl -k -X POST "https://localhost:6051/api/assets" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Company Logo",
    "type": "Image",
    "mimeType": "image/png",
    "binaryContent": "'$(base64 -i logo.png)'"
  }'
```

### CSS Stylesheets
- Stored as text
- Automatically wrapped in style tags
```bash
curl -k -X POST "https://localhost:6051/api/assets" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Main Styles",
    "type": "Css",
    "content": "body { font-family: Arial; } .header { background: #f5f5f5; }"
  }'
```

### Fonts
- Stored as WOFF2 binary data
- Automatically generates @font-face declaration
```bash
curl -k -X POST "https://localhost:6051/api/assets" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Open Sans",
    "type": "Font",
    "mimeType": "font/woff2",
    "binaryContent": "'$(base64 -i OpenSans.woff2)'"
  }'
```

### Partial Templates
- Reusable template fragments
- Can access parent template context
- Uses standard Handlebars partial syntax
- Supports nested partials (partials can include other partials)
```handlebars
{{> partial-name}}  <!-- Standard Handlebars partial syntax -->
{{> header}}       <!-- Example: Include header partial -->
{{> footer}}       <!-- Example: Include footer partial -->
```

Example partial template creation:
```bash
curl -k -X POST "https://localhost:6051/api/assets" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Invoice Header",
    "type": "PartialTemplate",
    "content": "<div class=\"header\"><h1>{{company.name}}</h1><p>Invoice #{{invoiceNumber}}</p></div>"
  }'
```

Usage in main template:
```handlebars
<html>
<body>
  {{> "invoice-header"}}  <!-- Will be replaced with the partial content -->
  <div class="content">
    <!-- Rest of the invoice content -->
  </div>
</body>
</html>
```

Note: Partial templates:
- Are automatically registered when the template renderer is created
- Share the same context as the parent template
- Can be used multiple times in the same template
- Support dynamic content through variables

## Themes and Localization

### Themes
The application supports light and dark themes, which can be changed in the settings:
- Light theme: Default theme with white background
- Dark theme: Dark mode with reduced eye strain

### Languages
Supported languages:
- English (default)
- Finnish (Suomi)
- Swedish (Svenska)

Language can be changed in the settings and affects:
- UI elements
- Error messages
- Date formats
- Number formats

## API Usage

The API provides a single endpoint for generating PDFs from Handlebars templates. Here are example requests using curl:

### Simple Example
```bash
curl -k -X POST "https://localhost:6051/api/pdf/generate" \
  -H "Content-Type: application/json" \
  -H "Accept: application/pdf" \
  -d '{
    "template": "<html><body><h1>{{title}}</h1><p>{{content}}</p></body></html>",
    "model": {
        "title": "Hello",
        "content": "World!"
    },
    "filename": "simple-example"
  }' \
  -o output.pdf
```

### Complex Invoice Example
```bash
curl -k -X POST "https://localhost:6051/api/pdf/generate" \
  -H "Content-Type: application/json" \
  -H "Accept: application/pdf" \
  -d '{
    "template": "<html><body><h1>{{company.name}}</h1><h2>Invoice #{{invoiceNumber}}</h2><div class=\"customer-info\"><p>Customer: {{customer.name}}</p><p>Date: {{formatDate date \"MM/dd/yyyy\"}}</p></div><table><thead><tr><th>Item</th><th>Quantity</th><th>Price</th></tr></thead><tbody>{{#each items}}<tr><td>{{name}}</td><td>{{quantity}}</td><td>${{price}}</td></tr>{{/each}}</tbody><tfoot><tr><td colspan=\"2\">Total:</td><td>${{total}}</td></tr></tfoot></table></body></html>",
    "model": {
        "company": {
            "name": "ACME Corp"
        },
        "invoiceNumber": "INV-2024-001",
        "customer": {
            "name": "John Doe"
        },
        "date": "2024-03-14T00:00:00",
        "items": [
            {
                "name": "Widget A",
                "quantity": 2,
                "price": 19.99
            },
            {
                "name": "Widget B",
                "quantity": 1,
                "price": 29.99
            }
        ],
        "total": 69.97
    },
    "filename": "invoice-example"
  }' \
  -o invoice.pdf
```

### Important Notes
- Use HTTPS endpoint (port 6051) for secure communication
- Include the `-k` flag to accept self-signed certificates in development
- Use the `-o` flag to save the PDF output to a file
- The response will be a binary PDF file
- Maximum request size is 100MB
- Response is cached for 1 minute
- The `Content-Disposition` header will include the specified filename with a `.pdf` extension

### Response Headers
```
Content-Type: application/pdf
Content-Disposition: attachment; filename=example.pdf
Cache-Control: public,max-age=60
```

### API Endpoint Details
- Method: POST
- Path: `/api/pdf/generate`
- Request Body:
  - `template`: HTML template with Handlebars expressions
  - `model`: Data model for the template
  - `filename`: Optional filename for the generated PDF (defaults to "ExamplePDF") 

## Custom Handlebars Helpers

You can create custom Handlebars helpers that are immediately available for use in your templates. These helpers are stored in the database and can be managed through the API.

### Adding a Custom Helper

```bash
curl -k -X POST "https://localhost:6051/api/customhelpers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "repeat",
    "description": "Repeats a string n times",
    "functionBody": "if (args.Length < 2) return string.Empty; var text = args[0]; if (!int.TryParse(args[1], out var count)) return text; return string.Concat(Enumerable.Repeat(text, count));",
    "isEnabled": true
  }'
```

The helper can be used immediately in your templates:
```handlebars
{{repeat "Hello " 3}}  <!-- Outputs: Hello Hello Hello -->
```

### More Helper Examples

1. Pad a number with zeros:
```bash
curl -k -X POST "https://localhost:6051/api/customhelpers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "padNumber",
    "description": "Pads a number with leading zeros",
    "functionBody": "if (args.Length < 2) return args[0] ?? string.Empty; var number = args[0]; var length = int.Parse(args[1]); return number?.PadLeft(length, '0') ?? string.Empty;",
    "isEnabled": true
  }'
```

Usage:
```handlebars
{{padNumber "42" "5"}}  <!-- Outputs: 00042 -->
```

2. Truncate text with ellipsis:
```bash
curl -k -X POST "https://localhost:6051/api/customhelpers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "truncate",
    "description": "Truncates text and adds ellipsis",
    "functionBody": "if (args.Length < 2) return args[0] ?? string.Empty; var text = args[0] ?? string.Empty; var length = int.Parse(args[1]); return text.Length <= length ? text : text.Substring(0, length) + \"...\";",
    "isEnabled": true
  }'
```

Usage:
```handlebars
{{truncate "This is a long text" "10"}}  <!-- Outputs: This is a... -->
```

3. Format phone number:
```bash
curl -k -X POST "https://localhost:6051/api/customhelpers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "phoneFormat",
    "description": "Formats a phone number (US format)",
    "functionBody": "if (string.IsNullOrEmpty(args[0])) return string.Empty; var cleaned = new string(args[0].Where(c => char.IsDigit(c)).ToArray()); if (cleaned.Length == 10) return $\"({cleaned.Substring(0, 3)}) {cleaned.Substring(3, 3)}-{cleaned.Substring(6)}\"; return args[0];",
    "isEnabled": true
  }'
```

Usage:
```handlebars
{{phoneFormat "1234567890"}}  <!-- Outputs: (123) 456-7890 -->
```

### Important Notes

- Custom helpers are written in C# and have access to basic .NET functionality
- Helpers are validated before being saved to ensure they compile correctly
- New helpers are available immediately without needing to restart the application
- Each helper should handle null/empty inputs gracefully
- The `args` array contains the arguments passed to the helper as strings
- Helper functions should be kept simple and focused on a single task
- Use `isEnabled: false` to temporarily disable a helper without deleting it

### Managing Helpers

List all helpers:
```bash
curl -k -X GET "https://localhost:6051/api/customhelpers"
```

Get a specific helper:
```bash
curl -k -X GET "https://localhost:6051/api/customhelpers/1"
```

Update a helper:
```bash
curl -k -X PUT "https://localhost:6051/api/customhelpers/1" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "name": "repeat",
    "description": "Repeats a string n times",
    "functionBody": "if (args.Length < 2) return string.Empty; var text = args[0]; if (!int.TryParse(args[1], out var count)) return text; return string.Concat(Enumerable.Repeat(text, count));",
    "isEnabled": true
  }'
```

Delete a helper:
```bash
curl -k -X DELETE "https://localhost:6051/api/customhelpers/1"
``` 