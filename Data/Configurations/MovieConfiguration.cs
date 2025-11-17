namespace N10.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.FileName).HasMaxLength(MovieConst.FileNameLength).IsRequired();

        entity.Property(e => e.Title).HasMaxLength(MovieConst.TitleLength).IsRequired();
        entity.Property(e => e.SortTitle).HasMaxLength(MovieConst.SortTitleLength);

        entity.Property(e => e.Version).HasMaxLength(MovieConst.VersionLength);
        entity.Property(e => e.Resolution).HasMaxLength(MovieConst.ResolutionLength);
        entity.Property(e => e.Color).HasMaxLength(MovieConst.ColorLength);
        entity.Property(e => e.Source).HasMaxLength(MovieConst.SourceLength);
        entity.Property(e => e.Audio).HasMaxLength(MovieConst.AudioLength);
        entity.Property(e => e.Video).HasMaxLength(MovieConst.VideoLength);
        entity.Property(e => e.Release).HasMaxLength(MovieConst.ReleaseLength);

        entity.Property(e => e.TmdbTitle).HasMaxLength(MovieConst.TmdbTitleLength);
        entity.Property(e => e.TmdbImageUrl).HasMaxLength(MovieConst.TmdbImageUrlLength);

        entity.Property(e => e.ImdbId).HasMaxLength(MovieConst.ImdbIdLength);


        entity.HasIndex(e => e.Title);
        entity.HasIndex(e => e.Year);

        entity.HasMany(e => e.Genres)
              .WithMany(e => e.Movies);

        //entity.HasMany(e => e.Genres).WithMany().UsingEntity<Dictionary<string, object>>("MovieMovieGenres",
        //j => j.HasOne<MovieGenre>().WithMany().HasForeignKey("GenreId"),
        //j => j.HasOne<Movie>().WithMany().HasForeignKey("MovieId"));
    }
}