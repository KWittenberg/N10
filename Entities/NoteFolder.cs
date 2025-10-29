namespace N10.Entities;

public class NoteFolder : BaseAuditableEntity<Guid>
{
    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; } = null!;

    public Guid? ParentFolderId { get; set; }
    public virtual NoteFolder? ParentFolder { get; set; } = null!;



    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }



    public virtual ICollection<NoteFolder> SubFolders { get; set; } = [];
    public virtual ICollection<Note> Notes { get; set; } = [];
}