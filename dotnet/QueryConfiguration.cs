namespace sk_webapi;

public class QueryConfiguration
{
    /// <summary>
    /// Percentage of remaining token budget (after prompt) that can be allocated to chat history
    /// </summary>
    public double HistoryPercentage { get; set; } = .5;
    
    /// <summary>
    /// Percentage of remaining token budget (after prompt) that can be allocated to Retrieval
    /// </summary>
    public double RagPercentage { get; set; } = .5;

    /// <summary>
    /// The number of tokens we can use per request.
    /// </summary>
    public int TokenBudget { get; set; } = 4096;

    public DateTime KnowledgeCutoff { get; set; } = new DateTime(2021, 12, 31, 23, 59, 59);
}