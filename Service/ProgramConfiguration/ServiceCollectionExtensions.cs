namespace ConferencePlanner.Service.ProgramConfiguration;

using ConferencePlanner.Data;

using Microsoft.EntityFrameworkCore;

internal static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDbContextFactory<ApplicationDbContext>((_, builder) =>
        {
            builder.UseNpgsql(configuration.GetConnectionString("ConferencePlanner"),
                contextOptionsBuilder => contextOptionsBuilder.MigrationsAssembly("Service"));
        });

        services.AddInstrumentation(environment, configuration);
        services.ConfigureGraphServices(environment);
    }

    public static IServiceCollection ConfigureGraphServices(
        this IServiceCollection services,
        IWebHostEnvironment? environment = null)
    {
        services.AddGraphQl();

        return services;
    }
}