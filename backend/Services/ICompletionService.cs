namespace sk_webapi.Services;

public interface ICompletionService
{
    Task<ChatMessage> GetLLMResponse(ChatMessage currentMessage, string chatId);
}