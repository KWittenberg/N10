namespace N10.Repository;

public class TvShowRepository(IDbContextFactory<ApplicationDbContext> context, ITmdbService tmdbService) : ITvShowRepository
{
    readonly string entityName = "TvShow";

    #region CRUD
    //public async Task<Result<List<MovieDto>>> GetAllAsync()
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var dtos = await db.Movies.AsNoTracking().OrderBy(x => x.SortTitle).Select(MovieMapping.ToDtoExpression).ToListAsync();
    //    if (dtos.Count == 0) return Result<List<MovieDto>>.Error($"{entityName} Not Found!");

    //    return Result<List<MovieDto>>.Ok(dtos);
    //}

    //public async Task<Result<MovieDto>> GetByIdAsync(Guid id)
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var entity = await db.Movies.FindAsync(id);
    //    if (entity is null) return Result<MovieDto>.Error($"{entityName} Not Found!");

    //    return Result<MovieDto>.Ok(entity.ToDto());
    //}

    //public async Task<Result> AddAsync(MovieInput input)
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var entity = input.ToEntity();

    //    await db.Movies.AddAsync(entity);
    //    await db.SaveChangesAsync();

    //    return Result.Ok($"{entityName} Added!");
    //}

    //public async Task<Result> UpdateAsync(Guid id, MovieInput input)
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var entity = await db.Movies.FindAsync(id);
    //    if (entity is null) return Result.Error($"{entityName} Not Found!");

    //    entity.UpdateFromInput(input);

    //    db.Movies.Update(entity);
    //    await db.SaveChangesAsync();

    //    return Result.Ok($"{entityName} Updated!");
    //}

    //public async Task<Result> DeleteAsync(Guid id)
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var entity = await db.Movies.FindAsync(id);
    //    if (entity is null) return Result.Error($"{entityName} Not Found!");

    //    db.Movies.Remove(entity);
    //    await db.SaveChangesAsync();

    //    return Result.Ok($"{entityName} Deleted!");
    //}
    #endregion


    #region Frontend Methods
    //public async Task<Result<PaginatedResult<MovieDto>>> GetFilteredPagedAsync(int pageNumber, int pageSize, MovieFilter? filter = null)
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var query = db.Movies.Include(x => x.Genres).AsNoTracking().AsQueryable();


    //    if (!string.IsNullOrEmpty(filter?.SearchTerm)) query = query.Where(m => m.Title.Contains(filter.SearchTerm));

    //    if (!string.IsNullOrEmpty(filter?.Letter) && filter.Letter != "all") query = query.Where(m => m.SortTitle.StartsWith(filter.Letter));
    //    if (filter?.YearFrom.HasValue == true) query = query.Where(m => m.Year >= filter.YearFrom);
    //    if (filter?.YearTo.HasValue == true) query = query.Where(m => m.Year <= filter.YearTo);
    //    if (filter?.Genres != null && filter.Genres.Any()) query = query.Where(m => m.Genres.Any(g => filter.Genres.Contains(g.TmdbName)));
    //    if (filter?.Resolutions != null && filter.Resolutions.Any()) query = query.Where(m => filter.Resolutions.Contains(m.Resolution));


    //    query = filter?.SortBy switch
    //    {
    //        "year" => filter.SortDescending ? query.OrderByDescending(m => m.Year) : query.OrderBy(m => m.Year),
    //        "rating" => filter.SortDescending ? query.OrderByDescending(m => m.ImdbRating) : query.OrderBy(m => m.ImdbRating),
    //        "title" or _ => filter.SortDescending ? query.OrderByDescending(m => m.SortTitle) : query.OrderBy(m => m.SortTitle)
    //    };

    //    var totalCount = await query.CountAsync();
    //    var dtos = await query
    //        .Skip((pageNumber - 1) * pageSize)
    //        .Take(pageSize)
    //        .Select(MovieMapping.ToDtoExpression)
    //        .ToListAsync();

    //    if (dtos.Count == 0 && pageNumber == 1) return Result<PaginatedResult<MovieDto>>.Error("Nema pronađenih filmova sa zadanim filterima!");

    //    return Result<PaginatedResult<MovieDto>>.Ok(new PaginatedResult<MovieDto>(dtos, pageNumber, pageSize, totalCount));
    //}

    //public async Task<Result<FilterOptionsDto>> GetFilterOptionsAsync()
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var options = new FilterOptionsDto
    //    {
    //        AvailableYears = await db.Movies
    //            .Where(m => m.Year.HasValue)
    //            .Select(m => m.Year.Value)
    //            .Distinct()
    //            .OrderBy(y => y)
    //            .ToListAsync(),

