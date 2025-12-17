namespace N10.Entities;

public class TvShowEpisode : BaseAuditableEntity
{
    public string FileName { get; set; } = string.Empty;
    public int? Season { get; set; }
    public int? Episode { get; set; }
    public string? EpisodeTitle { get; set; }


    public string? Version { get; set; }
    public string? Resolution { get; set; }
    public string? Color { get; set; }
    public string? Source { get; set; }
    public string? Audio { get; set; }
    public string? Video { get; set; }
    public string? Release { get; set; }

    public int TvShowId { get; set; }
    public virtual TvShow TvShow { get; set; } = null!;
}