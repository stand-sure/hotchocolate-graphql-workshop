namespace ConferencePlanner.Service.ProgramConfiguration;

using ConferencePlanner.Data;

using Microsoft.EntityFrameworkCore;

using Serilog;

internal static class ServiceCollectionExtensions
{
    private const string ConnectionStringName = "ConferencePlanner";
    private const string MigrationsAssemblyName = "ConferencePlannerService";

    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddLogging(builder => builder.AddSerilog());
        services.AddDbContextFactory<ApplicationDbContext>((_, builder) => { builder.ConfigureDbContextFactory(configuration); });

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("Startup", tags: new[] { "ready" });

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

    public static DbContextOptionsBuilder ConfigureDbContextFactory(this DbContextOptionsBuilder builder, IConfiguration configuration)
    {
        builder.UseNpgsql(configuration.GetConnectionString(ServiceCollectionExtensions.ConnectionStringName),
            contextOptionsBuilder => contextOptionsBuilder.MigrationsAssembly(ServiceCollectionExtensions.MigrationsAssemblyName));

        return builder;
    }
}