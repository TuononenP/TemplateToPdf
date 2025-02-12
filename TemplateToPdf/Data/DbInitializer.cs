using Microsoft.EntityFrameworkCore;
using TemplateToPdf.Models;

namespace TemplateToPdf.Data;

public static class DbInitializer
{
    public static async Task Initialize(TemplateDbContext context)
    {
        // Create database if it doesn't exist
        await context.Database.EnsureCreatedAsync();

        // Check if the invoice template already exists
        if (!await context.Templates.AnyAsync(t => t.Id == 1))
        {
            var invoiceTemplate = new Template
            {
                Id = 1,
                Name = "Invoice Template",
                Content = @"<html>
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
                </html>",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Templates.Add(invoiceTemplate);
            await context.SaveChangesAsync();
        }
    }
} 