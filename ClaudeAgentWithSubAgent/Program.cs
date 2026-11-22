using System.ComponentModel;
using Anthropic;
using Anthropic.Core;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
{
    return $"The weather in {location} is cloudy with a high of 15°C.";
}

var client = new AnthropicClient(new ClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("Anthropic_Demo_Api_Key")
});

var weatherSearchAgent = client.AsAIAgent(
    new ChatClientAgentOptions
    {
        Name =  "weather-search",
        ChatOptions = new()
        {
            ModelId = "claude-sonnet-5-5",
            Instructions  = "You are a friendly and knowledgeable assistant participating in a group discussion. \nYour role is to provide helpful, accurate, and concise information when addressed. \nMaintain a respectful tone, ensure your responses are clear and relevant to the group's ongoing conversation, and assist in facilitating productive discussions. \nMessages from users will be in the format 'UserName: chat messages'. \nPay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly.",
            Tools = [AIFunctionFactory.Create(GetWeather)]
        },
    } 
);

#pragma warning disable MAAI001
var backgroundProvider = new BackgroundAgentsProvider([weatherSearchAgent]);
#pragma warning restore MAAI001

var agent = client.AsAIAgent(
    new ChatClientAgentOptions
    {
        Name =  "example",
        ChatOptions = new()
        {
            ModelId = "claude-sonnet-5-5",
            Instructions  = "You are a friendly and knowledgeable assistant participating in a group discussion. \nYour role is to provide helpful, accurate, and concise information when addressed. \nMaintain a respectful tone, ensure your responses are clear and relevant to the group's ongoing conversation, and assist in facilitating productive discussions. \nMessages from users will be in the format 'UserName: chat messages'. \nPay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly.",
        },
        AIContextProviders = [backgroundProvider]
    } 
);
    
var response = await agent.RunAsync("what is weather of Sydney");
Console.WriteLine(response.Text);