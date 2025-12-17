namespace N10.DTOs;

public class NoteInput
{
    public int? UserId { get; set; }

    public int? NoteFolderId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    // public NoteContentType ContentType { get; set; } = NoteContentType.PlainText;

    public string? Color { get; set; } = "#fd7e14";

    public DateTime? ReminderAt { get; set; }

    public bool IsEncrypted { get; set; }

    public string? EncryptionMetadata { get; set; }
}