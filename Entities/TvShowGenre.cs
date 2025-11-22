namespace N10.Entities;

public class TvShowGenre : BaseEntity<Guid>
{
    public int TmdbId { get; set; }

    public string TmdbName { get; set; } = string.Empty;


    public virtual ICollection<TvShow> TvShows { get; set; } = [];
}