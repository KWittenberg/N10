var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

//services.AddControllersWithViews();

// Add services to the container.
services.AddRazorComponents().AddInteractiveServerComponents();


// Add Repositories & Services
services.IdentityService(configuration);
services.AddRepositories(configuration);
services.AddService(configuration);



#region ---> app
var app = builder.Build();

app.UseRequestLocalization(); // For Localization!

await SeedDataAsync(app.Services); // Add for START Seed

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
#endregion

#region ---> START SeedService
static async Task SeedDataAsync(IServiceProvider provider)
{
    var scope = provider.CreateAsyncScope();
    var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
    await seedService.StartSeedAsync();
}
#endregion