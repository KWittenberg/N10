namespace N10.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServiceExtensions(this IServiceCollection services, IConfiguration configuration)
    {
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


        // ✅ Add database
        var index = configuration.GetValue<int>("Database:Index", 0);
        var names = configuration.GetSection("Database:Names").Get<string[]>() ?? throw new InvalidOperationException("Missing database names!");
        var cs = configuration.GetConnectionString(names[index]) ?? throw new InvalidOperationException("Connection not found!");

        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            // if (index == 3 || index == 4) options.UseNpgsql(cs);
            // else if (index == 5) options.UseMySql(cs, ServerVersion.AutoDetect(cs));
            // else options.UseSqlServer(cs);
            options.UseSqlServer(cs);
        });

        // ✅ Ovaj transient je ključan za Blazor Server!
        // Provide ApplicationDbContext via the factory as TRANSIENT so Identity gets a fresh context per injection
        services.AddTransient(sp => sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

        // services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);
        // services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        // ✅ Identity koristi transient DbContext
        //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString),
        //    contextLifetime: ServiceLifetime.Transient,  // 👈 OVO JE KLJUČNO!
        //    optionsLifetime: ServiceLifetime.Singleton);

        services.AddDatabaseDeveloperPageExceptionFilter();

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
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();


        return services;
    }
}