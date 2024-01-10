using Redis.OM;
using Redis.OM.Contracts;

namespace sk_webapi;

public class IndexSetupService : IHostedService
{
    private readonly IRedisConnectionProvider _provider;

    public IndexSetupService(IRedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _provider.Connection.DropIndexAndAssociatedRecords(typeof(ChatMessage));
        await _provider.Connection.CreateIndexAsync(typeof(ChatMessage));
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}