namespace N10.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        #region EMAIL SERVICE CONFIGURATION
        services.AddScoped<IEmailService, EmailService>();

        // Required for HtmlToStringService
        services.AddScoped<HtmlRenderer>();
        // Service to render Blazor components to string
        services.AddScoped<HtmlToStringService>();
        #endregion



        //services.AddScoped<ICurrentUserService, CurrentUserService>();


        services.AddScoped<MovieService>();

        services.AddTransient<TmdbAuthenticationHandler>();
        //services.AddHttpClient<ITmdbService, TmdbService>().AddHttpMessageHandler<TmdbAuthenticationHandler>();

        services.AddHttpClient<ITmdbService, TmdbService>((serviceProvider, client) =>
            {
                // Dohvati opcije da izvučemo BaseUrl
                var tmdbOptions = serviceProvider.GetRequiredService<IOptions<TmdbOptions>>().Value;

                // Postavi Base Address OVDJE (npr. "https://api.themoviedb.org/3/")
                client.BaseAddress = new Uri(tmdbOptions.BaseUrl);
            })
            .AddHttpMessageHandler<TmdbAuthenticationHandler>() // Tvoj auth
            .AddStandardResilienceHandler(); // 🚀 N10 MAGIJA: Automatski retry, circuit breaker, timeout!




        // Alternative way to configure HttpClient with BaseAddress from configuration
        //services.AddHttpClient<ITmdbService, TmdbService>(client =>
        //{
        //    client.BaseAddress = new Uri(configuration["TmdbOptions:BaseUrl"]!);
        //}).AddHttpMessageHandler<TmdbAuthenticationHandler>();

        services.AddHttpClient<OpenWeatherClient>();

        //services.AddScoped<LocalAiService>();
        services.AddSingleton<LocalAiService>();

        //services.AddScoped<LocalAI>(provider => new LocalAI(
        //modelPath: @"C:\models\tinyllama-1.1b-chat-v1.0.Q4_K_M.gguf",
        //llamaPath: @"C:\llama\llama-cli.exe"));


        services.AddScoped<ChronicleService>();
        services.AddScoped<AiStudioService>();



        return services;
    }
}