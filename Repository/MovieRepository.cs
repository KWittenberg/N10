namespace N10.Repository;

public class MovieRepository(IDbContextFactory<ApplicationDbContext> context, ITmdbService tmdbService) : IMovieRepository
{
    readonly string entityName = "Movie";

    #region CRUD
    public async Task<Result<List<MovieDto>>> GetAllAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        var dtos = await db.Movies.AsNoTracking().OrderBy(x => x.SortTitle).Select(MovieMapping.ToDtoExpression).ToListAsync();
        if (dtos.Count == 0) return Result<List<MovieDto>>.Error($"{entityName} Not Found!");

        return Result<List<MovieDto>>.Ok(dtos);
    }

    public async Task<Result<MovieDto>> GetByIdAsync(Guid id)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.Movies.FindAsync(id);
        if (entity is null) return Result<MovieDto>.Error($"{entityName} Not Found!");

        return Result<MovieDto>.Ok(entity.ToDto());
    }

    public async Task<Result> AddAsync(MovieInput input)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = input.ToEntity();

        await db.Movies.AddAsync(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Added!");
    }

    public async Task<Result> UpdateAsync(Guid id, MovieInput input)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.Movies.FindAsync(id);
        if (entity is null) return Result.Error($"{entityName} Not Found!");

        entity.UpdateFromInput(input);

        db.Movies.Update(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Updated!");
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await using var db = await context.CreateDbContextAsync();

        var entity = await db.Movies.FindAsync(id);
        if (entity is null) return Result.Error($"{entityName} Not Found!");

        db.Movies.Remove(entity);
        await db.SaveChangesAsync();

        return Result.Ok($"{entityName} Deleted!");
    }
    #endregion


    #region Frontend Methods
    public async Task<Result<PaginatedResult<MovieDto>>> GetFilteredPagedAsync(int pageNumber, int pageSize, MovieFilter? filter = null)
    {
        await using var db = await context.CreateDbContextAsync();

        var query = db.Movies.Include(x => x.Genres).AsNoTracking().AsQueryable();


        if (!string.IsNullOrEmpty(filter?.SearchTerm)) query = query.Where(m => m.Title.Contains(filter.SearchTerm));

        if (!string.IsNullOrEmpty(filter?.Letter) && filter.Letter != "all") query = query.Where(m => m.SortTitle.StartsWith(filter.Letter));
        if (filter?.YearFrom.HasValue == true) query = query.Where(m => m.Year >= filter.YearFrom);
        if (filter?.YearTo.HasValue == true) query = query.Where(m => m.Year <= filter.YearTo);
        if (filter?.Genres != null && filter.Genres.Any()) query = query.Where(m => m.Genres.Any(g => filter.Genres.Contains(g.TmdbName)));
        if (filter?.Resolutions != null && filter.Resolutions.Any()) query = query.Where(m => filter.Resolutions.Contains(m.Resolution));


        query = filter?.SortBy switch
        {
            "year" => filter.SortDescending ? query.OrderByDescending(m => m.Year) : query.OrderBy(m => m.Year),
            "rating" => filter.SortDescending ? query.OrderByDescending(m => m.ImdbRating) : query.OrderBy(m => m.ImdbRating),
            "title" or _ => filter.SortDescending ? query.OrderByDescending(m => m.SortTitle) : query.OrderBy(m => m.SortTitle)
        };

        var totalCount = await query.CountAsync();
        var dtos = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(MovieMapping.ToDtoExpression).ToListAsync();

        if (dtos.Count == 0 && pageNumber == 1) return Result<PaginatedResult<MovieDto>>.Error("Nema pronađenih filmova sa zadanim filterima!");

        return Result<PaginatedResult<MovieDto>>.Ok(new PaginatedResult<MovieDto>(dtos, pageNumber, pageSize, totalCount));
    }

    public async Task<Result<FilterOptionsDto>> GetFilterOptionsAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        var options = new FilterOptionsDto
        {
            AvailableYears = await db.Movies.Where(m => m.Year.HasValue).Select(m => m.Year.Value).Distinct().OrderBy(y => y).ToListAsync(),

            AvailableGenres = await db.MovieGenres.Select(mg => mg.TmdbName).Distinct().OrderBy(g => g).ToListAsync(),

            AvailableResolutions = await db.Movies.Where(m => m.Resolution != null).Select(m => m.Resolution).Distinct().OrderBy(r => r).ToListAsync()
        };

        return Result<FilterOptionsDto>.Ok(options);
    }


    // Simple pagination without filters
    public async Task<Result<PaginatedResult<MovieDto>>> GetPagedAsync(int pageNumber, int pageSize)
    {
        await using var db = await context.CreateDbContextAsync();

        var totalCount = await db.Movies.CountAsync();
        var dtos = await db.Movies.AsNoTracking().OrderBy(x => x.SortTitle).Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .Select(MovieMapping.ToDtoExpression)
            .ToListAsync();

        if (dtos.Count == 0) return Result<PaginatedResult<MovieDto>>.Error($"{entityName} Not Found!");

        return Result<PaginatedResult<MovieDto>>.Ok(new PaginatedResult<MovieDto>(dtos, pageNumber, pageSize, totalCount));
    }
    #endregion



    public async Task SyncParsedMoviesAsync(string path)
    {
        List<Movie> parsed = await GetAllMoviesInFolderAsync(path);
        if (parsed.Count == 0) return;

        await using var db = await context.CreateDbContextAsync();

        var dbMovies = await db.Movies.ToListAsync();

        // HASH SET za bržu usporedbu
        var parsedFilesSet = parsed.Select(x => x.FileName).ToHashSet();

        // A) DELETE - file više ne postoji
        var toDelete = dbMovies.Where(dbMovie => !parsedFilesSet.Contains(dbMovie.FileName)).ToList();
        foreach (var dbMovie in toDelete)
        {
            db.Movies.Remove(dbMovie);
        }

        // B) INSERT / UPDATE
        foreach (var parsedMovie in parsed)
        {
            var dbMovie = await db.Movies.FirstOrDefaultAsync(x => x.FileName == parsedMovie.FileName);

            if (dbMovie is null)
            {
                // ADD new movie
                var newMovie = new Movie
                {
                    FileName = parsedMovie.FileName,
                    Title = parsedMovie.Title,
                    SortTitle = CreateSortTitle(parsedMovie.Title) ?? string.Empty,
                    Year = parsedMovie.Year,
                    Version = parsedMovie.Version,
                    Resolution = parsedMovie.Resolution,
                    Color = parsedMovie.Color,
                    Source = parsedMovie.Source,
                    Audio = parsedMovie.Audio,
                    Video = parsedMovie.Video,
                    Release = parsedMovie.Release,
                    TmdbId = parsedMovie.TmdbId,
                    IsMetadataFetched = false
                };

                await db.Movies.AddAsync(newMovie);
            }
            else
            {
                // UPDATE existing movie - samo ako su se promijenile bitne stvari
                bool hasImportantChanges = false;

                // Provjeri promjene u naslovu ili godini - to su bitne promjene
                if (dbMovie.Title != parsedMovie.Title || dbMovie.Year != parsedMovie.Year)
                {
                    dbMovie.Title = parsedMovie.Title;
                    dbMovie.Year = parsedMovie.Year;
                    dbMovie.SortTitle = CreateSortTitle(parsedMovie.Title) ?? string.Empty;
                    hasImportantChanges = true;
                }

                // Ažuriraj tehničke podatke (uvijek) ali ne resetiraj metadata
                dbMovie.Version = parsedMovie.Version;
                dbMovie.Resolution = parsedMovie.Resolution;
                dbMovie.Color = parsedMovie.Color;
                dbMovie.Source = parsedMovie.Source;
                dbMovie.Audio = parsedMovie.Audio;
                dbMovie.Video = parsedMovie.Video;
                dbMovie.Release = parsedMovie.Release;

                // Ako su se bitne stvari promijenile, resetiraj metadata
                if (hasImportantChanges)
                {
                    dbMovie.IsMetadataFetched = false;
                    dbMovie.TmdbId = parsedMovie.TmdbId; // Resetiraj TMDB ID jer se film možda promijenio
                }

                db.Movies.Update(dbMovie);
            }
        }

        await db.SaveChangesAsync();
    }




    static readonly string[] IgnorePrefixes = ["the ", "a ", "an "];

    string CreateSortTitle(string title)
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

    public async Task PopulateFromTmdbAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        var movies = await db.Movies.Include(x => x.Genres).Where(x => x.IsMetadataFetched == false).ToListAsync();
        if (movies.Count == 0) return;

        foreach (var movie in movies)
        {
            try
            {
                TmdbDetails? details = null;

                // Pokušaj dohvatiti po TmdbId ako postoji
                if (movie.TmdbId is not null) details = await tmdbService.GetMovieByIdAsync(movie.TmdbId.Value);

                // Ako nema detalja, pretraži po naslovu i godini
                if (details is null)
                {
                    var search = await tmdbService.SearchMoviesAsync(movie.Title, movie.Year?.ToString());
                    if (search?.Results is null || search.Results.Count == 0) continue;

                    var best = search.Results.First();
                    if (best.Id == 0) continue;

                    movie.TmdbId = best.Id;
                    details = await tmdbService.GetMovieByIdAsync(best.Id);
                    if (details is null) continue;
                }

                // MAPIRANJE TMDB → MOVIE
                movie.TmdbTitle = details.Title;
                movie.ImdbId = details.ImdbId;
                movie.TmdbImageUrl = string.IsNullOrEmpty(details.PosterPath)
                    ? null
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

                // Delay (anti-rate-limit)
                await Task.Delay(300);
            }
            catch
            {
                // Ako pojedini film padne, preskači
                continue;
            }
        }
    }






    #region GetAllMoviesInFolder
    public async Task<List<Movie>> GetAllMoviesInFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        var fileNames = await ReadFolderAsync(path, cancellationToken);

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

    async Task<List<string>> ReadFolderAsync(string path, CancellationToken cancellationToken = default)
    {
        var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mkv", ".mp4", ".avi" };

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested(); // Baci exception ako je otkazano

            // Change Folder Path as needed
            var allFiles = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            // var allFiles = Directory.EnumerateFiles(Globals.X265Path, "*.*", SearchOption.TopDirectoryOnly);
            // var allFiles = Directory.EnumerateFiles(Globals.LocalMovies, "*.*", SearchOption.TopDirectoryOnly);

            var fileNames = allFiles
                .Where(file => extensions.Contains(Path.GetExtension(file)))
                .Select(file => Path.GetFileName(file))
                .ToList();

            return fileNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();

        }, cancellationToken);
    }

    async Task<Movie> ParseFilenameAsync(string filename)
    {
        var nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
        var info = new Movie { FileName = filename };

        var tokens = Regex.Split(nameWithoutExt, @"[.\s_-]+")
                          .Where(t => !string.IsNullOrWhiteSpace(t))
                          .ToList();

        var knownPatterns = new Dictionary<string, Action<Movie, string>>(StringComparer.OrdinalIgnoreCase)
        {
            // Versions
            ["4K"] = (m, _) => m.Version = AddVersion(m, "4K"),
            ["CHINESE"] = (m, _) => m.Version = AddVersion(m, "CHINESE"),
            ["COLLECTION"] = (m, _) => m.Version = AddVersion(m, "COLLECTION"),
            ["CRITERION"] = (m, _) => m.Version = AddVersion(m, "CRITERION"),
            ["CUT"] = (m, _) => m.Version = AddVersion(m, "CUT"),
            ["DIRECTOR'S"] = (m, _) => m.Version = AddVersion(m, "DIRECTOR'S"),
            ["EDITION"] = (m, _) => m.Version = AddVersion(m, "EDITION"),
            ["EXTENDED"] = (m, _) => m.Version = AddVersion(m, "EXTENDED"),
            ["FINAL"] = (m, _) => m.Version = AddVersion(m, "FINAL"),
            ["FRENCH"] = (m, _) => m.Version = AddVersion(m, "FRENCH"),
            ["IMAX"] = (m, _) => m.Version = AddVersion(m, "IMAX"),
            ["HR"] = (m, _) => m.Version = AddVersion(m, "HR"),
            ["INTERNAL"] = (m, _) => m.Version = AddVersion(m, "INTERNAL"),
            ["JAPANESE"] = (m, _) => m.Version = AddVersion(m, "JAPANESE"),
            ["KOREAN"] = (m, _) => m.Version = AddVersion(m, "KOREAN"),
            ["MASTERED"] = (m, _) => m.Version = AddVersion(m, "MASTERED"),
            ["REMASTERED"] = (m, _) => m.Version = AddVersion(m, "REMASTERED"),
            ["REPACK"] = (m, _) => m.Version = AddVersion(m, "REPACK"),
            ["ROGUE"] = (m, _) => m.Version = AddVersion(m, "ROGUE"),
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
            ["HDCzT"] = (m, _) => m.Release = "HDCzT",
            ["Tigole"] = (m, _) => m.Release = "Tigole",
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
    #endregion
}