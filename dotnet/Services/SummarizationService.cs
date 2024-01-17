using Microsoft.SemanticKernel;

namespace sk_webapi.Services;

public class SummarizationService : ISummarizationService
{
    private readonly IChatMessageService _chatMessageService;
    private readonly Kernel _kernel;

    public SummarizationService(Kernel kernel, IChatMessageService chatMessageService)
    {
        _kernel = kernel;
        _chatMessageService = chatMessageService;
    }

    public async Task<string> Summarize(string chatId)
    {
        if (!_kernel.Plugins.TryGetPlugin("summarization", out var plugin))
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "summarization");
            plugin = _kernel.ImportPluginFromPromptDirectory(dir);
        }

        var history = await _chatMessageService.GetFormattedMessageHistoryAsync(chatId);
        
        var res = await _kernel.InvokeAsync(plugin["summarize"], new KernelArguments(){["history"] = history});

        return res.ToString();
    }
}