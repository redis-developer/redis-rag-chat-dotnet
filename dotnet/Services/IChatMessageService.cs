namespace sk_webapi.Services;

public interface IChatMessageService
{
    Task AddMessageAsync(ChatMessage message);
    Task<ChatMessage?> GetMessageAsync(string messageId);
    Task<IList<ChatMessage>> GetMessagesForChatAsync(string chatId);
}