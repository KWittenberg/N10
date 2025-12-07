namespace N10.Services.Movies;

public class MovieScannerService
{
    public async Task<List<Movie>> GetAllMoviesInFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        var filePaths = await ReadFolderAsync(path, cancellationToken);
        var movieInfos = new List<Movie>();

        var tasks = filePaths.Select(async filePath =>
        {
            try
            {
                return await ParseFilenameAsync(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing {filePath}: {ex.Message}");
                return new Movie { Title = $"Error: {Path.GetFileName(filePath)}" };
            }
        });

        var results = await Task.WhenAll(tasks);
        movieInfos.AddRange(results.Where(info => info != null));

        return movieInfos;
    }

    // 1. Čitanje foldera i filtriranje video datoteka
    async Task<List<string>> ReadFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mkv", ".mp4", ".avi" };

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested(); // Baci exception ako je otkazano

            return Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(f => extensions.Contains(Path.GetExtension(f)))
                            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                            .ToList();



            // Change Folder Path as needed
            // var allFiles = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            // var allFiles = Directory.EnumerateFiles(Globals.X265Path, "*.*", SearchOption.TopDirectoryOnly);
            // var allFiles = Directory.EnumerateFiles(Globals.LocalMovies, "*.*", SearchOption.TopDirectoryOnly);

            // var fileNames = allFiles.Where(file => extensions.Contains(Path.GetExtension(file))).Select(file => Path.GetFileName(file)).ToList();

            // return fileNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();

        }, cancellationToken);
    }

    // 2. Parsiranje imena datoteke
    async Task<Movie> ParseFilenameAsync(string filePath)
    {
        var filename = Path.GetFileName(filePath);
        var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);

        var info = new Movie
        {
            FileName = filename,
            FileCreatedUtc = File.GetCreationTimeUtc(filePath),
            FileModifiedUtc = File.GetLastWriteTimeUtc(filePath)
        };

        var tokens = Regex.Split(nameWithoutExt, @"[.\s_-]+").Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

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

            if (MoviePatterns.KnownPatterns.TryGetValue(token.ToUpperInvariant(), out var apply)) apply(info, token);
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
}
