namespace N10.Services;

public static class ConfigureCulture
{
    public static IServiceCollection AddConfigureCulture(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo> { new("hr-HR") };
            options.DefaultRequestCulture = new RequestCulture("hr-HR");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        return services;
    }
}