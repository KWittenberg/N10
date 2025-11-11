namespace N10.Services.Interfaces;

public interface ITmdbService
{
    Task<TmdbSearchList?> SearchMoviesAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true);

    Task<TmdbDetails?> GetMovieByIdAsync(int id, string? language = "en-US");

    Task<TmdbVideo?> GetMovieTrailerAsync(int id, string? language = "en-US");
}