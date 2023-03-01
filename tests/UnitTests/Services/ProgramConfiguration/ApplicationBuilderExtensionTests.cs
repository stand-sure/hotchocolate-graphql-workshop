namespace UnitTests.Services.ProgramConfiguration;

using System.Text.Json;

using ConferencePlanner.Service.ProgramConfiguration;

using FluentAssertions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        const string expectedEndpointDisplayName = "Hot Chocolate GraphQL Pipeline";

        this.VerifyEndpoint(expectedEndpointDisplayName);
    }

    [Fact]
    public void ConfigureApplicationShouldSetUpMetricsEndpoint()
    {
        const string expectedEndpointDisplayName = "Prometheus metrics";

        this.VerifyEndpoint(expectedEndpointDisplayName);
    }

    [Fact]
    public void ConfigureApplicationShouldSetUpReadyHealthEndpoint()
    {
        const string expectedEndpointDisplayName = "Health checks";

        IEnumerable<Endpoint> endpoints = this.VerifyEndpoint(expectedEndpointDisplayName);

        endpoints.Cast<RouteEndpoint>().Should().Contain(endpoint => endpoint.RoutePattern.RawText == "/healthz/ready");
    }

    [Fact]
    public void ConfigureApplicationShouldSetUpLiveHealthEndpoint()
    {
        const string expectedEndpointDisplayName = "Health checks";

        IEnumerable<Endpoint> endpoints = this.VerifyEndpoint(expectedEndpointDisplayName);

        endpoints.Cast<RouteEndpoint>().Should().Contain(endpoint => endpoint.RoutePattern.RawText == "/healthz/live");
    }

    private IEnumerable<Endpoint> VerifyEndpoint(string expectedEndpointDisplayName)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.ConfigureBuilder();

        WebApplication app = builder.Build();
        app.ConfigureApplication();

        var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
        endpointDataSource.Should().NotBeNull();

        this.testOutputHelper.WriteLine(string.Join(null, endpointDataSource.Endpoints));
        IEnumerable<Endpoint> endpoints = endpointDataSource.Endpoints.Where(endpoint => endpoint.DisplayName == expectedEndpointDisplayName).ToList();

        endpoints.Should().NotBeNullOrEmpty();

        return endpoints;
    }
}