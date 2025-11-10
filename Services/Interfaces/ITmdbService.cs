namespace N10.Services.Interfaces;

public interface ITmdbService
{
    Task<TmdbMovieList?> SearchMoviesAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true);

    Task<TmdbDetails?> GetMovieByIdAsync(int id, string? language = "en-US");
}