    //        AvailableGenres = await db.MovieGenres
    //            .Select(mg => mg.TmdbName)
    //            .Distinct()
    //            .OrderBy(g => g)
    //            .ToListAsync(),

    //        AvailableResolutions = await db.Movies
    //            .Where(m => m.Resolution != null)
    //            .Select(m => m.Resolution)
    //            .Distinct()
    //            .OrderBy(r => r)
    //            .ToListAsync()
    //    };

    //    return Result<FilterOptionsDto>.Ok(options);
    //}


    // Simple pagination without filters
    //public async Task<Result<PaginatedResult<MovieDto>>> GetPagedAsync(int pageNumber, int pageSize)
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var totalCount = await db.Movies.CountAsync();
    //    var dtos = await db.Movies.AsNoTracking().OrderBy(x => x.SortTitle).Skip((pageNumber - 1) * pageSize).Take(pageSize)
    //        .Select(MovieMapping.ToDtoExpression)
    //        .ToListAsync();

    //    if (dtos.Count == 0) return Result<PaginatedResult<MovieDto>>.Error($"{entityName} Not Found!");

    //    return Result<PaginatedResult<MovieDto>>.Ok(new PaginatedResult<MovieDto>(dtos, pageNumber, pageSize, totalCount));
    //}
    #endregion



    //public async Task SyncParsedMoviesAsync(string path)
    //{
    //    List<TvShow> parsed = await GetAllTvShowInFolderAsync(path);
    //    if (parsed.Count == 0) return;

    //    await using var db = await context.CreateDbContextAsync();

    //    var dbMovies = await db.Movies.ToListAsync();

    //    // HASH SET za bržu usporedbu
    //    var parsedFilesSet = parsed.Select(x => x.FileName).ToHashSet();

    //    // A) DELETE - file više ne postoji
    //    var toDelete = dbMovies.Where(dbMovie => !parsedFilesSet.Contains(dbMovie.FileName)).ToList();
    //    foreach (var dbMovie in toDelete)
    //    {
    //        db.Movies.Remove(dbMovie);
    //    }

    //    // B) INSERT / UPDATE
    //    foreach (var parsedMovie in parsed)
    //    {
    //        var dbMovie = await db.Movies.FirstOrDefaultAsync(x => x.FileName == parsedMovie.FileName);

    //        if (dbMovie is null)
    //        {
    //            // ADD new movie
    //            var newMovie = new Movie
    //            {
    //                FileName = parsedMovie.FileName,
    //                Title = parsedMovie.Title,
    //                SortTitle = CreateSortTitle(parsedMovie.Title) ?? string.Empty,
    //                Year = parsedMovie.Year,
    //                Version = parsedMovie.Version,
    //                Resolution = parsedMovie.Resolution,
    //                Color = parsedMovie.Color,
    //                Source = parsedMovie.Source,
    //                Audio = parsedMovie.Audio,
    //                Video = parsedMovie.Video,
    //                Release = parsedMovie.Release,
    //                TmdbId = parsedMovie.TmdbId,
    //                IsMetadataFetched = false
    //            };

    //            await db.Movies.AddAsync(newMovie);
    //        }
    //        else
    //        {
    //            // UPDATE existing movie - samo ako su se promijenile bitne stvari
    //            bool hasImportantChanges = false;

    //            // Provjeri promjene u naslovu ili godini - to su bitne promjene
    //            if (dbMovie.Title != parsedMovie.Title || dbMovie.Year != parsedMovie.Year)
    //            {
    //                dbMovie.Title = parsedMovie.Title;
    //                dbMovie.Year = parsedMovie.Year;
    //                dbMovie.SortTitle = CreateSortTitle(parsedMovie.Title) ?? string.Empty;
    //                hasImportantChanges = true;
    //            }

    //            // Ažuriraj tehničke podatke (uvijek) ali ne resetiraj metadata
    //            dbMovie.Version = parsedMovie.Version;
    //            dbMovie.Resolution = parsedMovie.Resolution;
    //            dbMovie.Color = parsedMovie.Color;
    //            dbMovie.Source = parsedMovie.Source;
    //            dbMovie.Audio = parsedMovie.Audio;
    //            dbMovie.Video = parsedMovie.Video;
    //            dbMovie.Release = parsedMovie.Release;

    //            // Ako su se bitne stvari promijenile, resetiraj metadata
    //            if (hasImportantChanges)
    //            {
    //                dbMovie.IsMetadataFetched = false;
    //                dbMovie.TmdbId = parsedMovie.TmdbId; // Resetiraj TMDB ID jer se film možda promijenio
    //            }

