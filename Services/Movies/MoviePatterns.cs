namespace N10.Services.Movies;

public static class MoviePatterns
{
    public static readonly Dictionary<string, Action<ScanMovieModel, string>> KnownPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        // Versions
        ["CHI"] = (m, _) => m.Version = AddVersion(m, "CHINESE"),
        ["CHINESE"] = (m, _) => m.Version = AddVersion(m, "CHINESE"),
        ["COLLECTION"] = (m, _) => m.Version = AddVersion(m, "COLLECTION"),
        ["CRITERION"] = (m, _) => m.Version = AddVersion(m, "CRITERION"),
        ["CUT"] = (m, _) => m.Version = AddVersion(m, "CUT"),
        ["DIRECTOR'S"] = (m, _) => m.Version = AddVersion(m, "DIRECTOR'S"),
        ["EDITION"] = (m, _) => m.Version = AddVersion(m, "EDITION"),
        ["ENG"] = (m, _) => m.Version = AddVersion(m, "ENGLISH"),
        ["EXTENDED"] = (m, _) => m.Version = AddVersion(m, "EXTENDED"),
        ["FINAL"] = (m, _) => m.Version = AddVersion(m, "FINAL"),
        ["FRENCH"] = (m, _) => m.Version = AddVersion(m, "FRENCH"),
        ["HR"] = (m, _) => m.Version = AddVersion(m, "CROATIAN"),
        ["IMAX"] = (m, _) => m.Version = AddVersion(m, "IMAX"),
        ["INTERNAL"] = (m, _) => m.Version = AddVersion(m, "INTERNAL"),
        ["JAPANESE"] = (m, _) => m.Version = AddVersion(m, "JAPANESE"),
        ["KOR"] = (m, _) => m.Version = AddVersion(m, "KOREAN"),
        ["KOREAN"] = (m, _) => m.Version = AddVersion(m, "KOREAN"),
        ["MASTERED"] = (m, _) => m.Version = AddVersion(m, "MASTERED"),
        ["NOR"] = (m, _) => m.Version = AddVersion(m, "NORWAY"),
        ["REGRADED"] = (m, _) => m.Version = AddVersion(m, "REGRADED"),
        ["REMASTERED"] = (m, _) => m.Version = AddVersion(m, "REMASTERED"),
        ["REPACK"] = (m, _) => m.Version = AddVersion(m, "REPACK"),
        ["ROGUE"] = (m, _) => m.Version = AddVersion(m, "ROGUE"),
        ["RUS"] = (m, _) => m.Version = AddVersion(m, "RUSSIAN"),
        ["RUSSIAN"] = (m, _) => m.Version = AddVersion(m, "RUSSIAN"),
        ["SPECIAL"] = (m, _) => m.Version = AddVersion(m, "SPECIAL"),
        ["SWEDISH"] = (m, _) => m.Version = AddVersion(m, "SWEDISH"),
        ["THEATRICAL"] = (m, _) => m.Version = AddVersion(m, "THEATRICAL"),
        ["ULYSSES"] = (m, _) => m.Version = AddVersion(m, "ULYSSES"),
        ["UNCUT"] = (m, _) => m.Version = AddVersion(m, "UNCUT"),
        ["UNRATED"] = (m, _) => m.Version = AddVersion(m, "UNRATED"),

        // Resolution
        ["480p"] = (m, _) => m.Resolution = "480p",
        ["720p"] = (m, _) => m.Resolution = "720p",
        ["1080p"] = (m, _) => m.Resolution = "1080p",
        ["2160p"] = (m, _) => m.Resolution = "2160p",
        ["4K"] = (m, _) => m.Resolution = "2160p",

        // Color
        ["8bit"] = (m, _) => m.Color = AddColor(m, "8bit"),
        ["10bit"] = (m, _) => m.Color = AddColor(m, "10bit"),
        ["12bit"] = (m, _) => m.Color = AddColor(m, "12bit"),
        ["16bit"] = (m, _) => m.Color = AddColor(m, "16bit"),
        ["HDR"] = (m, _) => m.Color = AddColor(m, "HDR"),
        ["HDR10Plus"] = (m, _) => m.Color = AddColor(m, "HDR10Plus"),
        ["HQ"] = (m, _) => m.Color = AddColor(m, "HQ"),

        // Source
        ["WEBRIP"] = (m, _) => m.Source = "WEBRip",
        ["WEB-DL"] = (m, _) => m.Source = "WEB-DL",
        ["BLURAY"] = (m, _) => m.Source = "BluRay",
        ["BRRIP"] = (m, _) => m.Source = "BRRip",
        ["HDRIP"] = (m, _) => m.Source = "HDRip",
        ["DVDRIP"] = (m, _) => m.Source = "DVDRip",

        // Audio
        ["2CH"] = (m, _) => m.Audio = "2CH",
        ["6CH"] = (m, _) => m.Audio = "6CH",
        ["8CH"] = (m, _) => m.Audio = "8CH",
        ["DTS"] = (m, _) => m.Audio = "DTS",
        ["AAC"] = (m, _) => m.Audio = "AAC",
        ["AC3"] = (m, _) => m.Audio = "AC3",
        ["FLAC"] = (m, _) => m.Audio = "FLAC",

        // Video
        ["X264"] = (m, _) => m.Video = AddVideo(m, "x264"),
        ["X265"] = (m, _) => m.Video = AddVideo(m, "x265"),
        ["HEVC"] = (m, _) => m.Video = AddVideo(m, "HEVC"),
        ["AV1"] = (m, _) => m.Video = AddVideo(m, "AV1"),

        // Release
        ["BONE"] = (m, _) => m.Release = "BONE",
        ["FGT"] = (m, _) => m.Release = "FGT",
        ["HDCzT"] = (m, _) => m.Release = "HDCzT",
        ["PSA"] = (m, _) => m.Release = "PSA",
        ["RARBG"] = (m, _) => m.Release = "RARBG",
        ["SPARKS"] = (m, _) => m.Release = "SPARKS",
        ["Tigole"] = (m, _) => m.Release = "Tigole",
        ["UTR"] = (m, _) => m.Release = "UTR",
        ["xxxpav69"] = (m, _) => m.Release = "xxxpav69",
        ["YIFY"] = (m, _) => m.Release = "YIFY"
    };

    #region --- HELPER METODA ---
    static string AddVersion(ScanMovieModel m, string value)
    {
        if (string.IsNullOrEmpty(m.Version)) return value;
        if (m.Version.Contains(value, StringComparison.OrdinalIgnoreCase)) return m.Version; // već postoji, preskoči

        return $"{m.Version}.{value}";
    }

    static string AddColor(ScanMovieModel m, string value)
    {
        if (string.IsNullOrEmpty(m.Color)) return value;
        if (m.Color.Contains(value, StringComparison.OrdinalIgnoreCase)) return m.Color;

        return $"{m.Color}.{value}";
    }

    static string AddVideo(ScanMovieModel m, string value)
    {
        if (string.IsNullOrEmpty(m.Video)) return value;
        if (m.Video.Contains(value, StringComparison.OrdinalIgnoreCase)) return m.Video;

        return $"{m.Video}.{value}";
    }
    #endregion
}