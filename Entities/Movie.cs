namespace N10.Entities;

public class Movie : BaseAuditableEntity<Guid>
{
    public string FileName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? SortTitle { get; set; }

    public int? Year { get; set; } // 1900-2099

    public string? Version { get; set; } // "IMAX", "EXTENDED", "INTERNAL", "REMASTERED", "UNRATED", "REPACK"
    public string? Resolution { get; set; } // "720p", "1080p", "2160p"
    public string? Color { get; set; } // "10bit", "8bit"
    public string? Source { get; set; } // "WEBRip", "BluRay"
    public string? Audio { get; set; } // "6CH", "8CH"
    public string? Video { get; set; } // "x264", "x265", "HEVC"
    public string? Release { get; set; } // "PSA", "YIFY", "SPARKS"


    public int? TmdbId { get; set; }
    public string? TmdbTitle { get; set; }
    public string? TmdbImageUrl { get; set; }

    public string? ImdbId { get; set; }
    public double? ImdbRating { get; set; }


    public bool IsMetadataFetched { get; set; }


    public virtual ICollection<MovieGenre> Genres { get; set; } = [];
}