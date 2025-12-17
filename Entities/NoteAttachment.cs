namespace N10.Entities;

public class NoteAttachment : BaseAttachmentEntity
{
    public int? ApplicationUserId { get; set; }
    public virtual ApplicationUser? ApplicationUser { get; set; }

    public int? NoteId { get; set; }
    public virtual Note? Note { get; set; }
}