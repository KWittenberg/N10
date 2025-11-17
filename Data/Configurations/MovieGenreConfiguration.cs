namespace N10.Data.Configurations;

public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
{
    public void Configure(EntityTypeBuilder<MovieGenre> entity)
    {
        entity.HasKey(e => e.Id);


        entity.Property(e => e.TmdbId).IsRequired();

        entity.Property(e => e.TmdbName).HasMaxLength(MovieGenreConst.TmdbNameLength).IsRequired();


        entity.HasIndex(e => e.TmdbName).IsUnique();
    }
}