namespace N10.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseMiddlewaresExtensions(this WebApplication app)
    {
        // 1. Exception handling and HSTS
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();

            // Apply migrations at startup
            //using var scope = app.Services.CreateScope();
            //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //await context.Database.MigrateAsync();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // 2. Status code pages
        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

        // 3. HTTPS redirection
        app.UseHttpsRedirection();

        // 4. Localization (culture from query string)
        app.AddLocalizationMiddleware();

        // 5. Antiforgery
        app.UseAntiforgery();

        // 6. Static files
        app.UseStaticFiles();
        app.MapStaticAssets();

        // 7. Authentication & Authorization
        //app.UseAuthentication();
        //app.UseAuthorization();

        // 8. Blazor endpoints
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        // 9. Identity endpoints
        // Add additional endpoints required by the Identity /Account Razor components.
        app.MapAdditionalIdentityEndpoints();

        return app;
    }
}



// Recommended Order for Middleware Registration
// The typical order for a Blazor Server app is:
// 1.	Exception handling (UseExceptionHandler, UseHsts)
// 2.	Migrations (dev only) (UseMigrationsEndPoint)
// 3.	Status code pages (UseStatusCodePagesWithReExecute)
// 4.	HTTPS redirection (UseHttpsRedirection)
// 5.	Localization (your AddMiddleware for culture, and UseRequestLocalization if used)
// 6.	Antiforgery (UseAntiforgery)
// 7.	Static files (MapStaticAssets)
// 8.	Blazor endpoints (MapRazorComponents<App>())
// 9.	Identity endpoints (MapAdditionalIdentityEndpoints)