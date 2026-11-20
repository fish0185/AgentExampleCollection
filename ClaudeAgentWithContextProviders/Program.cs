using Anthropic;
using Anthropic.Core;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Compaction;
using Microsoft.Extensions.AI;

var client = new AnthropicClient(new ClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("Anthropic_Demo_Api_Key")
});

var summarizerChatClient = client.AsIChatClient("claude-opus-5")
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

#pragma warning disable MAAI001
PipelineCompactionStrategy pipeline = new(
    new ToolResultCompactionStrategy(CompactionTriggers.TokensExceed(4000)),
    new SummarizationCompactionStrategy(summarizerChatClient, CompactionTriggers.TokensExceed(10000)),
    new SlidingWindowCompactionStrategy(CompactionTriggers.TurnsExceed(10)),
    new TruncationCompactionStrategy(CompactionTriggers.TokensExceed(200000)));
#pragma warning restore MEAI001
var compactionProvider = new CompactionProvider(pipeline);

var agent = client.AsAIAgent(
    new ChatClientAgentOptions
    {
        Name =  "example",
        ChatOptions = new()
        {
            ModelId = "claude-sonnet-5-5",
            Instructions  = "You are a friendly and knowledgeable assistant participating in a group discussion. \nYour role is to provide helpful, accurate, and concise information when addressed. \nMaintain a respectful tone, ensure your responses are clear and relevant to the group's ongoing conversation, and assist in facilitating productive discussions. \nMessages from users will be in the format 'UserName: chat messages'. \nPay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly."
        },
        AIContextProviders = [compactionProvider]
    } 
);
    
var response = await agent.RunAsync("What's the max token I can pass to you?");
Console.WriteLine(response.Text);