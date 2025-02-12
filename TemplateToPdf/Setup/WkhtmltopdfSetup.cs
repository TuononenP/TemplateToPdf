using System.Runtime.InteropServices;
using Serilog;
using TemplateToPdf.Interfaces;
using TemplateToPdf.Services;
using Wkhtmltopdf.NetCore;

namespace TemplateToPdf.Setup;

public static class WkhtmltopdfSetup
{
    private static bool IsLinuxDistro(string distroId)
    {
        try
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return false;
            var osRelease = File.ReadAllText("/etc/os-release");
            return osRelease.Contains($"ID={distroId}");
        }
        catch
        {
            return false;
        }
    }

    public static void ConfigureWkhtmltopdf(WebApplicationBuilder builder)
    {
        var binariesPath = Path.Combine(AppContext.BaseDirectory, "wkhtmltopdf");
        string wkhtmltopdfPath;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            wkhtmltopdfPath = Path.Combine(binariesPath, "win-x64", "wkhtmltopdf.exe");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            wkhtmltopdfPath = Path.Combine(binariesPath, "osx-x64", "wkhtmltopdf");
        }
        else
        {
            // Linux-specific paths
            var linuxPath = IsLinuxDistro("alpine") 
                ? Path.Combine(binariesPath, "linux-alpine", "wkhtmltopdf")
                : IsLinuxDistro("ubuntu") || IsLinuxDistro("debian")
                    ? Path.Combine(binariesPath, "linux-debian", "wkhtmltopdf")
                    : Path.Combine(binariesPath, "linux-x64", "wkhtmltopdf");
            
            wkhtmltopdfPath = linuxPath;
        }

        // Ensure the binary exists and is executable
        if (!File.Exists(wkhtmltopdfPath))
        {
            var error = $"wkhtmltopdf binary not found at {wkhtmltopdfPath}. Please ensure the binaries are included in the project.";
            Log.Error(error);
            throw new FileNotFoundException(error);
        }

        // Make binary executable on Unix systems
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var chmodProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"+x {wkhtmltopdfPath}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            chmodProcess.Start();
            chmodProcess.WaitForExit();
        }

        Log.Information("Using wkhtmltopdf from: {Path}", wkhtmltopdfPath);
        builder.Services.AddWkhtmltopdf(wkhtmltopdfPath);
        builder.Services.AddScoped<IHtmlToPdfConverter, WkhtmlToPdfConverter>();
    }
} 