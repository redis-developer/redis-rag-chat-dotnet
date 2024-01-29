namespace sk_webapi.Services;

public interface IUserIntentExtractionService
{
    public Task<string> GetUserIntent(string input, string chatId);
}