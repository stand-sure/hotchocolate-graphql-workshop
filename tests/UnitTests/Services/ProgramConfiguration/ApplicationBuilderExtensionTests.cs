namespace UnitTests.Services.ProgramConfiguration;

using ConferencePlanner.Service.ProgramConfiguration;

using FluentAssertions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;
using Xunit.Categories;

using ApplicationBuilderExtensions = ConferencePlanner.Service.ProgramConfiguration.ApplicationBuilderExtensions;

[UnitTest(nameof(ApplicationBuilderExtensions))]
public class ApplicationBuilderExtensionTests
{
    private readonly ITestOutputHelper testOutputHelper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationBuilderExtensionTests" /> class.
    /// </summary>
    public ApplicationBuilderExtensionTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ConfigureApplicationShouldSetUpGraphQlEndpoint()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.ConfigureBuilder();

        WebApplication app = builder.Build();
        app.ConfigureApplication();

        var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
        endpointDataSource.Should().NotBeNull();

        List<string?> names = endpointDataSource.Endpoints.Select(endpoint => endpoint.DisplayName).ToList();
        this.testOutputHelper.WriteLine(string.Join(null, names));
        names.Should().BeEquivalentTo("Hot Chocolate GraphQL Pipeline");
    }
}