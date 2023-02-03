namespace ConferencePlanner.Service.ProgramConfiguration;

internal static class ApplicationBuilderExtensions
{
    public static void ConfigureApplication(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(routeBuilder => { routeBuilder.MapGraphQL(); });
    }
}