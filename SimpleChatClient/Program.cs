using Anthropic;
using Anthropic.Core;
using Microsoft.Extensions.AI;

AnthropicClient client = new AnthropicClient(new ClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("Anthropic_Demo_Api_Key")
});

IChatClient chatClient = client.AsIChatClient("claude-haiku-4-5")
    .AsBuilder()
    .UseFunctionInvocation() // Optional middleware
    .Build();
    
var result = await chatClient.GetResponseAsync("what is dotnet core");
Console.WriteLine(result.Text);