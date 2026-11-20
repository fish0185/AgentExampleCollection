using Anthropic;
using Anthropic.Core;
using Microsoft.Agents.AI;

var apiCallSkill = new AgentInlineSkill(
        name: "api-call",
        description: "Enabling to fire a get request for random url",
        instructions: """
                      Use this skill when answering questions like start a remote api call
                      1. Read the api-call-guide resource for the full set of rules.
                      2. Answer based on those rules.
                      """)
    .AddResource(
        "api-call-guide",
        """
        # Api Call Guide
        - Set the url from user prompt
        - Call the script api-call-guide and return the raw response
        """)
    .AddScript("api-call-guide",
        async (string url) =>
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return "Error from api-call-guide";
                }
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error from api-call-guide";
            }
        });

var skills = new List<AgentSkill>
{
    apiCallSkill
};

var skillsProvider = new AgentSkillsProvider(skills, options: new AgentSkillsProviderOptions
{
    DisableLoadSkillApproval = true,
    DisableReadSkillResourceApproval = true,
    DisableRunSkillScriptApproval = true
});

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
            Instructions  = "You are a friendly and knowledgeable assistant participating in a group discussion. \nYour role is to provide helpful, accurate, and concise information when addressed. \nMaintain a respectful tone, ensure your responses are clear and relevant to the group's ongoing conversation, and assist in facilitating productive discussions. \nMessages from users will be in the format 'UserName: chat messages'. \nPay attention to the 'UserName' to understand who is speaking and tailor your responses accordingly."
        },
        AIContextProviders = [skillsProvider]
    }
);
    
var response = await agent.RunAsync("what is the response from api-call with url https://mock.httpstatus.codes/status/200");
Console.WriteLine(response.Text);