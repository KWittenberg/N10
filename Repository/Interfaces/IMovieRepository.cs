namespace N10.Repository.Interfaces;

public interface IMovieRepository
{
    Task<Result<List<MovieDto>>> GetAllAsync();

    Task<Result> DeleteAsync(Guid id);


    Task<Result> PopulateFromTmdbByIdAsync(Guid id);



    Task SyncParsedMoviesAsync();

    Task PopulateFromTmdbAsync();
}