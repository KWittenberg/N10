namespace N10.Entities;

public class NoteInput
{
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    // public NoteContentType ContentType { get; set; } = NoteContentType.PlainText;

    public string? Color { get; set; }

    public DateTime? ReminderAt { get; set; }

    public bool IsEncrypted { get; set; }

    public string? EncryptionMetadata { get; set; }
}