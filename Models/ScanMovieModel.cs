namespace N10.Models;

public class ScanMovieModel
{
    public string? FileName { get; set; }

    public string? Title { get; set; }
    public string? SortTitle { get; set; }

    public int? Year { get; set; }
    public string? Version { get; set; }
    public string? Resolution { get; set; }
    public string? Color { get; set; }
    public string? Source { get; set; }
    public string? Audio { get; set; }
    public string? Video { get; set; }
    public string? Release { get; set; }
    public int? TmdbId { get; set; }

    public DateTime? FileCreatedUtc { get; set; }
    public DateTime? FileModifiedUtc { get; set; }





    public Movie ToMovieEntity()
    {
        return new Movie
        {
            FileName = this.FileName ?? string.Empty,
            Title = this.Title ?? string.Empty,
            SortTitle = this.SortTitle,
            Year = this.Year,
            Version = this.Version,
            Resolution = this.Resolution,
            Color = this.Color,
            Source = this.Source,
            Audio = this.Audio,
            Video = this.Video,
            Release = this.Release,
            TmdbId = this.TmdbId,
            FileCreatedUtc = this.FileCreatedUtc,
            FileModifiedUtc = this.FileModifiedUtc
        };
    }
}