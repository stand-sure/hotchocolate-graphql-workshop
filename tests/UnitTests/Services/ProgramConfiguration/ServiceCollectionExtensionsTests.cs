namespace UnitTests.Services.ProgramConfiguration;

using ConferencePlanner.Service.ProgramConfiguration;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

using Xunit.Categories;

[UnitTest(nameof(ServiceCollectionExtensions))]
public class ServiceCollectionExtensionsTests
{
#pragma warning disable EF1001
    private readonly NpgsqlOptionsExtension npgsqlOptionsExtension;
#pragma warning restore EF1001

    private const string ConnectionString = "Host=localhost:26257;Database=conference;Username=user;Password=pass;";
    private const string ConnectionStringKey = "ConnectionStrings:ConferencePlanner";

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceCollectionExtensionsTests"/> class.
    /// </summary>
    public ServiceCollectionExtensionsTests()
    {
        IConfiguration configuration = MakeConfiguration();

        var options = new DbContextOptionsBuilder()
            .ConfigureDbContextFactory(configuration)
            .Options;

#pragma warning disable EF1001
        this.npgsqlOptionsExtension = options.FindExtension<NpgsqlOptionsExtension>()!;
#pragma warning restore EF1001
    }

    [Fact]
    public void ConfigureDbContextFactoryShouldConfigureDatabase()
    {
        this.npgsqlOptionsExtension.ConnectionString.Should().Be(ServiceCollectionExtensionsTests.ConnectionString);
    }

    [Fact]
    public void ConfigureDbContextFactoryShouldConfigureMigrationsAssembly()
    {
        this.npgsqlOptionsExtension.MigrationsAssembly.Should().Be("ConferencePlannerService");
    }

    private static IConfiguration MakeConfiguration()
    {
        var configValues = new Dictionary<string, string>
        {
            { ServiceCollectionExtensionsTests.ConnectionStringKey, ServiceCollectionExtensionsTests.ConnectionString },
        };

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(configValues!);
        IConfiguration configuration = configurationBuilder.Build();
        return configuration;
    }
}