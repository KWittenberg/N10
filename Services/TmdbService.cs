namespace N10.Services;

public class TmdbService(HttpClient client, IOptions<TmdbOptions> options) : ITmdbService
{
    public async Task<TmdbSearch?> GetMovieAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true)
    {
        var yearParam = string.IsNullOrWhiteSpace(year) ? string.Empty : $"&year={year}";
        var url = $"{options.Value.BaseUrl}search/movie?query={Uri.EscapeDataString(query)}&include_adult={includeAdult}&language={language}&page=1{yearParam}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TmdbSearch>();
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








    //readonly JsonSerializerOptions jsonSerializerOptions = new()
    //{
    //    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    //};

    //public async Task<TmdbSearchResponse?> GetMovieAsync(string query, string? year, string language = "en-US", bool includeAdult = true)
    //{
    //    if (string.IsNullOrWhiteSpace(year)) year = $"&year={year}";

    //    var response = await client.GetAsync($"{options.Value.BaseUrl}search/movie?query={query}&include_adult={includeAdult}&language={language}&page=1{year}");

    //    return await response.Content.ReadFromJsonAsync<TmdbSearchResponse>();
    //}

    //public async Task<MovieDetails> GetMovieDetails(int? id)
    //{
    //    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

    //    var baseAddress = new Uri("http://api.themoviedb.org/3/");

    //    using (var httpClient = new HttpClient { BaseAddress = baseAddress })
    //    {
    //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

    //        using (var response = await httpClient.GetAsync("movie/" + id + "?api_key=" + TMDBApiKey.ApiKey + "&language=en-US"))
    //        {
    //            string responseData = await response.Content.ReadAsStringAsync();
    //            var model = JsonConvert.DeserializeObject<MovieDetails>(responseData);
    //            return model;
    //        }
    //    }
    //}

    //public async Task<List<Movies>> GetPopularMovies()
    //{
    //    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

    //    var baseAddress = new Uri("http://api.themoviedb.org/3/");

    //    using (var httpClient = new HttpClient { BaseAddress = baseAddress })
    //    {
    //        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

    //        using (var response = await httpClient.GetAsync("movie/popular?api_key=" + TMDBApiKey.ApiKey + "&language=en-US&page="))
    //        {
    //            string responseData = await response.Content.ReadAsStringAsync();
    //            var model = JsonConvert.DeserializeObject<PopularMovies>(responseData);
    //            return model.results;
    //        }
    //    }
    //}

}