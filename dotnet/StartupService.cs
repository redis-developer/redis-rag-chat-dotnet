using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.KernelMemory;

namespace sk_webapi;

public class StartupService : IHostedService
{
    private readonly IKernelMemory _kernelMemory;
    private readonly IConfiguration _configuration;

    public StartupService(IKernelMemory kernelMemory, IConfiguration configuration)
    {
        _kernelMemory = kernelMemory;
        _configuration = configuration;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        return;
        var beerFile = _configuration["BeersFile"];
        if (string.IsNullOrEmpty(beerFile))
        {
            throw new ArgumentException("BeersFile must be specified in configuration.");
        }

        using var fs = new FileStream(beerFile, FileMode.Open, FileAccess.Read);
        using var doc = JsonDocument.Parse(fs);
        var i = 0;
        if (doc.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in doc.RootElement.EnumerateArray())
            {
                var beer = JsonSerializer.Deserialize<Beer>(element.GetRawText(), new JsonSerializerOptions(){PropertyNameCaseInsensitive = true});
                if (beer is null || string.IsNullOrEmpty(beer.Description))
                {
                    continue;
                }

                var txt = $"Name: {beer.Name}\nDescription: {beer.Description}";
                await _kernelMemory.ImportTextAsync(txt, cancellationToken:cancellationToken);
                // await Task.Delay(50, cancellationToken);
                if (i++ > 500)
                {
                    break;
                }
            }
        }
        else
        {
            throw new ArgumentException("BeersFile root element must be an array");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}