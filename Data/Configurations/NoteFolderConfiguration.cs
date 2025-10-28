namespace N10.Data.Configurations;

public class NoteFolderConfiguration : IEntityTypeConfiguration<NoteFolder>
{
    public void Configure(EntityTypeBuilder<NoteFolder> entity)
    {
        entity.HasKey(e => e.Id);
        
        entity.Property(e => e.Name).IsRequired().HasMaxLength(NoteFolderConst.NameLength);
        
        entity.Property(e => e.Color).HasMaxLength(NoteFolderConst.ColorLength);


        entity.HasOne(e => e.ApplicationUser).WithMany().HasForeignKey(e => e.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(e => e.ParentFolder).WithMany(f => f.SubFolders).HasForeignKey(e => e.ParentFolderId).OnDelete(DeleteBehavior.Cascade);
        entity.HasMany(e => e.SubFolders).WithOne(f => f.ParentFolder).HasForeignKey(f => f.ParentFolderId).OnDelete(DeleteBehavior.NoAction); // Izbegavaj rekurziju
        entity.HasMany(e => e.Notes).WithOne(n => n.NoteFolder).HasForeignKey(n => n.NoteFolderId).OnDelete(DeleteBehavior.Cascade);
    }
}
