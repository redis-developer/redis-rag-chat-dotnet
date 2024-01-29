using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory;

namespace sk_webapi;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IKernelMemory _kernelMemory;

    public DocumentsController(IKernelMemory kernelMemory)
    {
        _kernelMemory = kernelMemory;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> AddDocument(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        await using var fileStream = file.OpenReadStream();
        var document = new Document().AddStream(file.FileName, fileStream);
        var res = await _kernelMemory.ImportDocumentAsync(document);
        return Ok(res);
    }
}