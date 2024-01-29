using Microsoft.KernelMemory;
using Microsoft.KernelMemory.Configuration;
using Microsoft.SemanticKernel;
using Redis.OM;
using Redis.OM.Contracts;
using sk_webapi;
using sk_webapi.plugins;
using sk_webapi.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
var muxer = ConnectionMultiplexer.Connect("localhost");

builder.Services.AddSingleton<IConnectionMultiplexer>(muxer);
builder.Services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(muxer));
builder.Services.AddSingleton<IChatMessageService, ChatMessageService>();

var kmEndpoint = builder.Configuration["KernelMemoryEndpoint"];
if (string.IsNullOrEmpty(kmEndpoint))
{
    throw new ConfigurationException("No KernelMemoryEndpointDefined in Configuration");
}
    
var kernelMemory = new MemoryWebClient(kmEndpoint);
builder.Services.AddSingleton(kernelMemory);

var kernelBuilder = builder.Services.AddKernel();
kernelBuilder.AddOpenAIChatCompletion(builder.Configuration["OpenAICompletionModelId"]!,
    builder.Configuration["OpenAIApiKey"]!);

// plugins loaded from prompts
kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(Directory.GetCurrentDirectory(), "plugins", "Intent"));
kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(Directory.GetCurrentDirectory(), "plugins", "summarization"));
kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(Directory.GetCurrentDirectory(), "plugins", "Chat"));

// our custom object plugins
kernelBuilder.Plugins.AddFromType<ChatHistoryPlugin>(nameof(ChatHistoryPlugin));
kernelBuilder.Plugins.AddFromType<TimePlugin>(nameof(TimePlugin));

// our Kernel Memory Plugin
kernelBuilder.Plugins.AddFromObject(new MemoryPlugin(kernelMemory), "memory");

builder.Services.AddSingleton<ICompletionService, CompletionService>();
builder.Services.AddSingleton<IUserIntentExtractionService, UserIntentExtractionService>();

builder.Services.AddCors(options => options.AddDefaultPolicy(
    policy  =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
    }));

builder.Services.AddHostedService<IndexSetupService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();
