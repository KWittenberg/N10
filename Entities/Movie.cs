namespace N10.Entities;

public class Movie : BaseEntity<Guid>
{
    public string Title { get; set; } = string.Empty;

    public int? Year { get; set; } // 1900-2099

    public string? VersionType { get; set; } // "IMAX", "DIRECTOR'S.CUT", "EXTENDED", "INTERNAL", "REMASTERED", "RUSSIAN", "UNRATED.DIRECTOR'S.CUT.REMASTERED", "UNRATED", "MASTERED.IN.4K", "PARAMOUNT.REISSUE.EXTENDED.CUT.REMASTERED", "REPACK"

    public string? ResolutionType { get; set; } // "720p", "1080p", "2160p"

    public string? ColorDepthType { get; set; } // "10bit", "8bit"

    public string? SourceType { get; set; } // "WEBRip", "BluRay"

    public string? Audio { get; set; } // "6CH", "8CH"

    public string? VideoCodec { get; set; } // "x264", "x265", "HEVC"

    public string? ReleaseGroup { get; set; } // "PSA", "YIFY", "SPARKS"
}