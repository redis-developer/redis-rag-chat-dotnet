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

        var intentPlugin = _kernel.Plugins["Intent"];
        
        var result = await _kernel.InvokeAsync(intentPlugin["IntentExtraction"], args);
        return result.ToString();
    }
}