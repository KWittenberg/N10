namespace N10.Models;

public class AiResult
{
    public string Title { get; set; } = string.Empty;

    public string EnhancedContent { get; set; } = string.Empty;

    public int SuggestedTypeId { get; set; }

    public string? InternalNote { get; set; }



    public string? ModelName { get; set; }

    public int? PromptTokens { get; set; }

    public int? ResponseTokens { get; set; }

    public string? Duration { get; set; }

    public int? TotalTokens { get; set; }
}