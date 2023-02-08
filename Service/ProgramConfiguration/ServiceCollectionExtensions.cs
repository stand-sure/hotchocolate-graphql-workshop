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

        services.AddGraphQl();
        services.AddInstrumentation(environment, configuration);
    }
}