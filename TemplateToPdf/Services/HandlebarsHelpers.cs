using HandlebarsDotNet;
using System.Globalization;

namespace TemplateToPdf.Services;

public static class HandlebarsHelpers
{
    public static void RegisterHelpers(IHandlebars handlebars)
    {
        // String helpers
        handlebars.RegisterHelper("uppercase", (context, arguments) => 
            arguments[0]?.ToString()?.ToUpper() ?? string.Empty);
            
        handlebars.RegisterHelper("lowercase", (context, arguments) => 
            arguments[0]?.ToString()?.ToLower() ?? string.Empty);
            
        handlebars.RegisterHelper("substring", (context, arguments) =>
        {
            var str = arguments[0]?.ToString() ?? string.Empty;
            if (arguments.Length < 2) return str;
            
            var start = Convert.ToInt32(arguments[1]);
            var length = arguments.Length > 2 ? Convert.ToInt32(arguments[2]) : str.Length - start;
            return str.Substring(start, Math.Min(length, str.Length - start));
        });

        // Number helpers
        handlebars.RegisterHelper("formatNumber", (context, arguments) =>
        {
            if (arguments.Length < 1) return "0";
            var format = arguments.Length > 1 ? arguments[1]?.ToString() : "N2";
            return decimal.TryParse(arguments[0]?.ToString(), out var number) 
                ? number.ToString(format) 
                : "0";
        });

        handlebars.RegisterHelper("formatCurrency", (context, arguments) =>
        {
            if (arguments.Length < 1) return "$0.00";
            var culture = arguments.Length > 1 
                ? new CultureInfo(arguments[1]?.ToString() ?? "en-US")
                : new CultureInfo("en-US");
            
            return decimal.TryParse(arguments[0]?.ToString(), out var number)
                ? number.ToString("C", culture)
                : "$0.00";
        });

        // Math helpers
        handlebars.RegisterHelper("sum", (context, arguments) =>
        {
            decimal sum = 0;
            foreach (var arg in arguments)
            {
                if (decimal.TryParse(arg?.ToString(), out var number))
                    sum += number;
            }
            return sum;
        });

        handlebars.RegisterHelper("multiply", (context, arguments) =>
        {
            if (arguments.Length < 2) return 0;
            if (!decimal.TryParse(arguments[0]?.ToString(), out var a) ||
                !decimal.TryParse(arguments[1]?.ToString(), out var b))
                return 0;
            return a * b;
        });

        handlebars.RegisterHelper("divide", (context, arguments) =>
        {
            if (arguments.Length < 2) return 0;
            if (!decimal.TryParse(arguments[0]?.ToString(), out var a) ||
                !decimal.TryParse(arguments[1]?.ToString(), out var b) ||
                b == 0)
                return 0;
            return a / b;
        });

        handlebars.RegisterHelper("round", (context, arguments) =>
        {
            if (arguments.Length < 1) return 0;
            if (!decimal.TryParse(arguments[0]?.ToString(), out var number))
                return 0;
            var decimals = arguments.Length > 1 && int.TryParse(arguments[1]?.ToString(), out var d) ? d : 0;
            return Math.Round(number, decimals);
        });

        // Array helpers
        handlebars.RegisterHelper("length", (context, arguments) =>
        {
            if (arguments[0] is System.Collections.IEnumerable enumerable)
            {
                return enumerable.Cast<object>().Count();
            }
            return 0;
        });

        handlebars.RegisterHelper("join", (context, arguments) =>
        {
            if (arguments.Length < 1) return string.Empty;
            var separator = arguments.Length > 1 ? arguments[1]?.ToString() ?? "," : ",";
            
            if (arguments[0] is System.Collections.IEnumerable enumerable)
            {
                return string.Join(separator, enumerable.Cast<object>());
            }
            return string.Empty;
        });

        // Conditional helpers
        handlebars.RegisterHelper("eq", (context, arguments) =>
        {
            if (arguments.Length < 2) return false;
            return arguments[0]?.ToString() == arguments[1]?.ToString();
        });

        handlebars.RegisterHelper("neq", (context, arguments) =>
        {
            if (arguments.Length < 2) return true;
            return arguments[0]?.ToString() != arguments[1]?.ToString();
        });

        handlebars.RegisterHelper("gt", (context, arguments) =>
        {
            if (arguments.Length < 2) return false;
            if (!decimal.TryParse(arguments[0]?.ToString(), out var a) ||
                !decimal.TryParse(arguments[1]?.ToString(), out var b))
                return false;
            return a > b;
        });

        handlebars.RegisterHelper("lt", (context, arguments) =>
        {
            if (arguments.Length < 2) return false;
            if (!decimal.TryParse(arguments[0]?.ToString(), out var a) ||
                !decimal.TryParse(arguments[1]?.ToString(), out var b))
                return false;
            return a < b;
        });

        handlebars.RegisterHelper("gte", (context, arguments) =>
        {
            if (arguments.Length < 2) return false;
            if (!decimal.TryParse(arguments[0]?.ToString(), out var a) ||
                !decimal.TryParse(arguments[1]?.ToString(), out var b))
                return false;
            return a >= b;
        });

        handlebars.RegisterHelper("lte", (context, arguments) =>
        {
            if (arguments.Length < 2) return false;
            if (!decimal.TryParse(arguments[0]?.ToString(), out var a) ||
                !decimal.TryParse(arguments[1]?.ToString(), out var b))
                return false;
            return a <= b;
        });

        // Date helpers (extending the existing formatDate)
        handlebars.RegisterHelper("now", (context, arguments) =>
        {
            var format = arguments.Length > 0 ? arguments[0]?.ToString() : "yyyy-MM-dd";
            return DateTime.Now.ToString(format);
        });

        handlebars.RegisterHelper("addDays", (context, arguments) =>
        {
            if (arguments.Length < 2) return string.Empty;
            if (!DateTime.TryParse(arguments[0]?.ToString(), out var date) ||
                !int.TryParse(arguments[1]?.ToString(), out var days))
                return string.Empty;
            
            var format = arguments.Length > 2 ? arguments[2]?.ToString() : "yyyy-MM-dd";
            return date.AddDays(days).ToString(format);
        });
    }
} 