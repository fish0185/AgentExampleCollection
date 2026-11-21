using System.ComponentModel;
using System.Text;
using Microsoft.Playwright;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace SimpleMcpServer;

[McpServerToolType]
public class WebTool
{
    public WebTool(ILogger<WebTool> logger)
    {
    }
    
    [McpServerTool(Name = "get_html_by_url")]
    [Description("Retrieves html content from a url.")]
    public static async Task<CallToolResult> GetHtmlAsync(
        string url,
        CancellationToken cancellationToken)
    {
        var html = await DownloadHtmlViaPlaywright(url);

        return new CallToolResult
        {
            Content =
            [
                new TextContentBlock
                {
                    Text = html
                }
            ]
        };
    }

    private static async Task<string> DownloadHtmlViaPlaywright(string url)
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();
        
        // Navigate to the website and wait until network requests finish
        await page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        var html = await page.ContentAsync();
        
        return html;
    }

    [McpServerTool(Name = "get_pdf_by_url")]
    [Description("Generates or retrieves a PDF from a url.")]
    public static async Task<CallToolResult> GetReportPdfAsync(
        string url,
        CancellationToken cancellationToken)
    {
        var (pdfBytes, fileName) = await DownloadPageViaPlaywright(url);
        string base64Pdf = Convert.ToBase64String(pdfBytes);

        return new CallToolResult
        {
            Content =
            [
                new EmbeddedResourceBlock
                {
                    Resource = new BlobResourceContents
                    {
                        Uri = $"file:///{Path.GetFileName(fileName)}",
                        MimeType = "application/pdf",
                        Blob = Encoding.UTF8.GetBytes(base64Pdf)
                    }
                }
            ]
        };
    }
    
    private static async Task<(byte[], string)> DownloadPageViaPlaywright(string url)
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();
        
        // Navigate to the website and wait until network requests finish
        await page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        var fileName = $"{Guid.NewGuid()}.pdf";
        // Save the page as a PDF
        var bytes = await page.PdfAsync(new PagePdfOptions
        {
            Path = fileName,
            Format = "A4",
            PrintBackground = true
        });
        
        return (bytes, fileName);
    }
}