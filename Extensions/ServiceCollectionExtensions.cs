namespace N10.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceCollectionExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Core framework services
        services.AddRazorComponents()
                .AddInteractiveServerComponents();
        //.AddHubOptions(options =>
        //{
        //    options.MaximumReceiveMessageSize = 50 * 1024 * 1024; // 10 MB limit (za prijenos slika iz JS-a)
        //});

        // For Production, set DetailedErrors to false
        services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
        services.AddLocalizationExtensions();

        // 2. Configuration & options
        services.Configure<AdminOptions>(configuration.GetSection(nameof(AdminOptions)));
        services.Configure<GmailOptions>(configuration.GetSection(nameof(GmailOptions)));
        services.Configure<TmdbOptions>(configuration.GetSection(nameof(TmdbOptions)));
        services.Configure<OpenWeatherOptions>(configuration.GetSection(nameof(OpenWeatherOptions)));
        services.Configure<MetaOptions>(configuration.GetSection(nameof(MetaOptions)));

        // 3. Infrastructure/platform services
        services.AddQuickGridEntityFrameworkAdapter();
        services.AddIdentityServiceExtensions(configuration);

        // 4. Application services & repositories
        services.AddRepositoriesExtensions(configuration);
        services.AddServiceExtensions(configuration);
        services.AddValidatorsExtensions();

        // 5. State & utility services
        services.AddScoped<AppState>();
        services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        services.AddTransient<ISeedService, SeedService>();

        return services;
    }
}