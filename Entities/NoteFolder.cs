namespace N10.Entities;

public class NoteFolder : BaseAuditableEntity
{
    public int? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; } = null!;

    public int? ParentFolderId { get; set; }
    public virtual NoteFolder? ParentFolder { get; set; } = null!;



    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }



    public virtual ICollection<NoteFolder> SubFolders { get; set; } = [];
    public virtual ICollection<Note> Notes { get; set; } = [];
}