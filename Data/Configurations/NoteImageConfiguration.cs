namespace N10.Data.Configurations;

public class NoteImageConfiguration : IEntityTypeConfiguration<NoteImage>
{
    public void Configure(EntityTypeBuilder<NoteImage> entity)
    {
        // Nasleđuje od BaseImageEntity, pa samo dodatno
        entity.HasOne(e => e.ApplicationUser).WithMany().HasForeignKey(e => e.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);
        
        entity.HasOne(e => e.Note).WithMany(n => n.Images).HasForeignKey(e => e.NoteId).OnDelete(DeleteBehavior.Cascade);
    }
}
