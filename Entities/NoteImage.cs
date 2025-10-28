namespace N10.Entities;

public class NoteImage : BaseImageEntity
{
    public Guid? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }

    public Guid? NoteId { get; set; }
    public virtual Note? Note { get; set; } = null!;
}