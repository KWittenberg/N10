namespace N10.Data.Configurations;

public class NoteShareConfiguration : IEntityTypeConfiguration<NoteShare>
{
    public void Configure(EntityTypeBuilder<NoteShare> entity)
    {
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Permissions).IsRequired();
        
        entity.Property(e => e.ExpiresAt);


        entity.HasOne(e => e.ApplicationUser).WithMany().HasForeignKey(e => e.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(e => e.SharedWithUser).WithMany().HasForeignKey(e => e.SharedWithUserId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(e => e.Note).WithMany(n => n.Shares).HasForeignKey(e => e.NoteId).OnDelete(DeleteBehavior.Cascade);
    }
}