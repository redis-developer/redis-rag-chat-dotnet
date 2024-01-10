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
    private static readonly string SystemPrompt =
        "This is a chat between an intelligent AI bot named Redis and one participant. SK stands for Semantic Kernel, " +
        "the AI platform used to build the bot. The AI was trained on data through 2021 and is not aware of events that " +
        "have occurred since then. It also has no ability to access data on the Internet, so it should not claim that it can " +
        "or say that it will go and look things up. Try to be concise with your answers, though it is not required. " +
        "Knowledge cutoff: {{$knowledgeCutoff}} / Current date: {{TimePlugin.Now}}.\\n\\n Provide " +
        "a response to the last message.";
    
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IChatMessageService _chatMessageService;
    private readonly ICompletionService _completionService;
    private readonly IKernelMemory _kernelMemory;

    public ChatController(IChatCompletionService chatCompletionService, IKernelMemory kernelMemory, IChatMessageService chatMessageService, ICompletionService completionService)
    {
        _chatCompletionService = chatCompletionService;
        _kernelMemory = kernelMemory;
        _chatMessageService = chatMessageService;
        _completionService = completionService;
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
            Message = "Hello! Welcome to beer chat! How can I help you?",
            Timestamp = DateTimeOffset.Now
        };
        await _chatMessageService.AddMessageAsync(initialMessage);
        return Ok(initialMessage);
    }

    
    [HttpPost("addDoc")]
    public async Task<IActionResult> AddDocument([FromBody] DocumentInsertionRequest docRequest)
    {
        var res = await _kernelMemory.ImportDocumentAsync(new Document().AddFile(docRequest.FilePath));
        return Ok(res);
    }
}