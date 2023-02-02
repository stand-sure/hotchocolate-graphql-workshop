
using ConferencePlanner.Data;

using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddDbContext<ApplicationDbContext>((provider, optionsBuilder) =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("ConferencePlanner"), contextOptionsBuilder => contextOptionsBuilder.MigrationsAssembly("Service"));
});

WebApplication app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
