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

builder.Services.AddSingleton(new QueryConfiguration());
builder.Services.AddSingleton<IConnectionMultiplexer>(muxer);
builder.Services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(muxer));
builder.Services.AddSingleton<IChatMessageService>(
    sp => new ChatMessageService(sp.GetService<IRedisConnectionProvider>()!));


var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddOpenAIChatCompletion(builder.Configuration["OpenAICompletionModelId"]!,
    builder.Configuration["OpenAIApiKey"]!);
var kernel = kernelBuilder.Build();
kernel.ImportPluginFromObject(new TimePlugin(), nameof(TimePlugin));


builder.Services.AddSingleton(kernel);
builder.Services.AddSingleton<ICompletionService, CompletionService>();
builder.Services.AddSingleton<ISummarizationService, SummarizationService>();
builder.Services.AddSingleton<IUserIntentExtractionService, UserIntentExtractionService>();

builder.Services.AddCors(options => options.AddDefaultPolicy(
    policy  =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
    }));

var kmBuilder = new KernelMemoryBuilder();

var kmEndpoint = builder.Configuration["KernelMemoryEndpoint"];
if (!string.IsNullOrEmpty(kmEndpoint))
{
    builder.Services.AddSingleton<IKernelMemory>(new MemoryWebClient(kmEndpoint));
}
else
{
    var memoryService = kmBuilder
        .WithOpenAIDefaults(builder.Configuration["OpenAIApiKey"]!)
        .WithSimpleQueuesPipeline()
        .WithRedisMemoryDb(new RedisConfig(){ConnectionString = "localhost"})
        .Build<MemoryServerless>();

    builder.Services.AddSingleton<IKernelMemory>(memoryService);
}

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
