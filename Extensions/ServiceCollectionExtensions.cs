namespace N10.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceCollectionExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Core framework services
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
        services.AddLocalizationExtensions();

        // 2. Configuration & options
        services.Configure<AdminOptions>(configuration.GetSection(nameof(AdminOptions)));

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