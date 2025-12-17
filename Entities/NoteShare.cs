namespace N10.Entities;

public class NoteShare : BaseEntity
{
    public int ApplicationUserId { get; set; }
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;

    public int SharedWithUserId { get; set; }
    public virtual ApplicationUser SharedWithUser { get; set; } = null!;

    public int NoteId { get; set; }
    public virtual Note Note { get; set; } = null!;

    public NoteSharePermission Permissions { get; set; } = NoteSharePermission.Read;

    public DateTime? ExpiresAt { get; set; }
}