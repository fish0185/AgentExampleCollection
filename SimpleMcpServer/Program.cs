using SimpleMcpServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMcpServer()
    .WithHttpTransport(options =>
    {
        // Recommended for servers that don't need server-to-client requests.
        options.Stateless = true;
    })
    .WithToolsFromAssembly(typeof(WebTool).Assembly);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseWhen(context => context.Request.Path.StartsWithSegments("/mcp"), appBuilder =>
{
    appBuilder.UseMiddleware<BearerTokenMiddleware>();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapMcp("/mcp");

app.Run();