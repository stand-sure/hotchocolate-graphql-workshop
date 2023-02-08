namespace ConferencePlanner.Service.ProgramConfiguration;

internal static class WebApplicationBuilderExtensions
{
    public static void ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder.Logging.ConfigureLogging();
        builder.Configuration.ConfigureConfiguration();
        builder.Services.ConfigureServices(builder.Configuration, builder.Environment);
    }
}