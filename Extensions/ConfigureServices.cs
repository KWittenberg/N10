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

        services.AddScoped<IMovieLibraryService, MovieLibraryService>();

        return services;
    }
}