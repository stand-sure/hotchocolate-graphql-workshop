namespace ConferencePlanner.Service.ProgramConfiguration;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Prometheus;

using Serilog;

internal static class ApplicationBuilderExtensions
{
    public static void ConfigureApplication(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseHttpMetrics();

        app.UseEndpoints(routeBuilder =>
        {
            routeBuilder.MapGraphQL();

            routeBuilder.MapHealthChecks("/healthz/ready",
                new HealthCheckOptions()
                {
                    Predicate = registration => registration.Tags.Contains("ready"),
                });

            routeBuilder.MapHealthChecks("/healthz/live",
                new HealthCheckOptions()
                {
                    Predicate = _ => false,
                });

            routeBuilder.MapMetrics();
        });
    }
}