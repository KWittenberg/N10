namespace N10.DTOs;

public class NoteDto
{
    public int Id { get; set; }

    public int? UserId { get; set; }
    public virtual UserDto? User { get; set; } = null!;

    public int? NoteFolderId { get; set; }
    //public virtual NoteFolderDto? NoteFolder { get; set; } = null!;


    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    // public NoteContentType ContentType { get; set; } = NoteContentType.PlainText;

    public string? Color { get; set; }

    public DateTime? ReminderAt { get; set; }

    public bool IsEncrypted { get; set; }

    public string? EncryptionMetadata { get; set; }



    //public virtual ICollection<NoteShareDto> Shares { get; set; } = [];


    //public virtual ICollection<NoteImageDto> Images { get; set; } = [];

    //public virtual ICollection<NoteAttachmentDto> Attachments { get; set; } = [];
}