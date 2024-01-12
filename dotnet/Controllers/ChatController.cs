using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using sk_webapi.Services;

namespace sk_webapi;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatMessageService _chatMessageService;
    private readonly ICompletionService _completionService;
    private readonly IConfiguration _configuration;

    public ChatController(IChatMessageService chatMessageService, ICompletionService completionService, IConfiguration configuration)
    {
        _chatMessageService = chatMessageService;
        _completionService = completionService;
        _configuration = configuration;
    }

    [HttpPost("{chatId}")]
    public async Task<IActionResult> ChatAsync([FromBody]Ask ask, [FromRoute] string chatId )
    {
        var userChatMessage = new ChatMessage
        {
            Timestamp = DateTimeOffset.Now,
            ChatId = chatId,
            Message = ask.Prompt,
            AuthorRole = AuthorRole.User
        };

        await _chatMessageService.AddMessageAsync(userChatMessage);
        var res = await _completionService.GetLLMResponse(userChatMessage, chatId);
        return Ok(new {message= res.Message});
    }

    [HttpPost("startChat")]
    public async Task<IActionResult> StartChat()
    {
        var initialMessage = new ChatMessage()
        {
            AuthorRole = AuthorRole.System,
            ChatId = Ulid.NewUlid().ToString(),
            Message = _configuration["InitialMessage"],
            Timestamp = DateTimeOffset.Now
        };
        await _chatMessageService.AddMessageAsync(initialMessage);
        return Ok(initialMessage);
    }
}