using ConferencePlanner.Data;
using ConferencePlanner.Service;

using Microsoft.EntityFrameworkCore;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole();
builder.Logging.AddSerilog();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddDbContext<ApplicationDbContext>((_, optionsBuilder) =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("ConferencePlanner"),
        contextOptionsBuilder => contextOptionsBuilder.MigrationsAssembly("Service"));
});

builder.Services.ConfigureGraphServices(builder.Environment);
builder.Services.AddInstrumentation(builder.Environment, builder.Configuration);

WebApplication app = builder.Build();

app.UseRouting();
#pragma warning disable ASP0014
app.UseEndpoints(routeBuilder => { routeBuilder.MapGraphQL(); });
#pragma warning restore ASP0014

app.Run();