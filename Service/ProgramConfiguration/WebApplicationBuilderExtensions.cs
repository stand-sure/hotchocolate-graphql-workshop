namespace ConferencePlanner.Service.ProgramConfiguration;

using Serilog;

internal static class WebApplicationBuilderExtensions
{
    public static void ConfigureBuilder(this WebApplicationBuilder builder)
    {
        builder.Configuration.ConfigureConfiguration();

        builder.Host.UseSerilog((context, provider, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider)
        );

        builder.Services.ConfigureServices(builder.Configuration, builder.Environment);
    }
}