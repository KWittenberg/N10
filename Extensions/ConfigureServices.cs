namespace N10.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailService, EmailService>();

        //services.AddScoped<HtmlRenderer>();
        //services.AddScoped<IHtmlToStringService, HtmlToStringService>();


        services.AddScoped<IDashboardService, DashboardService>();

        //services.AddScoped<ICurrentUserService, CurrentUserService>();
        //services.AddScoped<AppState>(); // services.AddSingleton<AppState>();
        //services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        //services.AddTransient<ISeedService, SeedService>();

        services.AddScoped<IMovieService, MovieService>();

        services.AddTransient<TmdbAuthenticationHandler>();
        services.AddHttpClient<ITmdbService, TmdbService>().AddHttpMessageHandler<TmdbAuthenticationHandler>();

        // Alternative way to configure HttpClient with BaseAddress from configuration
        //services.AddHttpClient<ITmdbService, TmdbService>(client =>
        //{
        //    client.BaseAddress = new Uri(configuration["TmdbOptions:BaseUrl"]!);
        //}).AddHttpMessageHandler<TmdbAuthenticationHandler>();

        return services;
    }
}