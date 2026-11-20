using Anthropic;
using Anthropic.Core;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Compaction;
using Microsoft.Extensions.AI;

var client = new AnthropicClient(new ClientOptions
{
    ApiKey = Environment.GetEnvironmentVariable("Anthropic_Demo_Api_Key")
});

TextSearchProviderOptions textSearchOptions = new()
{
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
};

var agent = client.AsAIAgent(
    new ChatClientAgentOptions
    {
        Name =  "example",
        ChatOptions = new()
        {
            ModelId = "claude-sonnet-5-5",
            Instructions  = "You are a friendly and knowledgeable assistant participating in a group discussion. \nYour role is to provide helpful, accurate, and concise information when addressed. \nMaintain a respectful tone, ensure your responses are clear and relevant to the group's ongoing conversation, and assist in facilitating productive discussions. \nMessages from users will be in the format 'UserName: chat messages'. \nPay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly."
        },
        AIContextProviders = [new TextSearchProvider(SearchAdapter, textSearchOptions)]
    } 
);
    
var response = await agent.RunAsync("how do i return my order");
Console.WriteLine(response.Text);

static Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string query, CancellationToken cancellationToken)
{
    // The mock search inspects the user's question and returns pre-defined snippets
    // that resemble documents stored in an external knowledge source.
    List<TextSearchProvider.TextSearchResult> results = new();

    if (query.Contains("return", StringComparison.OrdinalIgnoreCase) || query.Contains("refund", StringComparison.OrdinalIgnoreCase))
    {
        results.Add(new()
        {
            SourceName = "Contoso Outdoors Return Policy",
            SourceLink = "https://contoso.com/policies/returns",
            Text = "Customers may return any item within 30 days of delivery. Items should be unused and include original packaging. Refunds are issued to the original payment method within 5 business days of inspection."
        });
    }

    return Task.FromResult<IEnumerable<TextSearchProvider.TextSearchResult>>(results);
}