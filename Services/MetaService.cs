namespace N10.Services;

public class MetaService(HttpClient client, IOptions<MetaOptions> options)
{
    public async Task<Result> PostImageFromMemoryAsync(string message, byte[] imageBytes)
    {
        using var content = new MultipartFormDataContent();

        // 1. Dodaj poruku (Caption)
        content.Add(new StringContent(message), "message");

        // 2. Dodaj sliku (Bajtovi)
        // "source" je naziv parametra koji Facebook traži za upload fajla
        var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        content.Add(imageContent, "source", $"vremeplov-{DateTime.Now:dd.MM.yyyy-HH.mm}.png");

        // 3. Dodaj flag da se odmah objavi
        content.Add(new StringContent("true"), "published");

        // 4. Pošalji na /photos endpoint
        var response = await client.PostAsync($"{options.Value.BaseUrl}{options.Value.BoltaId}/photos", content);

        if (response.IsSuccessStatusCode) return Result.Ok($"Facebook Image Posted! - {DateTime.Now:f}");
        else return Result.Error(await response.Content.ReadAsStringAsync());
    }
}