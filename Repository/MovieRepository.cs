namespace N10.Repository;

public class MovieRepository(IDbContextFactory<ApplicationDbContext> context,
                                IMovieService movieService,
                                ITmdbService tmdbService) : IMovieRepository
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




    public async Task SyncParsedMoviesAsync()
    {
        List<Movie> parsed = await movieService.GetAllMoviesAsync();
        if (parsed.Count == 0) return;

        await using var db = await context.CreateDbContextAsync();

        var dbMovies = await db.Movies.ToListAsync();

        // HASH SET radi brže usporedbe
        var parsedFilesSet = parsed.Select(x => x.FileName).ToHashSet();

        // A) DELETE - file više ne postoji
        foreach (var dbMovie in dbMovies)
        {
            if (!parsedFilesSet.Contains(dbMovie.FileName)) db.Movies.Remove(dbMovie);
        }

        // B) INSERT / UPDATE
        foreach (var p in parsed)
        {
            var dbMovie = await db.Movies.FirstOrDefaultAsync(x => x.FileName == p.FileName);

            if (dbMovie is null)
            {
                // Add
                var newMovie = new Movie
                {
                    FileName = p.FileName,
                    Title = p.Title,
                    SortTitle = CreateSortTitle(p.Title) ?? string.Empty,
                    Year = p.Year,
                    Version = p.Version,
                    Resolution = p.Resolution,
                    Color = p.Color,
                    Source = p.Source,
                    Audio = p.Audio,
                    Video = p.Video,
                    Release = p.Release,
                    TmdbId = p.TmdbId,
                    IsMetadataFetched = false
                };

                await db.Movies.AddAsync(newMovie);
            }
            else
            {
                // update (ako treba)
                bool changed = false;

                if (dbMovie.Title != p.Title)
                {
                    dbMovie.Title = p.Title;
                    changed = true;
                }

                if (dbMovie.SortTitle != p.SortTitle)
                {
                    dbMovie.SortTitle = p.SortTitle;
                    changed = true;
                }

                if (dbMovie.Year != p.Year)
                {
                    dbMovie.Year = p.Year;
                    changed = true;
                }

                if (changed)
                {
                    dbMovie.IsMetadataFetched = false;
                    db.Movies.Update(dbMovie);
                }
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

            foreach (var g in details.Genres)
            {
                if (!movie.Genres.Any(x => x.TmdbId == g.Id))
                {
                    movie.Genres.Add(new MovieGenre
                    {
                        TmdbId = g.Id,
                        TmdbName = g.Name
                    });
                }

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

                if (movie.TmdbId is not null) details = await tmdbService.GetMovieByIdAsync(movie.TmdbId.Value);

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
                    ? "img/NoImage.webp"
                    : details.PosterPath;

                // Genres
                movie.Genres.Clear();

                foreach (var g in details.Genres)
                {
                    movie.Genres.Add(new MovieGenre
                    {
                        TmdbId = g.Id,
                        TmdbName = g.Name
                    });
                }

                movie.IsMetadataFetched = true;

                // SAVE
                db.Movies.Update(movie);
                await db.SaveChangesAsync();

                // 5) DELAY (anti-rate-limit)
                await Task.Delay(300);
            }
            catch
            {
                // ako pojedini film padne, preskači
                continue;
            }
        }
    }

}