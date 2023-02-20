namespace UnitTests.Services.ProgramConfiguration;

using System.Diagnostics;

using ConferencePlanner.Service.ProgramConfiguration;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Xunit.Abstractions;
using Xunit.Categories;

[UnitTest(nameof(ServiceCollectionExtensionsInstrumentation))]
public class ServiceCollectionExtensionsInstrumentationTests
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly IServiceCollection serviceCollection;
    private readonly IWebHostEnvironment environment;
    private readonly IConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceCollectionExtensionsInstrumentationTests"/> class.
    /// </summary>
    public ServiceCollectionExtensionsInstrumentationTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        this.serviceCollection = new ServiceCollection();
        this.environment = Mock.Of<IWebHostEnvironment>(hostEnvironment => hostEnvironment.ApplicationName == "myApp");

        var configValues = new Dictionary<string, string>
        {
            { "JaegerExporter:EndpointUri", "http://localhost:14268/api/traces" },
        };

        this.configuration = new ConfigurationBuilder().AddInMemoryCollection(configValues!).Build();
    }

    [Fact]
    public void AddInstrumentationShouldAddOpenTelemetry()
    {
        this.serviceCollection.AddInstrumentation(this.environment, this.configuration);

        var services = this.serviceCollection.BuildServiceProvider().GetServices<IHostedService>().ToList();

        this.testOutputHelper.WriteLine($"{string.Join(null, services.Select(s => s.GetType().Name))}");

        services.Should().Contain(service => service.GetType().Name == "TelemetryHostedService");
    }

    [Fact]
    public void AddInstrumentationShouldAddHttpClientInstrumentation()
    {
        this.serviceCollection.AddInstrumentation(this.environment, this.configuration);

        var tracerProvider = this.serviceCollection.BuildServiceProvider().GetService<TracerProvider>();

        var instrumentationList = tracerProvider.GetType()
            .GetProperty("Instrumentations", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .GetValue((tracerProvider)) as List<object?>;

        instrumentationList.Should().Contain(o => o.GetType().Name == "HttpClientInstrumentation");
    }

    [Fact]
    public void AddInstrumentationShouldAddAspNetCoreInstrumentation()
    {
        this.serviceCollection.AddInstrumentation(this.environment, this.configuration);

        var tracerProvider = this.serviceCollection.BuildServiceProvider().GetService<TracerProvider>();

        var instrumentationList = tracerProvider.GetType()
            .GetProperty("Instrumentations", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .GetValue((tracerProvider)) as List<object?>;

        instrumentationList.Should().Contain(o => o.GetType().Name == "AspNetCoreInstrumentation");
    }

    [Fact]
    public void AddInstrumentationShouldAddEntityFrameworkInstrumentation()
    {
        this.serviceCollection.AddInstrumentation(this.environment, this.configuration);

        var tracerProvider = this.serviceCollection.BuildServiceProvider().GetService<TracerProvider>();

        var instrumentationList = tracerProvider.GetType()
            .GetProperty("Instrumentations", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            .GetValue((tracerProvider)) as List<object?>;

        instrumentationList.Should().Contain(o => o.GetType().Name == "EntityFrameworkInstrumentation");
    }

    [Fact]
    public void ConfigureTraceProviderShouldSetServiceName()
    {
        const string serviceName = "foo";
        const string version = "v1.2.3.4";
        ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName, version);

        var builderMock = new Mock<TracerProviderBuilder> { CallBase = true };

        List<string> sources = new List<string>();

        builderMock.Setup(builder => builder.AddSource(It.IsAny<string[]>()))
            .Callback<string[]>((names) => { sources.AddRange(names); })
            .Returns<string[]>((names) => builderMock.Object);

        var tracerProviderBuilder = builderMock.Object;
        tracerProviderBuilder.ConfigureTraceProvider(resourceBuilder, this.configuration, this.environment, serviceName);

        sources.Should().Contain(serviceName);
    }
}