namespace sk_webapi;

public static class TokenUtil
{
    private static readonly SharpToken.GptEncoding _tokenizer = SharpToken.GptEncoding.GetEncoding("cl100k_base");
    internal static int TokenCount(string? text) => _tokenizer.Encode(text).Count;
}