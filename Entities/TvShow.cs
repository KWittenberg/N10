namespace N10.Entities;

public class TvShow : BaseAuditableEntity
{
    public string FolderName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? SortTitle { get; set; }

    public int? Year { get; set; }


    public int? TmdbId { get; set; }
    public string? TmdbTitle { get; set; }
    public string? TmdbImageUrl { get; set; }

    public string? ImdbId { get; set; }
    public double? ImdbRating { get; set; }


    public bool IsMetadataFetched { get; set; }


    public virtual ICollection<TvShowGenre> Genres { get; set; } = [];
    public virtual ICollection<TvShowEpisode> Episodes { get; set; } = [];
}