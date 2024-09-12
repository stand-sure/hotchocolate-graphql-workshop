using ConferencePlanner.Service.ProgramConfiguration;

using Serilog;
using Serilog.Sinks.Grafana.Loki;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console(new LokiJsonTextFormatter())
    .CreateBootstrapLogger();

Log.Information("Starting at {Time}", DateTimeOffset.UtcNow);

WebApplication app = builder.Build();
app.ConfigureApplication();

app.Run();