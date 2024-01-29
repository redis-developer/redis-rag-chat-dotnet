using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace sk_webapi.Services;

public class UserIntentExtractionService : IUserIntentExtractionService
{
    private readonly Kernel _kernel;

    public UserIntentExtractionService(Kernel kernel)
    {
        _kernel = kernel;
    }


    public async Task<string> GetUserIntent(string input, string chatId)
    {
        var args = new KernelArguments()
        {
            ["input"] = input,
            ["chatId"] = chatId
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