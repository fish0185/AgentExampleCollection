using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

var openAiClient = new OpenAIClient(Environment.GetEnvironmentVariable("OpenAi_Demo_Api_Key"));

var agent = openAiClient.GetChatClient("gpt-6-sol").AsAIAgent(new ChatClientAgentOptions
{
    Name = "ArtDirector",
    ChatOptions = new ChatOptions
    {
        Instructions = "Use the GenerateImageAsync tool whenever the user asks to create or draw an image.",
        Reasoning = new ReasoningOptions
        {
            Effort = ReasoningEffort.None
        }
    }
});

var result = await agent.RunAsync("do you know how to generate an image?");
Console.WriteLine(result);