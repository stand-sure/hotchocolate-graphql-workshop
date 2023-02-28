using ConferencePlanner.Service.ProgramConfiguration;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting at {Time}", DateTimeOffset.UtcNow);

WebApplication app = builder.Build();
app.ConfigureApplication();

app.Run();