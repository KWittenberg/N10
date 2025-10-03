namespace N10.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

        services.AddQuickGridEntityFrameworkAdapter();

        services.Configure<AdminOptions>(configuration.GetSection(nameof(AdminOptions)));


        services.AddConfigureCulture();
        services.AddConfigureValidators();

        //services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AppState>(); // services.AddSingleton<AppState>();
        services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        services.AddTransient<ISeedService, SeedService>();



        return services;
    }
}