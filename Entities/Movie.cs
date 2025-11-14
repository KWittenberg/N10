namespace N10.Entities;

public class Movie : BaseEntity<Guid>
{

    public string FileName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int? Year { get; set; } // 1900-2099

    public string? Version { get; set; } // "IMAX", "DIRECTOR'S.CUT", "EXTENDED", "INTERNAL", "REMASTERED", "RUSSIAN", "UNRATED.DIRECTOR'S.CUT.REMASTERED", "UNRATED", "MASTERED.IN.4K", "PARAMOUNT.REISSUE.EXTENDED.CUT.REMASTERED", "REPACK"

    public string? Resolution { get; set; } // "720p", "1080p", "2160p"

    public string? Color { get; set; } // "10bit", "8bit"

    public string? Source { get; set; } // "WEBRip", "BluRay"

    public string? Audio { get; set; } // "6CH", "8CH"

    public string? Video { get; set; } // "x264", "x265", "HEVC"

    public string? Release { get; set; } // "PSA", "YIFY", "SPARKS"



    public int? TmdbId { get; set; }

    public string? TmdbTitle { get; set; }

    public string? TmdbOriginalTitle { get; set; }

    public string? TmdbImageUrl { get; set; }

    public string? ImdbId { get; set; }




    public override string ToString() => $"Title: {Title}\nYear: {Year}\nVersion: {Version}\nResolution: {Resolution}\nColor: {Color}\nSource: {Source}\nAudio: {Audio}\nVideo: {Video}\nRelease: {Release}\nTMDB: {TmdbId}";
}