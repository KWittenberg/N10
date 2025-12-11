namespace N10.Extensions;

public static class ConfigureLocalization
{
    public static IServiceCollection AddLocalizationExtensions(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "hr-HR", "en-US", "de-DE", "it-IT" };
            options.SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
        });

        return services;
    }

    public static void AddUseRequestLocalization(this WebApplication app)
    {
        app.UseRequestLocalization(new RequestLocalizationOptions()
            .AddSupportedCultures(new[] { "hr-HR", "en-US", "de-DE", "it-IT" })
            .AddSupportedUICultures(new[] { "hr-HR", "en-US", "de-DE", "it-IT" }));


        //app.UseRequestLocalization(app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value);

        //app.UseRequestLocalization(localizationOptions); // For Localization!
        //app.UseRequestLocalization(); // For Localization!
    }


    // Middleware to handle culture parameter in query string
    //public static void AddLocalizationMiddleware(this WebApplication app)
    //{
    //    app.Use(async (context, next) =>
    //    {
    //        var cultureQuery = context.Request.Query["culture"];
    //        if (!string.IsNullOrEmpty(cultureQuery))
    //        {
    //            var culture = new CultureInfo(cultureQuery);
    //            CultureInfo.CurrentCulture = culture;
    //            CultureInfo.CurrentUICulture = culture;
    //        }

    //        await next(context);
    //    });
    //}
}