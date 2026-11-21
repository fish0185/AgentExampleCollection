using Amazon;
using Amazon.BedrockRuntime;
using Microsoft.Extensions.AI;

IAmazonBedrockRuntime runtime = new AmazonBedrockRuntimeClient(
    "Access_Key", 
    "Secret_Key", 
    RegionEndpoint.APSoutheast2
);
        
// Get the Microsoft.Extensions.AI abstraction
IChatClient client = runtime.AsIChatClient("global.anthropic.claude-opus-4-5-20251101-v1:0"); 

var response = await client.GetResponseAsync("Hello from Bedrock!");
Console.WriteLine(response.Text);