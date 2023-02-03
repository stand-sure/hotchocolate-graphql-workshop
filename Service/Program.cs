using ConferencePlanner.Service.ProgramConfiguration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.ConfigureBuilder();

WebApplication app = builder.Build();
app.ConfigureApplication();

app.Run();