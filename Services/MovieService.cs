using System.Text.RegularExpressions;

namespace N10.Services;

//public class MovieLibraryService(IHttpClientFactory httpClientFactory) : BackgroundService, IMovieLibraryService
public class MovieService(IOptions<TmdbOptions> tmdb) : IMovieService
{
    //private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    //private readonly string _apiKey = "tvoj_api_key"; // Spremi u appsettings.json
    //private List<MovieModel> _movies = new List<MovieModel>(); // Ili koristi DB

    //private readonly string folderPath = @"C:\Temp";
    private readonly string folderPath = @"\\192.168.1.1\TOSHIBA_ExternalUSB30_2_ba41\X265";


    public async Task<List<Movie>> GetAllMoviesAsync(CancellationToken cancellationToken = default)
    {
        var fileNames = await ReadFolderAsync(cancellationToken);

        var movieInfos = new List<Movie>();

        var tasks = fileNames.Select(async fileName =>
        {
            try
            {
                return await ParseFilenameAsync(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing {fileName}: {ex.Message}");
                return new Movie { Title = $"Error: {fileName}" };
            }
        });

        var results = await Task.WhenAll(tasks);
        movieInfos.AddRange(results.Where(info => info != null));

        return movieInfos;
    }

    async Task<List<string>> ReadFolderAsync(CancellationToken cancellationToken = default)
    {
        var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mkv", ".mp4", ".avi" };

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested(); // Baci exception ako je otkazano

            var allFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);

            var fileNames = allFiles
                .Where(file => extensions.Contains(Path.GetExtension(file)))
                .Select(file => Path.GetFileName(file))
                .ToList();

            return fileNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();

        }, cancellationToken);
    }

    public async Task<Movie> ParseFilenameAsync(string filename)
    {
        var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
        var info = new Movie { FileName = filename };

        // Pre-pass za fraze koje želimo tretirati kao jedan token
        nameWithoutExt = nameWithoutExt
            .Replace("Special.Edition", "SpecialEdition", StringComparison.OrdinalIgnoreCase)
            .Replace("Final.Cut", "FinalCut", StringComparison.OrdinalIgnoreCase)
            .Replace("Director's.Cut", "DirectorsCut", StringComparison.OrdinalIgnoreCase)
            .Replace("Extended.Edition", "ExtendedEdition", StringComparison.OrdinalIgnoreCase)
            .Replace("Unrated.Directors.Cut", "UnratedDirectorsCut", StringComparison.OrdinalIgnoreCase)
            .Replace("Mastered.In.4K", "MasteredIn4K", StringComparison.OrdinalIgnoreCase);

        var tokens = Regex.Split(nameWithoutExt, @"[.\s_-]+")
                          .Where(t => !string.IsNullOrWhiteSpace(t))
                          .ToList();

        // --- MAPA POZNATIH PATTERNA ---
        var knownPatterns = new Dictionary<string, Action<Movie, string>>(StringComparer.OrdinalIgnoreCase)
        {
            // Versions
            ["IMAX"] = (m, _) => m.Version = AddVersion(m, "IMAX"),
            ["INTERNAL"] = (m, _) => m.Version = AddVersion(m, "INTERNAL"),
            ["DIRECTOR'S"] = (m, _) => m.Version = AddVersion(m, "DIRECTOR'S.CUT"),
            ["DIRECTORSCUT"] = (m, _) => m.Version = AddVersion(m, "DIRECTOR'S.CUT"),
            ["CUT"] = (m, _) => m.Version = AddVersion(m, "CUT"),
            ["SPECIALEDITION"] = (m, _) => m.Version = AddVersion(m, "SPECIAL.EDITION"),
            ["FINALCUT"] = (m, _) => m.Version = AddVersion(m, "FINAL.CUT"),
            ["EXTENDED"] = (m, _) => m.Version = AddVersion(m, "EXTENDED"),
            ["EXTENDEDEDITION"] = (m, _) => m.Version = AddVersion(m, "EXTENDED.EDITION"),
            ["UNRATED"] = (m, _) => m.Version = AddVersion(m, "UNRATED"),
            ["UNRATEDDIRECTORSCUT"] = (m, _) => m.Version = AddVersion(m, "UNRATED.DIRECTOR'S.CUT"),
            ["MASTEREDIN4K"] = (m, _) => m.Version = AddVersion(m, "MASTERED.IN.4K"),
            ["REMASTERED"] = (m, _) => m.Version = AddVersion(m, "REMASTERED"),
            ["REPACK"] = (m, _) => m.Version = AddVersion(m, "REPACK"),
            ["RUSSIAN"] = (m, _) => m.Version = AddVersion(m, "RUSSIAN"),

            // Resolution
            ["480p"] = (m, _) => m.Resolution = "480p",
            ["720p"] = (m, _) => m.Resolution = "720p",
            ["1080p"] = (m, _) => m.Resolution = "1080p",
            ["2160p"] = (m, _) => m.Resolution = "2160p",
            ["4K"] = (m, _) => m.Resolution = "2160p",

            // Color
            // TODO: dodati isto metodu za spajanje 10bit.HDR
            //["8bit"] = (m, _) => m.Color = "8bit",
            //["10bit"] = (m, _) => m.Color = "10bit",
            //["12bit"] = (m, _) => m.Color = "12bit",
            //["16bit"] = (m, _) => m.Color = "16bit",
            //["HDR"] = (m, _) => m.Color = "HDR",

            ["8bit"] = (m, _) => m.Color = AddColor(m, "8bit"),
            ["10bit"] = (m, _) => m.Color = AddColor(m, "10bit"),
            ["12bit"] = (m, _) => m.Color = AddColor(m, "12bit"),
            ["16bit"] = (m, _) => m.Color = AddColor(m, "16bit"),
            ["HDR"] = (m, _) => m.Color = AddColor(m, "HDR"),

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
            ["X264"] = (m, _) => m.Video = "x264",
            ["X265"] = (m, _) => m.Video = "x265",
            ["HEVC"] = (m, _) => m.Video = "HEVC",
            ["AV1"] = (m, _) => m.Video = "AV1",

            // Release
            ["FGT"] = (m, _) => m.Release = "FGT",
            ["PSA"] = (m, _) => m.Release = "PSA",
            ["YIFY"] = (m, _) => m.Release = "YIFY",
            ["UTR"] = (m, _) => m.Release = "UTR",
            ["SPARKS"] = (m, _) => m.Release = "SPARKS",
            ["RARBG"] = (m, _) => m.Release = "RARBG",
            ["xxxpav69"] = (m, _) => m.Release = "xxxpav69"
        };

        // --- DETEKCIJA GODINE + TMDB ID-a ---
        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (int.TryParse(token, out int number))
            {
                if (number >= 1900 && number <= 2099) info.Year = number;
                else if (number > 0 && number < 9999999)
                {
                    // heuristika: TMDB ID je obično zadnji ili predzadnji token
                    if (i >= tokens.Count - 2) info.TmdbId = number;
                }
            }

            if (knownPatterns.TryGetValue(token.ToUpperInvariant(), out var apply)) apply(info, token);
        }

        // --- NASLOV ---
        if (info.Year.HasValue)
        {
            int yearIndex = tokens.FindIndex(t => t == info.Year.Value.ToString());
            info.Title = string.Join(" ", tokens.Take(yearIndex));
        }
        else
        {
            // fallback ako nema godine
            info.Title = string.Join(" ", tokens);
        }

        // --- FORMATIRANJE NASLOVA ---
        info.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Title.ToLowerInvariant().Trim());

        return await Task.FromResult(info);
    }

    // --- HELPER METODA ---
    static string AddVersion(Movie m, string value)
    {
        if (string.IsNullOrEmpty(m.Version)) return value;
        if (m.Version.Contains(value, StringComparison.OrdinalIgnoreCase)) return m.Version; // već postoji, preskoči

        return $"{m.Version}.{value}";
    }

    static string AddColor(Movie m, string value)
    {
        if (string.IsNullOrEmpty(m.Color)) return value;
        if (m.Color.Contains(value, StringComparison.OrdinalIgnoreCase)) return m.Color; // već postoji, preskoči

        return $"{m.Color}.{value}";
    }


    public async Task<Movie> ParseFilenameWithRegexAsync(string filename)
    {
        var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
        var info = new Movie();

        info.FileName = filename;

        // 1. TMDB ID na kraju (fleksibilno)
        var tmdbMatch = Regex.Match(nameWithoutExt, @"[-._]?tmdb[-._]?(\d{5,7})$", RegexOptions.IgnoreCase);
        if (tmdbMatch.Success && int.TryParse(tmdbMatch.Groups[1].Value, out int tmdbId))
        {
            info.TmdbId = tmdbId;
            nameWithoutExt = Regex.Replace(nameWithoutExt, @"[-._]?tmdb[-._]?\d{5,7}$", "", RegexOptions.IgnoreCase).Trim(' ', '.', '-', '_');
        }
        else
        {
            var idMatch = Regex.Match(nameWithoutExt, @"-(\d{5,7})$");
            if (idMatch.Success && int.TryParse(idMatch.Groups[1].Value, out tmdbId))
            {
                info.TmdbId = tmdbId;
                nameWithoutExt = nameWithoutExt.Substring(0, nameWithoutExt.Length - idMatch.Value.Length).Trim(' ', '.', '-', '_');
            }
        }

        // 2. Pronađi POSLJEDNJU godinu (fleksibilno, sa \b za izolaciju)
        var yearMatches = Regex.Matches(nameWithoutExt, @"\b((19|20)\d{2})\b");
        if (yearMatches.Count > 0)
        {
            var lastYearMatch = yearMatches[yearMatches.Count - 1];
            var yearStr = lastYearMatch.Value;
            if (int.TryParse(yearStr, out int year) && year >= 1900 && year <= 2099)
            {
                info.Year = year;

                var start = lastYearMatch.Index;
                var end = start + lastYearMatch.Length;
                var preYear = nameWithoutExt.Substring(0, start).Trim(' ', '.', '-', '_');
                var postYear = nameWithoutExt.Substring(end).Trim(' ', '.', '-', '_');

                // Title iz preYear (ili iz postYear prije tagova ako pre prazan)
                info.Title = Regex.Replace(preYear, @"\.+", " ").Trim();
                if (string.IsNullOrEmpty(info.Title))
                {
                    // Pokuşaj iz postYear prije prvih tagova
                    var tagStartMatch = Regex.Match(postYear, @"\b(1080p|720p|2160p|4K|WEBRip|BluRay|WEB-DL|10bit|8bit|x265|x264|HEVC|AV1|DTS|AAC|\dCH|-PSA|-YIFY)\b", RegexOptions.IgnoreCase);
                    if (tagStartMatch.Success)
                    {
                        var potentialTitle = postYear.Substring(0, tagStartMatch.Index).Trim();
                        info.Title = Regex.Replace(potentialTitle, @"\.+", " ").Trim();
                    }
                    else
                    {
                        info.Title = Regex.Replace(postYear, @"\.+", " ").Trim();
                    }
                }

                // TitleCase za ljepši izgled
                if (!string.IsNullOrEmpty(info.Title))
                {
                    info.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Title.ToLower());
                }

                // Tagovi iz postYear + preYear (cijeli za sigurnost)
                var tagsStr = preYear + " " + postYear;

                // VersionType
                var versionRegex = new Regex(@"\b(IMAX|DIRECTORS?.CUT|EXTENDED|INTERNAL|REMASTERED|UNRATED|MASTERED.IN.4K|REPACK|FINAL.CUT)\b", RegexOptions.IgnoreCase);
                var versionMatch = versionRegex.Match(tagsStr);
                if (versionMatch.Success) info.Version = versionMatch.Value.ToUpper();

                // ResolutionType
                var resMatch = Regex.Match(postYear, @"\b(480p|720p|1080p|2160p|4K)\b", RegexOptions.IgnoreCase);
                if (resMatch.Success) info.Resolution = resMatch.Value.ToLower();

                // ColorDepthType
                var colorMatch = Regex.Match(postYear, @"\b(8bit|10bit|12bit)\b", RegexOptions.IgnoreCase);
                if (colorMatch.Success) info.Color = colorMatch.Value.ToLower();

                // SourceType
                var sourceRegex = new Regex(@"\b(WEBRip|WEBDL|BluRay|BRRip|HDRip|DVDRip|HDTV|CAM|WEB-DL)\b", RegexOptions.IgnoreCase);
                var sourceMatch = sourceRegex.Match(postYear);
                if (sourceMatch.Success) info.Source = sourceMatch.Value.ToUpper();

                // Audio
                var audioMatch = Regex.Match(postYear, @"\b(\dCH|DTS|AAC|AC3|FLAC)\b", RegexOptions.IgnoreCase);
                if (audioMatch.Success) info.Audio = audioMatch.Value.ToUpper();

                // VideoCodec
                var codecMatch = Regex.Match(postYear, @"\b(x264|x265|HEVC|AVC|AV1)\b", RegexOptions.IgnoreCase);
                if (codecMatch.Success) info.Video = codecMatch.Value.ToUpper();

                // ReleaseGroup
                var groupMatch = Regex.Match(postYear, @"-([A-Z0-9]+)$", RegexOptions.IgnoreCase);
                if (groupMatch.Success) info.Release = groupMatch.Groups[1].Value.ToUpper();
            }
        }
        else
        {
            // Fallback bez godine (npr. serije bez godine u filenameu)
            info.Title = Regex.Replace(nameWithoutExt, @"\.+", " ").Trim();
            if (!string.IsNullOrEmpty(info.Title))
            {
                info.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Title.ToLower());
            }
        }

        return await Task.FromResult(info);
    }
}