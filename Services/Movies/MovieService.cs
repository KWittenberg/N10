namespace N10.Services.Movies;

public class MovieService(IDbContextFactory<ApplicationDbContext> context, ITmdbService tmdbService)
{
    readonly string entityName = "Movie";




    #region MovieScannerService
    public async Task<List<ScanMovieModel>> GetAllMoviesInFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        var filePaths = await ReadFolderAsync(path, cancellationToken);
        var movieInfos = new List<ScanMovieModel>();

        var tasks = filePaths.Select(async filePath =>
        {
            try
            {
                return await ParseFilenameAsync(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing {filePath}: {ex.Message}");
                return new ScanMovieModel { Title = $"Error: {Path.GetFileName(filePath)}" };
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
    async Task<ScanMovieModel> ParseFilenameAsync(string filePath)
    {
        var filename = Path.GetFileName(filePath);
        var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);

        var info = new ScanMovieModel
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
        info.SortTitle = CreateSortTitle(info.Title);

        return await Task.FromResult(info);
    }

    // 3. Kreiranje sortirajućeg naslova
    public string CreateSortTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return string.Empty;

        title = title.Trim().ToLower();

        foreach (var prefix in IgnorePrefixes)
        {
            if (title.StartsWith(prefix)) title = title[prefix.Length..];
        }

        // makni smeće
        title = new string(title.Where(char.IsLetterOrDigit).ToArray());

        // velika slova čisto radi lijepog spremanja
        return char.ToUpper(title[0]) + title[1..];
    }

    static readonly string[] IgnorePrefixes = ["the ", "a ", "an "];
    #endregion




    public async Task<Result> PopulateFromTmdbByIdAsync(Guid id)
    {
        await using var db = await context.CreateDbContextAsync();

        var movie = await db.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id);
        if (movie is null) return Result.Error($"{entityName} Not Found!");

        try
        {
            var details = await tmdbService.GetMovieByIdAsync(movie.TmdbId!.Value);
            if (details is null) return Result.Error("Tmdb Details Not Found!");


            // MAPIRANJE TMDB → MOVIE
            movie.TmdbTitle = details.Title;
            movie.ImdbId = details.ImdbId;

            movie.TmdbImageUrl = string.IsNullOrEmpty(details.PosterPath)
                ? string.Empty
                : details.PosterPath;

            // Genres
            movie.Genres.Clear();

            // Dohvati sve TmdbId-eve iz details
            var tmdbIds = details.Genres.Select(g => g.Id).ToList();

            // Pronađi sve postojeće žanrove odjednom
            var existingGenres = await db.MovieGenres.Where(x => tmdbIds.Contains(x.TmdbId)).ToListAsync();

            foreach (var g in details.Genres)
            {
                var genre = existingGenres.FirstOrDefault(x => x.TmdbId == g.Id);

                if (genre is null)
                {
                    genre = new MovieGenre
                    {
                        TmdbId = g.Id,
                        TmdbName = g.Name
                    };
                    db.MovieGenres.Add(genre);
                    existingGenres.Add(genre); // dodaj u listu da ga možeš ponovno koristiti
                }

                movie.Genres.Add(genre);
            }

            movie.IsMetadataFetched = true;

            // SAVE
            db.Movies.Update(movie);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }

        return Result.Ok($"{entityName} Succifuly Populated From Tmdb!");
    }
}
