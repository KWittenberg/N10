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
                if (versionMatch.Success) info.VersionType = versionMatch.Value.ToUpper();

                // ResolutionType
                var resMatch = Regex.Match(postYear, @"\b(480p|720p|1080p|2160p|4K)\b", RegexOptions.IgnoreCase);
                if (resMatch.Success) info.ResolutionType = resMatch.Value.ToLower();

                // ColorDepthType
                var colorMatch = Regex.Match(postYear, @"\b(8bit|10bit|12bit)\b", RegexOptions.IgnoreCase);
                if (colorMatch.Success) info.ColorDepthType = colorMatch.Value.ToLower();

                // SourceType
                var sourceRegex = new Regex(@"\b(WEBRip|WEBDL|BluRay|BRRip|HDRip|DVDRip|HDTV|CAM|WEB-DL)\b", RegexOptions.IgnoreCase);
                var sourceMatch = sourceRegex.Match(postYear);
                if (sourceMatch.Success) info.SourceType = sourceMatch.Value.ToUpper();

                // Audio
                var audioMatch = Regex.Match(postYear, @"\b(\dCH|DTS|AAC|AC3|FLAC)\b", RegexOptions.IgnoreCase);
                if (audioMatch.Success) info.Audio = audioMatch.Value.ToUpper();

                // VideoCodec
                var codecMatch = Regex.Match(postYear, @"\b(x264|x265|HEVC|AVC|AV1)\b", RegexOptions.IgnoreCase);
                if (codecMatch.Success) info.VideoCodec = codecMatch.Value.ToUpper();

                // ReleaseGroup
                var groupMatch = Regex.Match(postYear, @"-([A-Z0-9]+)$", RegexOptions.IgnoreCase);
                if (groupMatch.Success) info.ReleaseGroup = groupMatch.Groups[1].Value.ToUpper();
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