namespace N10.Extensions;

public static class SeedServiceExtensions
{
    public static async Task SeedDataAsync(this IServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
        await seedService.StartSeedAsync();
    }
}