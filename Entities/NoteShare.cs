namespace N10.Entities;

public class NoteShare : BaseEntity<Guid>
{
    public Guid ApplicationUserId { get; set; }
    public virtual ApplicationUser ApplicationUser { get; set; } = null!;

    public Guid SharedWithUserId { get; set; }
    public virtual ApplicationUser SharedWithUser { get; set; } = null!;

    public Guid NoteId { get; set; }
    public virtual Note Note { get; set; } = null!;

    public NoteSharePermission Permissions { get; set; } = NoteSharePermission.Read;

    public DateTime? ExpiresAt { get; set; }
}