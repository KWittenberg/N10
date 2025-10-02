namespace N10.Services;

public static class ConfigureValidators
{
    public static IServiceCollection AddConfigureValidators(this IServiceCollection services)
    {
        // ovo radi, ali nije flexible! Vezan si za konkretnu klasu, ne možeš lako mockati u unit testovima.
        //services.AddScoped<RoleInputValidator>();
        //services.AddScoped<UserInputValidator>();


        // koristiš abstraction (SOLID princip)
        services.AddScoped<IValidator<RoleInput>, RoleInputValidator>();
        //services.AddScoped<IValidator<UserInput>, UserInputValidator>();


        return services;
    }
}