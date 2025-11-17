namespace N10.Repository;

public class MovieRepository(IDbContextFactory<ApplicationDbContext> context) : IMovieRepository
{
    readonly string entityName = "Movie";

    public async Task<Result<List<MovieDto>>> GetAllAsync()
    {
        await using var db = await context.CreateDbContextAsync();

        var dtos = await db.Movies.AsNoTracking().OrderBy(x => x.SortTitle).Select(MovieMapping.ToDtoExpression).ToListAsync();
        if (dtos.Count == 0) return Result<List<MovieDto>>.Error($"{entityName} Not Found!");

        return Result<List<MovieDto>>.Ok(dtos);
    }



















}