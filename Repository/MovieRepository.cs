namespace N10.Repository;

public class MovieRepository(IDbContextFactory<ApplicationDbContext> context, IMovieService movieService) : IMovieRepository
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
}