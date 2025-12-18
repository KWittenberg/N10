namespace N10.Services;

public class TmdbService(HttpClient client, IOptions<TmdbOptions> options) : ITmdbService
{

    #region MOVIES
    public async Task<TmdbSearchList?> SearchMoviesAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true)
    {
        var yearParam = string.IsNullOrWhiteSpace(year) ? string.Empty : $"&year={year}";
        var relativeUrl = $"search/movie?query={Uri.EscapeDataString(query)}&include_adult={includeAdult}&language={language}&page=1{yearParam}";

        return await client.GetFromJsonAsync<TmdbSearchList>(relativeUrl);
    }

    public async Task<TmdbDetails?> GetMovieByIdAsync(int id, string? language = "en-US")
    {
        return await client.GetFromJsonAsync<TmdbDetails>($"movie/{id}?language={language}");
    }

    public async Task<TmdbCredits?> GetMovieCreditsByIdAsync(int id, string? language = "en-US")
    {
        var credits = await client.GetFromJsonAsync<TmdbCredits>($"movie/{id}/credits?language={language}");

        if (credits is not null)
        {
            foreach (var castMember in credits.Casts ?? [])
            {
                castMember.ProfilePath = string.IsNullOrEmpty(castMember.ProfilePath)
                    ? "/images/profile.jpg"
                    : $"{options.Value.BaseImageUrl}{castMember.ProfilePath}"; // Tu koristimo options za slike
            }
            foreach (var crewMember in credits.Crews)
            {
                crewMember.ProfilePath = string.IsNullOrEmpty(crewMember.ProfilePath)
                    ? "/images/profile.jpg"
                    : $"https://image.tmdb.org/t/p/w500{crewMember.ProfilePath}";
            }
        }

        return credits;
    }

    public async Task<TmdbVideo?> GetMovieTrailerByIdAsync(int id, string? language = "en-US")
    {
        var videos = await client.GetFromJsonAsync<TmdbVideos>($"movie/{id}/videos?language={language}");

        return videos?.Videos.FirstOrDefault(x =>
            x.Site != null && x.Site.Contains("YouTube", StringComparison.OrdinalIgnoreCase) &&
            x.Type != null && x.Type.Contains("Trailer", StringComparison.OrdinalIgnoreCase));
    }
    #endregion


    #region TV SERIES
    public async Task<TmdbTvShowSearchList?> SearchTvShowsAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true)
    {
        var yearParam = string.IsNullOrWhiteSpace(year) ? string.Empty : $"&year={year}";
        var relativeUrl = $"search/tv?query={Uri.EscapeDataString(query)}&include_adult={includeAdult}&language={language}&page=1{yearParam}";

        return await client.GetFromJsonAsync<TmdbTvShowSearchList>(relativeUrl);
    }

    public async Task<TmdbTvShowDetails?> GetTvShowByIdAsync(int id, string? language = "en-US")
    {
        return await client.GetFromJsonAsync<TmdbTvShowDetails>($"tv/{id}?language={language}");
    }

    //public async Task<TmdbTvShowSearchList?> SearchTvShowsAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true)
    //{
    //    var yearParam = string.IsNullOrWhiteSpace(year) ? string.Empty : $"&year={year}";
    //    var url = $"{options.Value.BaseUrl}search/tv?query={Uri.EscapeDataString(query)}&include_adult={includeAdult}&language={language}&page=1{yearParam}";
    //    var response = await client.GetAsync(url);
    //    response.EnsureSuccessStatusCode();

    //    return await response.Content.ReadFromJsonAsync<TmdbTvShowSearchList>();
    //}

    //public async Task<TmdbTvShowDetails?> GetTvShowByIdAsync(int id, string? language = "en-US")
    //{
    //    var response = await client.GetAsync($"{options.Value.BaseUrl}tv/{id}&language={language}");
    //    response.EnsureSuccessStatusCode();

    //    return await response.Content.ReadFromJsonAsync<TmdbTvShowDetails>();
    //}

    public async Task<TmdbCredits?> GetTvShowCreditsByIdAsync(int id, string? language = "en-US")
    {
        var response = await client.GetAsync($"{options.Value.BaseUrl}tv/{id}/credits?language={language}");
        response.EnsureSuccessStatusCode();

        var credits = await response.Content.ReadFromJsonAsync<TmdbCredits>();

        foreach (var castMember in credits?.Casts!)
        {
            castMember.ProfilePath = string.IsNullOrEmpty(castMember.ProfilePath)
                ? "/images/profile.jpg"
                : $"https://image.tmdb.org/t/p/w500{castMember.ProfilePath}";
        }

        foreach (var crewMember in credits.Crews)
        {
            crewMember.ProfilePath = string.IsNullOrEmpty(crewMember.ProfilePath)
                ? "/images/profile.jpg"
                : $"https://image.tmdb.org/t/p/w500{crewMember.ProfilePath}";
        }

        return credits;
    }

    public async Task<TmdbVideo?> GetTvShowTrailerByIdAsync(int id, string? language = "en-US")
    {
        var url = $"{options.Value.BaseUrl}tv/{id}/videos?language={language}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var videos = await response.Content.ReadFromJsonAsync<TmdbVideos>();

        return videos?.Videos.FirstOrDefault(x => x.Site!.Contains("YouTube", StringComparison.OrdinalIgnoreCase)
                                               && x.Type!.Contains("Trailer", StringComparison.OrdinalIgnoreCase));
    }
    #endregion


    #region OLD METHODS
    //public async Task<TmdbSearchList?> SearchMoviesAsync(string query, string? year = null, string? language = "en-US", bool includeAdult = true)
    //{
    //    var yearParam = string.IsNullOrWhiteSpace(year) ? string.Empty : $"&year={year}";
    //    var url = $"{options.Value.BaseUrl}search/movie?query={Uri.EscapeDataString(query)}&include_adult={includeAdult}&language={language}&page=1{yearParam}";
    //    var response = await client.GetAsync(url);
    //    response.EnsureSuccessStatusCode();

    //    return await response.Content.ReadFromJsonAsync<TmdbSearchList>();
    //}

    //public async Task<TmdbDetails?> GetMovieByIdAsync(int id, string? language = "en-US")
    //{
    //    var response = await client.GetAsync($"{options.Value.BaseUrl}movie/{id}&language={language}");
    //    response.EnsureSuccessStatusCode();

    //    var movie = await response.Content.ReadFromJsonAsync<TmdbDetails>();

    //    //movie?.PosterPath = string.IsNullOrEmpty(movie.PosterPath)
    //    //                ? "/images/poster.png"
    //    //                : $"{options.Value.BaseImageUrl}{movie.PosterPath}";

    //    //movie?.BackdropPath = string.IsNullOrEmpty(movie.BackdropPath)
    //    //                   ? "/images/backdrop.jpg"
    //    //                   : $"{options.Value.BaseImageUrl}{movie.BackdropPath}";

    //    return movie;
    //}

    //public async Task<TmdbCredits?> GetMovieCreditsByIdAsync(int id, string? language = "en-US")
    //{
    //    var response = await client.GetAsync($"{options.Value.BaseUrl}movie/{id}/credits?language={language}");
    //    response.EnsureSuccessStatusCode();

    //    var credits = await response.Content.ReadFromJsonAsync<TmdbCredits>();

    //    foreach (var castMember in credits?.Casts!)
    //    {
    //        castMember.ProfilePath = string.IsNullOrEmpty(castMember.ProfilePath)
    //            ? "/images/profile.jpg"
    //            : $"https://image.tmdb.org/t/p/w500{castMember.ProfilePath}";
    //    }

    //    foreach (var crewMember in credits.Crews)
    //    {
    //        crewMember.ProfilePath = string.IsNullOrEmpty(crewMember.ProfilePath)
    //            ? "/images/profile.jpg"
    //            : $"https://image.tmdb.org/t/p/w500{crewMember.ProfilePath}";
    //    }

    //    return credits;
    //}

    //public async Task<TmdbVideo?> GetMovieTrailerByIdAsync(int id, string? language = "en-US")
    //{
    //    var url = $"{options.Value.BaseUrl}movie/{id}/videos?language={language}";
    //    var response = await client.GetAsync(url);
    //    response.EnsureSuccessStatusCode();

    //    var videos = await response.Content.ReadFromJsonAsync<TmdbVideos>();

    //    return videos?.Videos.FirstOrDefault(x => x.Site!.Contains("YouTube", StringComparison.OrdinalIgnoreCase)
    //                                           && x.Type!.Contains("Trailer", StringComparison.OrdinalIgnoreCase));
    //}
    #endregion
}