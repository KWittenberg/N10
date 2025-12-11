namespace N10.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServiceExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        // ✅ 1. Authentication setup
        services.AddCascadingAuthenticationState();
        //services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        services.AddAuthorization();
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();


        // ✅ 2. Database Connection setup
        var index = configuration.GetValue<int>("Database:Index", 0);
        var names = configuration.GetSection("Database:Names").Get<string[]>() ?? throw new InvalidOperationException("Missing database names!");
        var cs = configuration.GetConnectionString(names[index]) ?? throw new InvalidOperationException("Connection not found!");


        // ✅ 3. GLAVNA STVAR: Registracija Factory-a - Koristimo Factory da možemo ručno stvarati contexte u Blazor komponentama
        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            // if (index == 3 || index == 4) options.UseNpgsql(cs);
            // else if (index == 5) options.UseMySql(cs, ServerVersion.AutoDetect(cs));
            // else options.UseSqlServer(cs);
            options.UseSqlServer(cs);
            // Opcija: EnableSensitiveDataLogging ako debugiraš
            // options.EnableSensitiveDataLogging(); 
        }, ServiceLifetime.Scoped);


        // ✅ 4. MOST: Transient Context za Identity - Ovaj transient je ključan za Blazor Server!
        // Ovo omogućava da Identity (koji traži DbContext, a ne Factory) dobije 
        // svježu instancu svaki put, umjesto da drži jednu "vječno" kroz Scoped.
        // services.AddTransient<ApplicationDbContext>(sp => sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
        services.AddTransient(sp => sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());


        // ✅ 5. Identity setup
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;

                //options.Password.RequiredLength = 3;
                //options.Password.RequireDigit = true;
                //options.Password.RequireNonAlphanumeric = false;

                //options.Lockout.MaxFailedAccessAttempts = 3;
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes((10));
            })
            .AddRoles<IdentityRole<Guid>>() // Add for Seed with Guid
            .AddEntityFrameworkStores<ApplicationDbContext>() // Ovdje Identity traži context, i dobije onaj Transient iz točke 4
            .AddSignInManager()
            .AddDefaultTokenProviders();

        // ✅ 6. Exception filter for database-related exceptions during development, need 'app.UseMigrationsEndPoint();'
        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }
}