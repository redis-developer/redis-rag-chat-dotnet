using Microsoft.SemanticKernel.ChatCompletion;
using Redis.OM.Modeling;

namespace sk_webapi;

[Document(StorageType = StorageType.Json)]
public class ChatMessage
{
    [Indexed]
    [RedisIdField]
    public string? Id { get; set; }

    [Indexed]
    public string? ChatId { get; set; }

    [Indexed]
    public string? Message { get; set; }

    [Indexed]
    public AuthorRole AuthorRole { get; set; }

    [Indexed]
    public DateTimeOffset Timestamp { get; set; }
}