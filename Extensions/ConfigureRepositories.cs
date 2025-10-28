﻿namespace N10.Extensions;

public static class ConfigureRepositories
{
    public static IServiceCollection AddRepositoriesExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<INoteRepository, NoteRepository>();


        return services;
    }
}