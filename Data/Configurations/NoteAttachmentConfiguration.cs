namespace N10.Data.Configurations;

public class NoteAttachmentConfiguration : IEntityTypeConfiguration<NoteAttachment>
{
    public void Configure(EntityTypeBuilder<NoteAttachment> entity)
    {
        // Nasleđuje od BaseAttachmentEntity, pa samo dodatno
        entity.HasOne(e => e.ApplicationUser).WithMany().HasForeignKey(e => e.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.Note).WithMany(n => n.Attachments).HasForeignKey(e => e.NoteId).OnDelete(DeleteBehavior.Cascade);
    }
}
