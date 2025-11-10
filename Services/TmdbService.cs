namespace N10.Services;

public class TmdbService(HttpClient client, IOptions<TmdbOptions> options) : ITmdbService
{
    public async Task<TmdbMovieList?> SearchMoviesAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true)
    {
        var yearParam = string.IsNullOrWhiteSpace(year) ? string.Empty : $"&year={year}";
        var url = $"{options.Value.BaseUrl}search/movie?query={Uri.EscapeDataString(query)}&include_adult={includeAdult}&language={language}&page=1{yearParam}";
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TmdbMovieList>();
    }

    public async Task<TmdbDetails?> GetMovieByIdAsync(int? id)
    {
        var response = await client.GetAsync($"{options.Value.BaseUrl}movie/{id}&language=en-US");
        var movie = await response.Content.ReadFromJsonAsync<TmdbDetails>();

        movie?.PosterPath = string.IsNullOrEmpty(movie.PosterPath)
                        ? "/images/poster.png"
                        : $"http://image.tmdb.org/t/p/w500{movie.PosterPath}";

        movie?.BackdropPath = string.IsNullOrEmpty(movie.BackdropPath)
                           ? "/images/backdrop.jpg"
                           : $"http://image.tmdb.org/t/p/w500{movie.BackdropPath}";

        return movie;
    }
}