    //            db.Movies.Update(dbMovie);
    //        }
    //    }

    //    await db.SaveChangesAsync();
    //}




    //static readonly string[] IgnorePrefixes = ["the ", "a ", "an "];

    //string CreateSortTitle(string title)
    //{
    //    if (string.IsNullOrWhiteSpace(title)) return string.Empty;

    //    title = title.Trim().ToLower();

    //    foreach (var prefix in IgnorePrefixes)
    //    {
    //        if (title.StartsWith(prefix)) title = title[prefix.Length..];
    //    }

    //    // makni smeće
    //    title = new string(title.Where(char.IsLetterOrDigit).ToArray());

    //    // velika slova čisto radi lijepog spremanja
    //    return char.ToUpper(title[0]) + title[1..];
    //}

    //public async Task<Result> PopulateFromTmdbByIdAsync(Guid id)
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var movie = await db.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id);
    //    if (movie is null) return Result.Error($"{entityName} Not Found!");

    //    try
    //    {
    //        var details = await tmdbService.GetMovieByIdAsync(movie.TmdbId!.Value);
    //        if (details is null) return Result.Error("Tmdb Details Not Found!");


    //        // MAPIRANJE TMDB → MOVIE
    //        movie.TmdbTitle = details.Title;
    //        movie.ImdbId = details.ImdbId;

    //        movie.TmdbImageUrl = string.IsNullOrEmpty(details.PosterPath)
    //            ? string.Empty
    //            : details.PosterPath;

    //        // Genres
    //        movie.Genres.Clear();

    //        // Dohvati sve TmdbId-eve iz details
    //        var tmdbIds = details.Genres.Select(g => g.Id).ToList();

    //        // Pronađi sve postojeće žanrove odjednom
    //        var existingGenres = await db.MovieGenres.Where(x => tmdbIds.Contains(x.TmdbId)).ToListAsync();

    //        foreach (var g in details.Genres)
    //        {
    //            var genre = existingGenres.FirstOrDefault(x => x.TmdbId == g.Id);

    //            if (genre is null)
    //            {
    //                genre = new MovieGenre
    //                {
    //                    TmdbId = g.Id,
    //                    TmdbName = g.Name
    //                };
    //                db.MovieGenres.Add(genre);
    //                existingGenres.Add(genre); // dodaj u listu da ga možeš ponovno koristiti
    //            }

    //            movie.Genres.Add(genre);
    //        }

    //        movie.IsMetadataFetched = true;

    //        // SAVE
    //        db.Movies.Update(movie);
    //        await db.SaveChangesAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        return Result.Error(ex.Message);
    //    }

    //    return Result.Ok($"{entityName} Succifuly Populated From Tmdb!");
    //}

    //public async Task PopulateFromTmdbAsync()
    //{
    //    await using var db = await context.CreateDbContextAsync();

    //    var movies = await db.Movies.Include(x => x.Genres).Where(x => x.IsMetadataFetched == false).ToListAsync();
    //    if (movies.Count == 0) return;

    //    foreach (var movie in movies)
    //    {
    //        try
    //        {
    //            TmdbDetails? details = null;

    //            // Pokušaj dohvatiti po TmdbId ako postoji
    //            if (movie.TmdbId is not null) details = await tmdbService.GetMovieByIdAsync(movie.TmdbId.Value);

    //            // Ako nema detalja, pretraži po naslovu i godini
    //            if (details is null)
    //            {
    //                var search = await tmdbService.SearchMoviesAsync(movie.Title, movie.Year?.ToString());
    //                if (search?.Results is null || search.Results.Count == 0) continue;

    //                var best = search.Results.First();
    //                if (best.Id == 0) continue;

    //                movie.TmdbId = best.Id;
    //                details = await tmdbService.GetMovieByIdAsync(best.Id);
    //                if (details is null) continue;
    //            }

    //            // MAPIRANJE TMDB → MOVIE
    //            movie.TmdbTitle = details.Title;
    //            movie.ImdbId = details.ImdbId;
    //            movie.TmdbImageUrl = string.IsNullOrEmpty(details.PosterPath)
    //                ? null
    //                : details.PosterPath;

    //            // Genres
    //            movie.Genres.Clear();

    //            // Dohvati sve TmdbId-eve iz details
    //            var tmdbIds = details.Genres.Select(g => g.Id).ToList();

    //            // Pronađi sve postojeće žanrove odjednom
    //            var existingGenres = await db.MovieGenres.Where(x => tmdbIds.Contains(x.TmdbId)).ToListAsync();

    //            foreach (var g in details.Genres)
    //            {
    //                var genre = existingGenres.FirstOrDefault(x => x.TmdbId == g.Id);

    //                if (genre is null)
    //                {
    //                    genre = new MovieGenre
    //                    {
    //                        TmdbId = g.Id,
    //                        TmdbName = g.Name
    //                    };
    //                    db.MovieGenres.Add(genre);
    //                    existingGenres.Add(genre); // dodaj u listu da ga možeš ponovno koristiti
    //                }

    //                movie.Genres.Add(genre);
    //            }

    //            movie.IsMetadataFetched = true;

    //            // SAVE
    //            db.Movies.Update(movie);
    //            await db.SaveChangesAsync();

    //            // Delay (anti-rate-limit)
    //            await Task.Delay(300);
    //        }
    //        catch
    //        {
    //            // Ako pojedini film padne, preskači
    //            continue;
    //        }
    //    }
    //}



    public async Task<List<TvShow>> GetAllTvShowsInFolderAsync(string basePath, CancellationToken cancellationToken = default)
    {
        var tvShows = new List<TvShow>();

        // 1. Pronađi sve foldere sa TV serijama
        var tvShowFolders = await GetTvShowFoldersAsync(basePath, cancellationToken);

        // 2. Za svaki folder parsiraj seriju i epizode
        foreach (var folderPath in tvShowFolders)
        {
            try
            {
                var tvShow = await ParseTvShowFromFolderAsync(folderPath, cancellationToken);
                if (tvShow != null && tvShow.Episodes.Any()) tvShows.Add(tvShow);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing folder {folderPath}: {ex.Message}");
                // Možemo kreirati TvShow sa minimalnim podacima ili preskočiti
            }
        }

        return tvShows;
    }

    async Task<List<string>> GetTvShowFoldersAsync(string basePath, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Directory.GetDirectories(basePath)
                .Select(Path.GetFullPath)
                .ToList();
        }, cancellationToken);
    }

    async Task<TvShow> ParseTvShowFromFolderAsync(string folderPath, CancellationToken cancellationToken)
    {
        var folderName = Path.GetFileName(folderPath);

        // 1. PARSIRAJ FOLDER - kreiraj TvShow
        var tvShow = ParseFolderName(folderName);

        // 2. PARSIRAJ SVE EPIZODE U FOLDERU
        var episodes = await ParseEpisodesInFolderAsync(folderPath, tvShow, cancellationToken);
        tvShow.Episodes = episodes;

        return tvShow;
    }

    public async Task<TvShow> AddTvShowFromFolderAsync(string folderPath, CancellationToken cancellationToken = default)
    {
        var folderName = Path.GetFileName(folderPath);

        // 1. PARSIRAJ FOLDER
        var tvShow = ParseFolderName(folderName);

        // 2. PARSIRAJ SVE EPIZODE U FOLDERU
        var episodes = await ParseEpisodesInFolderAsync(folderPath, tvShow, cancellationToken);
        tvShow.Episodes = episodes;

        return tvShow;
    }

    TvShow ParseFolderName(string folderName)
    {
        var tvShow = new TvShow { FolderName = folderName };
        var tokens = Regex.Split(folderName, @"[.\s_-]+")
                          .Where(t => !string.IsNullOrWhiteSpace(t))
                          .ToList();

        // Traži godinu (1900-2099) i TMDB ID
        for (int i = 0; i < tokens.Count; i++)
        {
            if (int.TryParse(tokens[i], out int number))
            {
                if (number >= 1900 && number <= 2099)
                {
                    tvShow.Year = number;
                    // Naslov je sve prije godine
                    tvShow.Title = string.Join(" ", tokens.Take(i));

                    // Provjeri ima li TMDB ID nakon godine
                    if (i + 1 < tokens.Count && int.TryParse(tokens[i + 1], out int tmdbId) && tmdbId > 0) tvShow.TmdbId = tmdbId;

                    break;
                }
            }
        }

        // Fallback ako nije pronađena godina
        if (string.IsNullOrEmpty(tvShow.Title)) tvShow.Title = folderName;

        // Formatiranje naslova
        tvShow.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tvShow.Title.ToLowerInvariant().Trim());

        return tvShow;
    }

    async Task<List<TvShowEpisode>> ParseEpisodesInFolderAsync(string folderPath, TvShow tvShow, CancellationToken cancellationToken)
    {
        var episodes = new List<TvShowEpisode>();
        var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mkv", ".mp4", ".avi" };

        var files = await Task.Run(() =>
        {
            return Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => extensions.Contains(Path.GetExtension(file)))
                .Select(Path.GetFileName)
                .ToList();
        }, cancellationToken);

        foreach (var fileName in files)
        {
            var episode = ParseEpisodeFileName(fileName, tvShow);
            episodes.Add(episode);
        }

        return episodes.OrderBy(e => e.Season).ThenBy(e => e.Episode).ToList();
    }

    TvShowEpisode ParseEpisodeFileName(string fileName, TvShow tvShow)
    {
        var episode = new TvShowEpisode
        {
            FileName = fileName,
            TvShow = tvShow
        };

        var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        var tokens = Regex.Split(nameWithoutExt, @"[.\s_-]+")
                          .Where(t => !string.IsNullOrWhiteSpace(t))
                          .ToList();

        // --- DETEKCIJA SEASON/EPISODE ---
        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i].ToUpperInvariant();

            // Standardni pattern S01E01
            if (token.StartsWith("S") && token.Contains("E"))
            {
                var match = Regex.Match(token, @"S(\d+)E(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    episode.Season = int.Parse(match.Groups[1].Value);
                    episode.Episode = int.Parse(match.Groups[2].Value);

                    // Episode title je sljedeći token (ako postoji i nije tehnički tag)
                    if (i + 1 < tokens.Count && !IsTechnicalTag(tokens[i + 1]))
                    {
                        episode.EpisodeTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tokens[i + 1].ToLowerInvariant().Trim());
                    }
                    break;
                }
            }

            // Pattern S01 E01 (odvojeno)
            if (token == "S" && i + 1 < tokens.Count)
            {
                if (int.TryParse(tokens[i + 1], out int season))
                {
                    episode.Season = season;

                    // Traži E/episode
                    if (i + 2 < tokens.Count && tokens[i + 2].ToUpperInvariant().StartsWith("E"))
                    {
                        var episodeToken = tokens[i + 2].ToUpperInvariant();
                        if (int.TryParse(episodeToken.Substring(1), out int episodeNum))
                        {
                            episode.Episode = episodeNum;

                            // Episode title
                            if (i + 3 < tokens.Count && !IsTechnicalTag(tokens[i + 3]))
                            {
                                episode.EpisodeTitle = string.Join(" ", tokens.Skip(i + 3).TakeWhile(t => !IsTechnicalTag(t)));
                                episode.EpisodeTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(episode.EpisodeTitle.ToLowerInvariant().Trim());
                            }
                        }
                    }
                    break;
                }
            }
        }

        // --- DETEKCIJA TEHNIČKIH TAGOVA ---
        var technicalTags = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
        {
            // Resolution
            ["480P"] = _ => episode.Resolution = "480p",
            ["720P"] = _ => episode.Resolution = "720p",
            ["1080P"] = _ => episode.Resolution = "1080p",
            ["2160P"] = _ => episode.Resolution = "2160p",
            ["4K"] = _ => episode.Resolution = "2160p",

            // Source
            ["WEBRIP"] = _ => episode.Source = "WEBRip",
            ["WEB-DL"] = _ => episode.Source = "WEB-DL",
            ["BLURAY"] = _ => episode.Source = "BluRay",
            ["BRRIP"] = _ => episode.Source = "BRRip",

            // Audio
            ["2CH"] = _ => episode.Audio = "2CH",
            ["6CH"] = _ => episode.Audio = "6CH",
            ["8CH"] = _ => episode.Audio = "8CH",
            ["DTS"] = _ => episode.Audio = "DTS",
            ["AAC"] = _ => episode.Audio = "AAC",

            // Video
            ["X264"] = _ => episode.Video = "x264",
            ["X265"] = _ => episode.Video = "x265",
            ["HEVC"] = _ => episode.Video = "HEVC",

            // Release
            ["PSA"] = _ => episode.Release = "PSA",
            ["UTR"] = _ => episode.Release = "UTR",
            ["RARBG"] = _ => episode.Release = "RARBG"
        };

        foreach (var token in tokens)
        {
            if (technicalTags.TryGetValue(token.ToUpperInvariant(), out var action)) action(token);
        }

        return episode;
    }

    bool IsTechnicalTag(string token)
    {
        var technicalKeywords = new HashSet<string> {
            "480P", "720P", "1080P", "2160P", "4K", "8BIT", "10BIT", "HDR",
            "WEBRIP", "WEB-DL", "BLURAY", "BRRIP", "HDRIP", "DVDRIP",
            "2CH", "6CH", "8CH", "DTS", "AAC", "AC3", "FLAC",
            "X264", "X265", "HEVC", "AV1",
            "PSA", "UTR", "RARBG", "YIFY", "SPARKS"
        };

        return technicalKeywords.Contains(token.ToUpperInvariant());
    }
}