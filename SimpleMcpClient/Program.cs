using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

var clientTransport = new HttpClientTransport(new HttpClientTransportOptions
{
    Name = "Everything",
    Endpoint = new Uri("http://localhost:5179/mcp"),
    AdditionalHeaders = new Dictionary<string, string>
    {
        { "Authorization", "Bearer your-secret" },
    }
});

var client = await McpClient.CreateAsync(clientTransport);

// Print the list of tools available from the server.
foreach (var tool in await client.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}

// Execute a tool (this would normally be driven by LLM tool invocations).
var result = await client.CallToolAsync(
    "get_html_by_url",
    new Dictionary<string, object?>() { ["url"] = "https://www.abc.net.au/" },
    cancellationToken: CancellationToken.None);

// echo always returns one and only one text content object
Console.WriteLine(result.Content.OfType<TextContentBlock>().First().Text);