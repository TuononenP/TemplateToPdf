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