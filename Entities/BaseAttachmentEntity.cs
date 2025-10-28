namespace N10.Entities;

public abstract class BaseAttachmentEntity : BaseAuditableEntity<Guid>
{
    public string FileName { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string? MimeType { get; set; }

    public long? FileSize { get; set; }

    public bool IsEncrypted { get; set; }
}