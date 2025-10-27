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
        var connectionIndex = 3; // Set: [0]Development [1]Remote [2]Production [3]PostgreSQL [4]MySQL
        var connectionName = new[] { "Development", "Remote", "Production", "PostgreSQL", "MySQL" };
        var connectionString = configuration.GetConnectionString(connectionName[connectionIndex]) ?? throw new InvalidOperationException("Connection not found!");

        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            if (connectionIndex == 3) options.UseNpgsql(connectionString);
            // else if (connectionIndex == 4) options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            else options.UseSqlServer(connectionString);
        }, ServiceLifetime.Scoped);

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