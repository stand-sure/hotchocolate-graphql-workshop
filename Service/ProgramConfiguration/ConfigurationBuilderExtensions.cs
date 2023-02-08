namespace ConferencePlanner.Service.ProgramConfiguration;

internal static class ConfigurationBuilderExtensions
{
    private const string AppsettingsJson = "appsettings.json";

    public static void ConfigureConfiguration(this IConfigurationBuilder builder)
    {
        builder.AddJsonFile(ConfigurationBuilderExtensions.AppsettingsJson);
    }
}