using Microsoft.AspNetCore.Components.Web;

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


        services.AddScoped<IMovieService, MovieService>();

        services.AddTransient<TmdbAuthenticationHandler>();
        services.AddHttpClient<ITmdbService, TmdbService>().AddHttpMessageHandler<TmdbAuthenticationHandler>();


        // Alternative way to configure HttpClient with BaseAddress from configuration
        //services.AddHttpClient<ITmdbService, TmdbService>(client =>
        //{
        //    client.BaseAddress = new Uri(configuration["TmdbOptions:BaseUrl"]!);
        //}).AddHttpMessageHandler<TmdbAuthenticationHandler>();



        services.AddScoped<LocalAiService>();

        return services;
    }
}