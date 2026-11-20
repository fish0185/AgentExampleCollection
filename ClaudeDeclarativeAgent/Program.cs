using Anthropic;
using Anthropic.Core;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var client = new AnthropicClient(new ClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("Anthropic_Demo_Api_Key")
});

IChatClient chatClient = client.AsIChatClient("claude-haiku-4-5")
    .AsBuilder()
    .UseFunctionInvocation() // Optional middleware
    .Build();

var yamlDefinition =
    """
    kind: Prompt
    name: Assistant
    description: Helpful assistant
    instructions: You are a helpful assistant. You answer questions in the language specified by the user. You return your answers in a JSON format.
    model:
        options:
            temperature: 0.9
    outputSchema:
        properties:
            language:
                type: string
                required: true
                description: The language of the answer.
            answer:
                type: string
                required: true
                description: The answer text.
    """;

// Create the agent from the YAML definition.
var agentFactory = new ChatClientPromptAgentFactory(chatClient);
var agent = await agentFactory.CreateFromYamlAsync(yamlDefinition);

// Invoke the agent and output the text result.
Console.WriteLine(await agent!.RunAsync("Tell me a joke about a pirate in English."));

// Invoke the agent with streaming support.
// await foreach (var update in agent!.RunStreamingAsync("Tell me a joke about a pirate in French."))
// {
//     Console.WriteLine(update);
// }