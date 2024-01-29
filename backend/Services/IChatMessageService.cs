namespace sk_webapi.Services;

public interface IChatMessageService
{
    Task AddMessageAsync(ChatMessage message);
    Task<string> GetFormattedMessageHistoryAsync(string chatId);
}