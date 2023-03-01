namespace ConferencePlanner.Service.ProgramConfiguration;

internal static class ConfigurationBuilderExtensions
{
    private const string SecretAppsettingsJson = "secrets/appsettings.json";

    public static void ConfigureConfiguration(this IConfigurationBuilder builder)
    {
        builder.AddJsonFile(ConfigurationBuilderExtensions.SecretAppsettingsJson, true, false);
    }
}