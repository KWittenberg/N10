namespace N10.DTOs;

public class MovieInput
{
    public string FileName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

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
    public string? TmdbTitle { get; set; }
    public string? TmdbImageUrl { get; set; }

    public string? ImdbId { get; set; }
    public double? ImdbRating { get; set; }


    public bool IsMetadataFetched { get; set; }
}