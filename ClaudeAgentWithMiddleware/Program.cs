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

// IChatClient middleware that logs requests and responses
async Task<ChatResponse> LoggingChatMiddleware(
    IEnumerable<ChatMessage> messages,
    ChatOptions? options,
    IChatClient innerChatClient,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"[ChatLog] Sending {messages.Count()} messages to model...");
    foreach (var msg in messages)
    {
        Console.WriteLine($"[ChatLog]   {msg.Role}: {msg.Text?.Substring(0, Math.Min(msg.Text.Length, 80))}");
    }

    var response = await innerChatClient.GetResponseAsync(messages, options, cancellationToken);

    Console.WriteLine($"[ChatLog] Received {response.Messages.Count} response messages.");
    return response;
}

async Task<AgentResponse> CustomAgentRunMiddleware(
    IEnumerable<ChatMessage> messages,
    AgentSession? session,
    AgentRunOptions? options,
    AIAgent innerAgent,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"Input: {messages.Count()}");
    var response = await innerAgent.RunAsync(messages, session, options, cancellationToken).ConfigureAwait(false);
    Console.WriteLine($"Output: {response.Messages.Count}");
    return response;
}


async ValueTask<object?> CustomFunctionCallingMiddleware(
    AIAgent agent,
    FunctionInvocationContext context,
    Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
    CancellationToken cancellationToken)
{
    Console.WriteLine($"Function Name: {context!.Function.Name}");
    var result = await next(context, cancellationToken);
    Console.WriteLine($"Function Call Result: {result}");

    return result;
}

var client = new AnthropicClient(new ClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("Anthropic_Demo_Api_Key")
});

var agent = client.AsAIAgent(
    new ChatClientAgentOptions
    {
        Name =  "example",
        ChatOptions = new()
        {
            ModelId = "claude-sonnet-5-5",
            Instructions  = "You are a friendly and knowledgeable assistant participating in a group discussion. \nYour role is to provide helpful, accurate, and concise information when addressed. \nMaintain a respectful tone, ensure your responses are clear and relevant to the group's ongoing conversation, and assist in facilitating productive discussions. \nMessages from users will be in the format 'UserName: chat messages'. \nPay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly.",
            Tools = [AIFunctionFactory.Create(GetWeather)]
        }
    },
    clientFactory: (chatClient) => chatClient
        .AsBuilder()
        .Use(getResponseFunc: LoggingChatMiddleware, getStreamingResponseFunc: null)
        .Build()
).AsBuilder()
.Use(runFunc: CustomAgentRunMiddleware, runStreamingFunc: null)
.Use(CustomFunctionCallingMiddleware)
.Build();
    
var response = await agent.RunAsync("what is weather of Sydney");
Console.WriteLine(response.Text);