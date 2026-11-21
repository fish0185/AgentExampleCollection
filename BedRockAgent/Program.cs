using Amazon;
using Amazon.BedrockRuntime;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

IAmazonBedrockRuntime runtime = new AmazonBedrockRuntimeClient(
    "Access_Key", 
    "Secret_Key", 
    RegionEndpoint.APSoutheast2
);
        
// Get the Microsoft.Extensions.AI abstraction
IChatClient client = runtime.AsIChatClient("global.anthropic.claude-opus-4-5-20251101-v1:0");

var agent = client.AsAIAgent(
    new ChatClientAgentOptions
    {
        Name =  "example",
        ChatOptions = new()
        {
            Instructions  = "You are a friendly and knowledgeable assistant participating in a group discussion. \nYour role is to provide helpful, accurate, and concise information when addressed. \nMaintain a respectful tone, ensure your responses are clear and relevant to the group's ongoing conversation, and assist in facilitating productive discussions. \nMessages from users will be in the format 'UserName: chat messages'. \nPay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly.",
        }
    } 
);

var response = await agent.RunAsync("Hello from Bedrock!");
Console.WriteLine(response.Text);