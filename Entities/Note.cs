namespace N10.Entities;

public class Note : BaseAuditableEntity<Guid>
{
    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; } = null!;

    public Guid? NoteFolderId { get; set; }
    public virtual NoteFolder? NoteFolder { get; set; } = null!;


    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    // public NoteContentType ContentType { get; set; } = NoteContentType.PlainText;

    public string? Color { get; set; }

    public DateTime? ReminderAt { get; set; }

    public bool IsEncrypted { get; set; }

    public string? EncryptionMetadata { get; set; }



    public virtual ICollection<NoteShare> Shares { get; set; } = [];


    public virtual ICollection<NoteImage> Images { get; set; } = [];

    public virtual ICollection<NoteAttachment> Attachments { get; set; } = [];
}