namespace UnitTests.Services.ProgramConfiguration;

using ConferencePlanner.Service.ProgramConfiguration;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

using Moq;

using Xunit.Abstractions;

public class ConfigurationBuilderExtensionsTests
{
    private readonly ITestOutputHelper testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationBuilderExtensionsTests"/> class.
    /// </summary>
    public ConfigurationBuilderExtensionsTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ConfigureConfigurationShouldAddAppSettings()
    {
        var sources = new List<IConfigurationSource>();

        IConfigurationBuilder builder = Mock.Of<IConfigurationBuilder>(MockBehavior.Strict);

        Mock.Get(builder)
            .Setup(b =>
                b.Add(It.IsAny<IConfigurationSource>()))
            .Callback<IConfigurationSource>((source) => { sources.Add(source); })
            .Returns<IConfigurationSource>((_) => builder);

        builder.ConfigureConfiguration();

        this.testOutputHelper.WriteLine($"{sources}");

        var jsonSources = sources.Where(source => source is JsonConfigurationSource).Cast<JsonConfigurationSource>();

        jsonSources.Should().Contain(source => source.Path == "appsettings.json");
    }
}