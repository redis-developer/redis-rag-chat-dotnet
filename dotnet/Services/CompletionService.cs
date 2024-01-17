using System.Text;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace sk_webapi.Services;

public class CompletionService : ICompletionService
{
    private readonly IKernelMemory _kernelMemory;
    private readonly IChatMessageService _chatMessageService;
    private readonly QueryConfiguration _queryConfiguration;
    private readonly Kernel _kernel;
    private readonly IConfiguration _configuration;
    private readonly IUserIntentExtractionService _userIntentExtraction;

    private string SystemPrompt => _configuration["SystemPrompt"]!; 


    private const string ChatSectionPrompt = """
The following is the chat history between you and the user.
""";

    private const string MemorySectionPrompt = """
The following are relevant memories to the question asked that you may use in your responses.
""";

    private const string UserLine = "user:";
    private const string BotLine = "bot:";
    private const string MemoryLine = "memory:";

    private int SystemPromptTokenCount => TokenUtil.TokenCount(SystemPrompt);
    private static readonly int ChatSectionPromptTokenCount = TokenUtil.TokenCount(ChatSectionPrompt);
    private static readonly int MemorySectionPromptTokenCount = TokenUtil.TokenCount(MemorySectionPrompt);
    private static readonly int UserLineTokenCount = TokenUtil.TokenCount(UserLine);
    private static readonly int BotLineTokenCount = TokenUtil.TokenCount(BotLine);
    private static readonly int MemoryLineTokenCount = TokenUtil.TokenCount(MemoryLine);

    public CompletionService(IKernelMemory kernelMemory, IChatMessageService chatMessageService, QueryConfiguration queryConfiguration, Kernel kernel, IConfiguration configuration, IUserIntentExtractionService userIntentExtraction)
    {
        _kernelMemory = kernelMemory;
        _chatMessageService = chatMessageService;
        _queryConfiguration = queryConfiguration;
        _kernel = kernel;
        _configuration = configuration;
        _userIntentExtraction = userIntentExtraction;
    }

    public async Task<ChatMessage> GetLLMResponse(ChatMessage currentMessage, string chatId)
    {
        var function = await CraftPrompt(currentMessage, chatId);
        
        var res = await function.InvokeAsync(_kernel,
            new KernelArguments() { ["knowledgeCutoff"] = _queryConfiguration.KnowledgeCutoff });
        return new ChatMessage()
        {
            AuthorRole = AuthorRole.Assistant,
            ChatId = chatId,
            Message = res.ToString(),
            Timestamp = DateTimeOffset.Now
        };
    }

    private async Task<KernelFunction> CraftPrompt(ChatMessage currentMessage, string chatId)
    {
        var sb = new StringBuilder();
        sb.AppendLine(SystemPrompt);
        var messageTokenCount = TokenUtil.TokenCount(currentMessage.Message);
        var remainingBudget = _queryConfiguration.TokenBudget - messageTokenCount - SystemPromptTokenCount - ChatSectionPromptTokenCount - MemorySectionPromptTokenCount;
        var chatHistoryBudget = (int)Math.Floor(_queryConfiguration.HistoryPercentage * remainingBudget);
        var ragBudget = (int)Math.Floor(_queryConfiguration.RagPercentage * remainingBudget);

        var userIntent = await _userIntentExtraction.GetUserIntent(currentMessage.Message!, chatId);

        var priorMessages = (await _chatMessageService.GetMessagesForChatAsync(chatId)).Where(x=>x.Id != currentMessage.Id).OrderByDescending(x=>x.Timestamp);
        var relevantDocuments = (await _kernelMemory.SearchAsync(userIntent, limit: 5)).Results.OrderByDescending(x=>x.Partitions.First().Relevance);

        sb.AppendLine(ChatSectionPrompt);
        foreach (var message in priorMessages)
        {
            chatHistoryBudget -= (TokenUtil.TokenCount(message.Message) + (message.AuthorRole == AuthorRole.User ? UserLineTokenCount : BotLineTokenCount));
            if (chatHistoryBudget < 0)
            {
                break;
            }
            
            if (message.AuthorRole == AuthorRole.User)
            {
                sb.AppendLine($"{UserLine} {message.Message}");
            }
            else
            {
                sb.AppendLine($"{BotLine} {message.Message}");
            }
        }

        sb.AppendLine(MemorySectionPrompt);
        foreach (var citation in relevantDocuments)
        {
            var fullMemoryText = string.Join(' ', citation.Partitions.Select(x => x.Text));
            ragBudget -= (TokenUtil.TokenCount(fullMemoryText) + MemoryLineTokenCount);

            if (ragBudget < 0)
            {
                break;
            }
            
            sb.AppendLine($"{MemoryLine} {fullMemoryText}");
        }

        sb.AppendLine(
            $"Respond to the following questions using the information and history above: {currentMessage.Message}");

        return _kernel.CreateFunctionFromPrompt(sb.ToString());
        
    }
}