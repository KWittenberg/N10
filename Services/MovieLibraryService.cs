using System.Text.RegularExpressions;

namespace N10.Services;

//public class MovieLibraryService(IHttpClientFactory httpClientFactory) : BackgroundService, IMovieLibraryService
public class MovieLibraryService() : IMovieLibraryService
{
    //private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    //private readonly string _apiKey = "tvoj_api_key"; // Spremi u appsettings.json
    //private List<MovieModel> _movies = new List<MovieModel>(); // Ili koristi DB

    //private readonly string folderPath = @"C:\Temp";
    private readonly string folderPath = @"\\192.168.1.1\TOSHIBA_ExternalUSB30_2_ba41\X265";




    public async Task<List<MovieInfo>> GetAllMoviesAsync(CancellationToken cancellationToken = default)
    {
        var fileNames = await ReadFolderAsync(cancellationToken);

        var movieInfos = new List<MovieInfo>();

        var tasks = fileNames.Select(async fileName =>
        {
            try
            {
                return await ParseFilenameAsync(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing {fileName}: {ex.Message}");
                return new MovieInfo { Title = $"Error: {fileName}" };
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

    async Task<MovieInfo> ParseFilenameAsync(string filename)
    {
        // Ukloni ekstenziju
        var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);

        var info = new MovieInfo();

        // 1. Pronađi godinu (4 broja između 1900-2099)
        var yearMatch = Regex.Match(nameWithoutExt, @"\.(19|20)\d{2}\.");
        if (yearMatch.Success)
        {
            var yearStr = yearMatch.Value.Trim('.');
            if (int.TryParse(yearStr, out int year) && year >= 1900 && year <= 2099)
            {
                info.Year = year;

                // Podijeli string na dio prije godine (naziv + eventualne verzije) i nakon (tehnički detalji)
                var parts = nameWithoutExt.Split(yearMatch.Value);
                var preYear = parts[0].Trim();
                var postYear = parts.Length > 1 ? parts[1].Trim() : string.Empty;

                // 2. Naziv: Dio prije godine, zamijeni točke sa spaceovima, ali izvuci verzije ako postoje
                // Prvo izvuci VersionType iz preYear ili postYear (common pattern: nakon naziva, prije godine ili u tehničkom dijelu)
                var versionPatterns = new[] { "IMAX", "DIRECTOR'S\\.CUT", "EXTENDED", "INTERNAL", "REMASTERED", "RUSSIAN", "UNRATED\\.DIRECTOR'S\\.CUT\\.REMASTERED", "UNRATED", "MASTERED\\.IN\\.4K", "PARAMOUNT\\.REISSUE\\.EXTENDED\\.CUT\\.REMASTERED", "REPACK", "SWEDISH" };
                var versionRegex = new Regex($@"\b({string.Join("|", versionPatterns)})\b", RegexOptions.IgnoreCase);
                var versionMatch = versionRegex.Match(preYear + "." + postYear); // Traži u cijelom
                if (versionMatch.Success)
                {
                    info.VersionType = versionMatch.Value.ToUpper(); // Normaliziraj na upper za konzistenciju
                    preYear = preYear.Replace(versionMatch.Value, "").Trim('.'); // Ukloni iz naziva ako je tu
                }

                info.Title = preYear.Replace('.', ' ').Trim(); // Sada naziv sa spaceovima

                // 3. Tehnički detalji iz postYear (rezolucija, boja, izvor, audio, kodek, grupa)
                // ResolutionType: 720p, 1080p, 2160p
                var resolutionMatch = Regex.Match(postYear, @"(720p|1080p|2160p)", RegexOptions.IgnoreCase);
                if (resolutionMatch.Success) info.ResolutionType = resolutionMatch.Value.ToLower();

                // ColorDepthType: 10bit, 8bit
                var colorDepthMatch = Regex.Match(postYear, @"(10bit|8bit)", RegexOptions.IgnoreCase);
                if (colorDepthMatch.Success) info.ColorDepthType = colorDepthMatch.Value.ToLower();

                // SourceType: WEBRip, BluRay (dodaj više ako treba, npr. HDRip, DVDRip)
                var sourceMatch = Regex.Match(postYear, @"(WEBRip|BrRip|BluRay|HDRip|DVDRip)", RegexOptions.IgnoreCase);
                if (sourceMatch.Success) info.SourceType = sourceMatch.Value;

                // Audio: 6CH, 8CH (možeš dodati više, npr. 2CH, DTS)
                var audioMatch = Regex.Match(postYear, @"(\dCH|DTS|AAC)", RegexOptions.IgnoreCase);
                if (audioMatch.Success) info.Audio = audioMatch.Value.ToUpper();

                // VideoCodec: x264, x265, HEVC (ako se pojavljuje više, uzmi prvi ili kombiniraj)
                var codecMatch = Regex.Match(postYear, @"(x264|x265|HEVC)", RegexOptions.IgnoreCase);
                if (codecMatch.Success) info.VideoCodec = codecMatch.Value.ToUpper();

                // ReleaseGroup: Zadnji dio nakon '-', npr. PSA, YIFY (pretpostavka da je na kraju)
                var groupMatch = Regex.Match(postYear, @"-(\w+)$");
                if (groupMatch.Success) info.ReleaseGroup = groupMatch.Groups[1].Value.ToUpper();
                else
                {
                    // Alternativa: Traži common grupe
                    var groupPatterns = new[] { "PSA", "YIFY", "SPARKS" }; // Dodaj više ako znaš
                    var groupRegex = new Regex($@"\b({string.Join("|", groupPatterns)})\b", RegexOptions.IgnoreCase);
                    var groupFound = groupRegex.Match(postYear);
                    if (groupFound.Success) info.ReleaseGroup = groupFound.Value.ToUpper();
                }
            }
        }

        // Fallback ako godina nije nađena: Cijeli naziv kao Title, i pokušaj naći ostalo u cijelom stringu
        if (string.IsNullOrEmpty(info.Title))
        {
            info.Title = nameWithoutExt.Replace('.', ' ').Trim();
            // Ponovi regexe za ostalo na cijelom nameWithoutExt ako treba
        }

        return await Task.FromResult(info); // Async wrapper
    }





    //public MovieInfo ParseFilename(string filename)
    //{
    //    // Ukloni ekstenziju
    //    var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);

    //    // Regex za godinu (4 broja između 1900-2099)
    //    var yearRegex = Regex.Match(nameWithoutExt, @"\.(19|20)\d{2}\.");
    //    if (yearRegex.Success)
    //    {
    //        var yearStr = yearRegex.Value.Trim('.');
    //        int year;
    //        if (int.TryParse(yearStr, out year))
    //        {
    //            // Podijeli na dijelove
    //            var parts = nameWithoutExt.Split(yearRegex.Value);
    //            var titlePart = parts[0].Replace('.', ' ').Trim(); // Naziv sa spaceovima
    //            var techPart = parts.Length > 1 ? parts[1] : string.Empty;

    //            // Parsiraj tehničke dijelove (opcionalno, koristi regex za specifične)
    //            var resolution = Regex.Match(techPart, @"(720p|1080p|2160p|4K)").Value;
    //            var codec = Regex.Match(techPart, @"(x264|x265|HEVC)").Value;

    //            return new MovieInfo
    //            {
    //                Title = titlePart,
    //                Year = year,
    //                ResolutionType = resolution,
    //                VideoCodec = codec
    //            };
    //        }
    //    }

    //    // Ako ne uspije, fallback na cijeli naziv
    //    return new MovieInfo { Title = nameWithoutExt.Replace('.', ' ').Trim() };
    //}





    //public async Task<List<string>> ReadFolderAsync(bool sortByYear = false)
    //{
    //    var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mkv", ".mp4", ".avi" };

    //    var allFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);

    //    var fileNames = allFiles.Where(file => extensions.Contains(Path.GetExtension(file)))
    //                            .Select(file => Path.GetFileName(file))
    //                            .ToList();

    //    return fileNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();
    //}






    //public async Task<List<MovieInfo>> GetAllMoviesInFolderAsync()
    //{
    //    var fileNames = await ReadFolderAsync();

    //    var movieInfos = new List<MovieInfo>();
    //    foreach (var fileName in fileNames)
    //    {
    //        var info = await ParseFilenameAsync(fileName);
    //        movieInfos.Add(info);
    //    }

    //    return movieInfos;
    //}






    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //    // Skeniraj folder
    //    var files = Directory.EnumerateFiles(_folderPath, "*.mkv", SearchOption.AllDirectories);

    //    foreach (var file in files)
    //    {
    //        var info = ParseFilename(file);
    //        if (!string.IsNullOrEmpty(info.Title) && info.Year.HasValue)
    //        {
    //            // TMDB search
    //            var query = $"{info.Title.Replace(' ', '+')}&year={info.Year}";
    //            var url = $"https://api.themoviedb.org/3/search/movie?api_key={_apiKey}&query={query}";
    //            var response = await _httpClient.GetStringAsync(url);
    //            var tmdbResult = JsonSerializer.Deserialize<TmdbSearchResponse>(response);

    //            if (tmdbResult.Results.Count > 0)
    //            {
    //                var tmdbMovie = tmdbResult.Results[0];
    //                // Dodaj u listu (ili DB)
    //                _movies.Add(new MovieModel
    //                {
    //                    LocalPath = file,
    //                    Title = tmdbMovie.Title,
    //                    Year = info.Year.Value,
    //                    Overview = tmdbMovie.Overview,
    //                    PosterUrl = $"https://image.tmdb.org/t/p/w500{tmdbMovie.PosterPath}",
    //                    // Dodaj ostalo
    //                });
    //            }
    //        }
    //    }
    //    // Nakon skeniranja, možeš spremiti u SQLite ili JSON file
    //}

    // Modeli
    public class TmdbSearchResponse
    {
        public List<TmdbMovie> Results { get; set; }
    }

    public class TmdbMovie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        // Ostalo
    }

    public class MovieModel
    {
        public string LocalPath { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Overview { get; set; }
        public string PosterUrl { get; set; }
    }

    // Metoda za dohvaćanje liste u Blazoru
    // public List<MovieModel> GetMovies() => _movies;
}