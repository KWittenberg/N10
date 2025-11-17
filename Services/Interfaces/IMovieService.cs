namespace N10.Services.Interfaces;

public interface IMovieService
{
    Task<List<Movie>> GetAllMoviesAsync(CancellationToken cancellationToken = default);
}