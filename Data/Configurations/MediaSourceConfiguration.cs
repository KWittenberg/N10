namespace N10.Data.Configurations;

public class MediaSourceConfiguration : IEntityTypeConfiguration<MediaSource>
{
    public void Configure(EntityTypeBuilder<MediaSource> builder)
    {
        builder.ToTable("MediaSources");

        builder.HasKey(e => e.Id);

        builder.Property(x => x.Name).HasMaxLength(MediaSourceConst.NameLength).IsRequired();

        builder.Property(x => x.BasePath).HasMaxLength(MediaSourceConst.BasePathLength).IsRequired();


        builder.HasMany(s => s.Movies).WithOne(m => m.MediaSource).HasForeignKey(m => m.MediaSourceId).OnDelete(DeleteBehavior.Cascade);
    }
}