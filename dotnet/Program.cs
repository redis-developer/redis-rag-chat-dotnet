using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory.AI.OpenAI;
using Microsoft.KernelMemory.Configuration;
using Microsoft.KernelMemory.Handlers;
using Microsoft.KernelMemory.MemoryDb.Redis;
using Microsoft.KernelMemory.MemoryStorage;
using Microsoft.KernelMemory.Search;
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
builder.Services.AddOpenAITextEmbeddingGeneration(builder.Configuration["OpenAIEmbeddingGenerationModelId"]!, builder.Configuration["OpenAIApiKey"]!);
var muxer = ConnectionMultiplexer.Connect("localhost");

builder.Services.AddSingleton(new QueryConfiguration());
builder.Services.AddSingleton<IConnectionMultiplexer>(muxer);
builder.Services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(muxer));
builder.Services.AddSingleton<IChatMessageService>(
    sp => new ChatMessageService(sp.GetService<IRedisConnectionProvider>()!));
ILogger<OpenAITextGenerator>? log = null;

builder.Services.AddScoped<IMemoryDb>(sp  => 
    new RedisMemory(
        new RedisConfig(), 
        sp.GetService<IConnectionMultiplexer>()!,
        sp.GetService<ITextEmbeddingGenerator>()!)
);
builder.Services.AddScoped<ISearchClient>(sp => new SearchClient(sp.GetService<IMemoryDb>()!, sp.GetService<ITextGenerator>()!));

var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatCompletion(builder.Configuration["OpenAICompletionModelId"]!,
    builder.Configuration["OpenAIApiKey"]!);
var kernel = kernelBuilder.Build();
kernel.ImportPluginFromObject(new TimePlugin(), nameof(TimePlugin));


builder.Services.AddSingleton(kernel);
builder.Services.AddSingleton<ICompletionService>(sp=>
    new CompletionService(
        sp.GetService<IKernelMemory>()!, 
        sp.GetService<IChatMessageService>()!,
        sp.GetService<QueryConfiguration>()!,
        sp.GetService<Kernel>()!, 
        sp.GetService<IConfiguration>()!));
builder.Services.AddScoped(sp => new MemoryService(null, sp.GetService<ISearchClient>()!));
builder.Services.AddOpenAIChatCompletion(builder.Configuration["OpenAICompletionModelId"]!, builder.Configuration["OpenAIApiKey"]!);

builder.Services.AddCors(options => options.AddDefaultPolicy(
    policy  =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
    }));

var kmBuilder = new KernelMemoryBuilder();

var memoryService = kmBuilder
    // .WithOpenAITextGeneration(openAiConfig)
    .WithOpenAIDefaults(builder.Configuration["OpenAIApiKey"]!)
    .WithSimpleQueuesPipeline()
    .With(new TextPartitioningOptions
    {
        MaxTokensPerLine = 255,
        MaxTokensPerParagraph = 255
    }).WithRedisMemoryDb(new RedisConfig(){ConnectionString = "localhost"})
    .Build<MemoryServerless>();

builder.Services.AddSingleton<IKernelMemory>(memoryService);
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

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary) {
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
