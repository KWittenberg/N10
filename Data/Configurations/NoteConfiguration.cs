namespace N10.Data.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> entity)
    {
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Title).IsRequired().HasMaxLength(NoteConst.TitleLength);
        
        entity.Property(e => e.Content).HasMaxLength(NoteConst.ContentLength);
        
        // entity.Property(e => e.ContentType).IsRequired();
        
        entity.Property(e => e.Color).HasMaxLength(NoteConst.ColorLength);
        
        entity.Property(e => e.ReminderAt);
        
        entity.Property(e => e.IsEncrypted).IsRequired();
        
        entity.Property(e => e.EncryptionMetadata).HasMaxLength(NoteConst.EncryptionMetadataLength);


        entity.HasOne(e => e.ApplicationUser).WithMany().HasForeignKey(e => e.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(e => e.NoteFolder).WithMany(f => f.Notes).HasForeignKey(e => e.NoteFolderId).OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(e => e.Shares).WithOne(s => s.Note).HasForeignKey(s => s.NoteId).OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(e => e.Images).WithOne(i => i.Note).HasForeignKey(i => i.NoteId).OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(e => e.Attachments).WithOne(a => a.Note).HasForeignKey(a => a.NoteId).OnDelete(DeleteBehavior.Cascade);
    }
}
