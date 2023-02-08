namespace ConferencePlanner.Service.ProgramConfiguration;

using Serilog;

internal static class LoggingBuilderExtensions
{
    public static void ConfigureLogging(this ILoggingBuilder builder)
    {
        builder.AddJsonConsole();
        builder.AddSerilog();
    }
}