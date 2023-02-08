namespace ConferencePlanner.Service.ProgramConfiguration;

#nullable enable
using System.Reflection;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

internal static class ServiceCollectionExtensionsInstrumentation
{
    public static void AddInstrumentation(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
    {
        string serviceName = environment.ApplicationName;
        var version = typeof(Program).GetTypeInfo().Assembly.GetName().Version?.ToString();
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName, version);

        services.AddOpenTelemetry().WithTracing(providerBuilder =>
        {
            providerBuilder.AddSource(serviceName).SetResourceBuilder(resourceBuilder);
            providerBuilder.AddHttpClientInstrumentation();
            providerBuilder.AddAspNetCoreInstrumentation();
            providerBuilder.AddHotChocolateInstrumentation();

            providerBuilder.AddEntityFrameworkCoreInstrumentation(options => { options.SetDbStatementForText = true; });

            providerBuilder.AddJaegerExporter(options =>
            {
                string? endpointUriAddress = configuration["JaegerExporter:EndpointUri"];

                bool goodUri = Uri.TryCreate(endpointUriAddress, UriKind.Absolute, out Uri? endPointUri);

                if (goodUri is false)
                {
                    return;
                }

                options.Endpoint = endPointUri;
                options.Protocol = JaegerExportProtocol.HttpBinaryThrift;
            });

            if (environment.IsDevelopment())
            {
                providerBuilder.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console);
            }
        }).StartWithHost();
    }
}