var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add Identity, Repositories, Services, Localization, Validators, Seed 
services.AddServiceCollectionExtensions(configuration);


#region ---> app
var app = builder.Build();

// START Seed Startup Data
await app.Services.SeedDataAsync();

app.UseMiddlewaresExtensions();

app.Run();
#endregion