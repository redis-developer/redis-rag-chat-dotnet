namespace sk_webapi.Services;

public interface ISummarizationService
{
    Task<string> Summarize(string chatId);
}