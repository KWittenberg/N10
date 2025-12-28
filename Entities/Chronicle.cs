namespace N10.Entities;

public class Chronicle : BaseEntity
{
    public byte? Day { get; set; }
    public byte? Month { get; set; }
    public int? Year { get; set; }


    public DateOnly? Date { get; set; }

    public string? Title { get; set; }

    public required string Content { get; set; } = string.Empty;

    public string? EnhancedContent { get; set; }

    public string? Note { get; set; }

    public ChronicleType Type { get; set; } = ChronicleType.Unspecified;

    public bool IsApproved { get; set; } = false;
}






public class SocialPost : BaseAuditableEntity
{
    public int ChronicleId { get; set; }

    public virtual Chronicle? Chronicle { get; set; }

    public string TextForPublish { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; }
}