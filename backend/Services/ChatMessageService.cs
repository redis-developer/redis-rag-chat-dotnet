using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using Redis.OM;

namespace sk_webapi.Services;

public class ChatMessageService : IChatMessageService
{
    private const string UserLine = "user:";
    private const string BotLine = "bot:";
    
    private readonly IRedisConnectionProvider _provider;

    public ChatMessageService(IRedisConnectionProvider provider)
    {
        _provider = provider;
    }

    private IRedisCollection<ChatMessage> Messages => _provider.RedisCollection<ChatMessage>();

    public Task AddMessageAsync(ChatMessage message) => Messages.InsertAsync(message);

    public async Task<string> GetFormattedMessageHistoryAsync(string chatId)
    {
        var messages = await Messages.Where(x => x.ChatId == chatId).ToListAsync();
        
        var sb = new StringBuilder();
        foreach (var message in messages)
        {
            if (message.AuthorRole == AuthorRole.User)
            {
                sb.AppendLine($"{UserLine} {message.Message}");
            }
            else
            {
                sb.AppendLine($"{BotLine} {message.Message}");
            }
        }

        return sb.ToString();
    }
}