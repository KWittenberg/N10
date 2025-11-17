namespace N10.Entities;

public class MovieGenre : BaseEntity<Guid>
{
    public int TmdbId { get; set; }

    public string TmdbName { get; set; } = string.Empty;


    public virtual ICollection<Movie> Movies { get; set; } = [];
}