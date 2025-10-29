namespace N10.Entities;

public class NoteInput
{
    public Guid? UserId { get; set; }

    public Guid? NoteFolderId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    // public NoteContentType ContentType { get; set; } = NoteContentType.PlainText;

    public string? Color { get; set; } = "#FFFFFF";

    public DateTime? ReminderAt { get; set; }

    public bool IsEncrypted { get; set; }

    public string? EncryptionMetadata { get; set; }
}