namespace N10.Services.Movies;

public class MovieService(IDbContextFactory<ApplicationDbContext> contextFactory, ITmdbService tmdbService)
{
    readonly string entityName = "Movie";



    public ValueTask<GridItemsProviderResult<Movie>> GetScannedMoviesForGrid(List<ScanMovieModel> sourceList, GridItemsProviderRequest<Movie> req, string searchTerm)
    {
        // 1. Pretvorba ScanMovieModel -> Movie
        var query = sourceList.Select(m => m.ToMovieEntity()).AsQueryable();

        // 2. Search (In-Memory)
        if (!string.IsNullOrWhiteSpace(searchTerm)) query = query.Where(x => x.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        // 3. Sort (In-Memory Reflection - kopirano iz tvog koda)
        var sortSpecs = req.GetSortByProperties();
        if (sortSpecs.Any())
        {
            var sortBy = sortSpecs.First();
            var propInfo = typeof(Movie).GetProperty(sortBy.PropertyName);

            if (propInfo != null)
            {
                query = sortBy.Direction == SortDirection.Ascending
                    ? query.OrderBy(x => propInfo.GetValue(x, null))
                    : query.OrderByDescending(x => propInfo.GetValue(x, null));
            }
        }

        // 4. Paginacija
        var count = query.Count();
        var items = query.Skip(req.StartIndex).Take(req.Count ?? 10).ToList();

        return ValueTask.FromResult(GridItemsProviderResult.From(items, count));
    }

    public async ValueTask<GridItemsProviderResult<Movie>> GetMoviesForGridAsync(GridItemsProviderRequest<Movie> req, string searchTerm)
    {
        // 1. Tvornica (Factory) - čisto i sigurno
        await using var db = await contextFactory.CreateDbContextAsync();
        var query = db.Movies.AsNoTracking();

        // 2. Search
        if (!string.IsNullOrWhiteSpace(searchTerm)) query = query.Where(x => x.Title.Contains(searchTerm));

        // 3. Sort (Tvoj EF.Property trik)
        var sort = req.GetSortByProperties().FirstOrDefault();
        if (sort.PropertyName != null)
        {
            query = sort.Direction == SortDirection.Ascending
                ? query.OrderBy(x => EF.Property<object>(x, sort.PropertyName))
                : query.OrderByDescending(x => EF.Property<object>(x, sort.PropertyName));
        }
        else query = query.OrderByDescending(x => x.FileCreatedUtc);

        // 4. Paginacija & Rezultat
        var count = await query.CountAsync();
        var items = await query.Skip(req.StartIndex).Take(req.Count ?? 20).ToListAsync();

        return GridItemsProviderResult.From(items, count);
    }



    #region SCAN
    public async Task<List<ScanMovieModel>> GetNewMoviesOnlyAsync(string path, HashSet<string> existingFileNames, CancellationToken cancellationToken = default)
    {
        // 1. Samo pročitaj imena fajlova (najbrža operacija)
        var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mkv", ".mp4", ".avi" };

        // Ovo je ultra brzo jer čita samo File Allocation Table, ne otvara fajlove
        var allFiles = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly).Where(f => extensions.Contains(Path.GetExtension(f)));

        // 2. Filtriraj ODMAH (prije teškog parsiranja)
        // Uzimamo samo one čiji FileName nije u bazi
        var newFilesPaths = allFiles.Where(f => !existingFileNames.Contains(Path.GetFileName(f))).ToList();
        if (newFilesPaths.Count == 0) return new List<ScanMovieModel>();

        // 3. Parsiraj samo nove (Heavy lifting)
        var movieInfos = new List<ScanMovieModel>();
        var tasks = newFilesPaths.Select(async filePath =>
        {
            try
            {
                // OVDJE se događa File.GetCreationTimeUtc i Regex - to je ono što košta
                return await ParseFilenameAsync(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing {filePath}: {ex.Message}");
                return null;
            }
        });

        var results = await Task.WhenAll(tasks);
        movieInfos.AddRange(results.Where(info => info != null)!);

        return movieInfos;
    }

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

    #region POPULATE
    public async Task<Result> PopulateFromTmdbByIdAsync(int id)
    {
        // 1. Otvaramo kratki context
        await using var context = await contextFactory.CreateDbContextAsync();

        // Dohvati film
        var movie = await context.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id);
        if (movie is null) return Result.Error($"{entityName} Not Found!");

        try
        {
            TmdbDetails? details = null;

            // 2. FIX: Prvo probaj po ID-u, ALI SAMO AKO POSTOJI
            if (movie.TmdbId.HasValue && movie.TmdbId.Value > 0) details = await tmdbService.GetMovieByIdAsync(movie.TmdbId.Value);

            // 3. FALLBACK: Ako nema ID-a ili ga API nije našao, traži po naslovu
            if (details is null)
            {
                // Ako fali ID, probaj search
                var search = await tmdbService.SearchMoviesAsync(movie.Title, movie.Year?.ToString());

                // Ako nema rezultata, odustani (ovo je onih tvojih 6 problematičnih)
                if (search?.Results is null || search.Results.Count == 0) return Result.Error($"TMDB Search failed for '{movie.Title}'");

                var best = search.Results.First();
                movie.TmdbId = best.Id; // Zapamti ID za ubuduće!
                details = await tmdbService.GetMovieByIdAsync(best.Id);
            }

            if (details is null) return Result.Error("Tmdb Details Not Found (even after search)!");

            // --- MAPIRANJE (Tvoj kod) ---
            movie.TmdbTitle = details.Title;
            movie.ImdbId = details.ImdbId;
            movie.TmdbImageUrl = string.IsNullOrEmpty(details.PosterPath) ? null : details.PosterPath;

            // Genres logic
            movie.Genres.Clear();
            var tmdbIds = details.Genres.Select(g => g.Id).ToList();
            var existingGenres = await context.MovieGenres.Where(x => tmdbIds.Contains(x.TmdbId)).ToListAsync();

            foreach (var g in details.Genres)
            {
                var genre = existingGenres.FirstOrDefault(x => x.TmdbId == g.Id);
                if (genre is null)
                {
                    genre = new MovieGenre { TmdbId = g.Id, TmdbName = g.Name };
                    context.MovieGenres.Add(genre);
                    existingGenres.Add(genre);
                }
                movie.Genres.Add(genre);
            }

            movie.IsMetadataFetched = true;
            //movie.FileModifiedUtc = DateTime.UtcNow; // Dobro je zabilježiti kad je ažurirano
            movie.LastModifiedUtc = DateTime.UtcNow; // Dobro je zabilježiti kad je ažurirano

            // SAVE
            // context.Movies.Update(movie); // Nije nužno jer je tracking upaljen, ali ne škodi
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Result.Error($"Error processing '{movie.Title}': {ex.Message}");
        }

        return Result.Ok($"'{movie.Title}' updated successfully!");
    }

    public async Task<Result> PopulateFromTmdbAsync()
    {
        List<int> idsToProcess;

        await using (var context = await contextFactory.CreateDbContextAsync())
        {
            idsToProcess = await context.Movies.AsNoTracking().Where(x => !x.IsMetadataFetched).Select(x => x.Id).ToListAsync();
        }

        if (idsToProcess.Count == 0) return Result.Error("No movies need updating.");

        int success = 0;
        int failed = 0;

        foreach (var id in idsToProcess)
        {
            var result = await PopulateFromTmdbByIdAsync(id);

            if (result.Success) success++;
            else
            {
                failed++;
                // Opcionalno: Logiraj grešku negdje
                Console.WriteLine(result.Message);
            }

            // Mali odmor za API
            await Task.Delay(50);
        }

        if (success == 0 && failed > 0) return Result.Error($"All {failed} failed. Check logs.");

        return Result.Ok($"Finished: {success} updated, {failed} failed.");
    }
    #endregion
}