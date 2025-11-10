namespace N10.Services.Interfaces;

public interface ITmdbService
{
    Task<TmdbSearch?> GetMovieAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true);

    Task<TmdbDetails?> GetMovieByIdAsync(int? id);
}