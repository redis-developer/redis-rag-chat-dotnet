using System.ComponentModel;
using Microsoft.SemanticKernel;
using sk_webapi.Services;

namespace sk_webapi.plugins;

public class ChatHistoryPlugin
{
    private readonly IChatMessageService _chatMessageService;

    public ChatHistoryPlugin(IChatMessageService chatMessageService)
    {
        _chatMessageService = chatMessageService;
    }

    [KernelFunction, Description("Get chat history formatted for the given chatId")]
    public async Task<string> GetChatHistoryAsync(string chatId)
    {
        return await _chatMessageService.GetFormattedMessageHistoryAsync(chatId);
    }
}