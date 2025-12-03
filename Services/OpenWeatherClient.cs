namespace N10.Services;

public class OpenWeatherClient(HttpClient client, IOptions<OpenWeatherOptions> weatherOptions)
{
    public async Task<OpenWeatherModel?> GetWeatherAsync([Description("The City Name to get the weather for.")] string cityName)
    {
        string query = $"{weatherOptions.Value.BaseUrl}weather?q={cityName}&units=metric&appid={weatherOptions.Value.ApiKey}";

        var response = await client.GetAsync(query);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OpenWeatherModel>();
    }
}