namespace ConferencePlanner.Service.ProgramConfiguration;

internal static class ConfigurationBuilderExtensions
{
    private const string AppsettingsJson = "appsettings.json";
    private const string SecretAppsettingsJson = $"secrets/{ConfigurationBuilderExtensions.AppsettingsJson}";

    public static void ConfigureConfiguration(this IConfigurationBuilder builder)
    {
        builder.AddJsonFile(ConfigurationBuilderExtensions.AppsettingsJson, false, false);
        builder.AddJsonFile(ConfigurationBuilderExtensions.SecretAppsettingsJson, true, false);
    }
}