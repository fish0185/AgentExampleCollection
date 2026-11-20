using Anthropic;
using Anthropic.Core;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

var client = new AnthropicClient(new ClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("Anthropic_Demo_Api_Key")
});

await using McpClient treeholeMcp = await McpClient.CreateAsync(
    new HttpClientTransport(new()
    {
        Endpoint = new("https://treehole.online/mcp"),
        AdditionalHeaders =  new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {Environment.GetEnvironmentVariable("Treehole_Mcp_Api_Key")}"
        }
    }));

var mcpTools = await treeholeMcp.ListToolsAsync().ConfigureAwait(false);

var agent = client.AsAIAgent(
    new ChatClientAgentOptions
    {
        Name =  "example",
        ChatOptions = new()
        {
            ModelId = "claude-sonnet-5-5",
            Instructions  = "You are a friendly and knowledgeable assistant participating in a group discussion. \nYour role is to provide helpful, accurate, and concise information when addressed. \nMaintain a respectful tone, ensure your responses are clear and relevant to the group's ongoing conversation, and assist in facilitating productive discussions. \nMessages from users will be in the format 'UserName: chat messages'. \nPay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly.",
            Tools = [..mcpTools.Cast<AITool>()]
        },
    } 
);
    
var response = await agent.RunAsync("What's the weather of Sydney today?");
Console.WriteLine(response.Text);