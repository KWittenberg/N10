namespace N10.Entities;

public class Chronicle : BaseEntity
{
    public DateOnly? Date { get; set; }

    public string? Title { get; set; }

    public required string Content { get; set; } = string.Empty;

    public ChronicleType Type { get; set; } = ChronicleType.Unspecified;
}


public enum ChronicleType
{
    Unspecified = 0, // Default dok ne znamo
    Historical = 1,  // Događaji (bitke, osnivanja)
    Biography = 2,   // Rođenja/Smrti (Trenk, Kučera)
    Religious = 3,   // Župe, svećenici
    Other = 99
}



public class SocialPost : BaseAuditableEntity
{
    public int ChronicleId { get; set; }

    public virtual Chronicle? Chronicle { get; set; }

    public string TextForPublish { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; }
}