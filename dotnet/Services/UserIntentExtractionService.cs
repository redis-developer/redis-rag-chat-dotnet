using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace sk_webapi.Services;

public class UserIntentExtractionService : IUserIntentExtractionService
{
    private readonly ISummarizationService _summarizationService;
    private readonly Kernel _kernel;

    public UserIntentExtractionService(Kernel kernel, ISummarizationService summarizationService)
    {
        _kernel = kernel;
        _summarizationService = summarizationService;
    }


    public async Task<string> GetUserIntent(string input, string chatId)
    {
        var summary = await _summarizationService.Summarize(chatId);
        
        var args = new KernelArguments()
        {
            ["input"] = input,
            ["summary"] = summary
        }; 
        if (!_kernel.Plugins.TryGetPlugin("Intent", out var intentPlugin))
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "Intent");
            intentPlugin = _kernel.ImportPluginFromPromptDirectory(dir);
        }
        
        var result = await _kernel.InvokeAsync(intentPlugin["IntentExtraction"], args);
        return result.ToString();
    }
}