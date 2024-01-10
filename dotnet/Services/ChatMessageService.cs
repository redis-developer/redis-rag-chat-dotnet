using Redis.OM.Contracts;
using Redis.OM.Searching;
using Redis.OM;

namespace sk_webapi.Services;

public class ChatMessageService : IChatMessageService
{
    private readonly IRedisConnectionProvider _provider;

    public ChatMessageService(IRedisConnectionProvider provider)
    {
        _provider = provider;
    }

    private IRedisCollection<ChatMessage> Messages => _provider.RedisCollection<ChatMessage>();


    public Task AddMessageAsync(ChatMessage message) => Messages.InsertAsync(message);

    public Task<ChatMessage?> GetMessageAsync(string messageId) => Messages.FirstOrDefaultAsync(x => x.Id == messageId);

    public Task<IList<ChatMessage>> GetMessagesForChatAsync(string chatId) => Messages.Where(x => x.ChatId == chatId).ToListAsync();
}