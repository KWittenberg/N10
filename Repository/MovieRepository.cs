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
        var dtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(MovieMapping.ToDtoExpression)
            .ToListAsync();

        if (dtos.Count == 0 && pageNumber == 1) return Result<PaginatedResult<MovieDto>>.Error("Nema pronađenih filmova sa zadanim filterima!");

        return Result<PaginatedResult<MovieDto>>.Ok(new PaginatedResult<MovieDto>(dtos, pageNumber, pageSize, totalCount));
    }

    public async Task<Result<FilterOptionsDto>> GetFilterOptionsAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        var options = new FilterOptionsDto
        {
            AvailableYears = await db.Movies
                .Where(m => m.Year.HasValue)
                .Select(m => m.Year.Value)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync(),

            AvailableGenres = await db.MovieGenres
                .Select(mg => mg.TmdbName)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync(),

            AvailableResolutions = await db.Movies
                .Where(m => m.Resolution != null)
                .Select(m => m.Resolution)
                .Distinct()
                .OrderBy(r => r)
                .ToListAsync()
        };

        return Result<FilterOptionsDto>.Ok(options);
    }



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







    public async Task PopulateFromTmdbAsyncOLD()
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