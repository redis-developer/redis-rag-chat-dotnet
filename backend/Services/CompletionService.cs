using System.Text;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace sk_webapi.Services;

public class CompletionService : ICompletionService
{
    private readonly Kernel _kernel;
    private readonly IConfiguration _configuration;
    private readonly IUserIntentExtractionService _userIntentExtraction;
    private static readonly DateTime _knowledgeCutoff = new (2021, 12, 31, 23, 59, 59);

    public CompletionService(Kernel kernel, IConfiguration configuration, IUserIntentExtractionService userIntentExtraction)
    {
        _kernel = kernel;
        _configuration = configuration;
        _userIntentExtraction = userIntentExtraction;
    }

    public async Task<ChatMessage> GetLLMResponse(ChatMessage currentMessage, string chatId)
    {
        var intent = await _userIntentExtraction.GetUserIntent(currentMessage.Message!, chatId);

        var chatPlugin = _kernel.Plugins["Chat"];
        var kernelArguments = new KernelArguments() { ["knowledgeCutoff"] = _knowledgeCutoff, ["SystemPrompt"] = _configuration["SystemPrompt"]!, ["Intent"] = intent };
        var res = await _kernel.InvokeAsync(chatPlugin["CompleteChatMessage"], kernelArguments);
        return new ChatMessage()
        {
            AuthorRole = AuthorRole.Assistant,
            ChatId = chatId,
            Message = res.ToString(),
            Timestamp = DateTimeOffset.Now
        };
    }
}