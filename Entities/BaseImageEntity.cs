namespace N10.Entities;

public abstract class BaseImageEntity : BaseAuditableEntity
{
    public string FileName { get; set; } = string.Empty;

    public ImageType Type { get; set; } = ImageType.Other;

    public string FileUrl { get; set; } = string.Empty;


    public int? Width { get; set; }

    public int? Height { get; set; }

    public long? FileSize { get; set; }


    public string? MimeType { get; set; }
}