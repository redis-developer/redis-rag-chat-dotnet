using Microsoft.KernelMemory;
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
builder.Services.AddSingleton<IChatMessageService>(
    sp => new ChatMessageService(sp.GetService<IRedisConnectionProvider>()!));



var kernelBuilder = builder.Services.AddKernel();
kernelBuilder.AddOpenAIChatCompletion(builder.Configuration["OpenAICompletionModelId"]!,
    builder.Configuration["OpenAIApiKey"]!);
kernelBuilder.Plugins.AddFromType<ChatHistoryPlugin>(nameof(ChatHistoryPlugin));
kernelBuilder.Plugins.AddFromObject(new TimePlugin(), nameof(TimePlugin));
kernelBuilder.Plugins.AddFromPromptDirectory(Path.Combine(Directory.GetCurrentDirectory(), "plugins", "summarization"));
builder.Services.AddSingleton<ICompletionService, CompletionService>();
builder.Services.AddSingleton<IUserIntentExtractionService, UserIntentExtractionService>();

builder.Services.AddCors(options => options.AddDefaultPolicy(
    policy  =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
    }));

var kmEndpoint = builder.Configuration["KernelMemoryEndpoint"];
IKernelMemory kernelMemory;
if (!string.IsNullOrEmpty(kmEndpoint))
{
    kernelMemory = new MemoryWebClient(kmEndpoint);
}
else
{
    kernelMemory = new KernelMemoryBuilder()
        .WithOpenAIDefaults(builder.Configuration["OpenAIApiKey"]!)
        .WithSimpleQueuesPipeline()
        .WithRedisMemoryDb(new RedisConfig(){ConnectionString = "localhost"})
        .Build<MemoryServerless>();
}

builder.Services.AddSingleton(kernelMemory);
kernelBuilder.Plugins.AddFromObject(new MemoryPlugin(kernelMemory), "memory");

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
