namespace UnitTests.Services.ProgramConfiguration;

using ConferencePlanner.Service.ProgramConfiguration;

using FluentAssertions;

using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit.Abstractions;
using Xunit.Categories;

[UnitTest(nameof(ServiceCollectionExtensionsGraphQl))]
public class ServiceCollectionExtensionsGraphQlTests
{
    private readonly ISchema schema;

    // ReSharper disable once NotAccessedField.Local
    private readonly ITestOutputHelper testOutputHelper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceCollectionExtensionsGraphQlTests" /> class.
    /// </summary>
    public ServiceCollectionExtensionsGraphQlTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        this.schema = MakeSchema().GetAwaiter().GetResult();
    }

    [Fact]
    public void AddQueriesShouldRegisterQueryType()
    {
        this.schema.QueryType.Name.Should().Be(OperationTypeNames.Query);
    }

    [Fact]
    public void AddQueriesShouldRegisterSpeakers()
    {
        this.schema.QueryType.Fields.ContainsField("speakers").Should().BeTrue();

        IEnumerable<NameString> typeNames = this.schema.Types.Select(t => t.Name).ToList();
        typeNames.Should().Contain("Speaker");
    }

    private static async Task<ISchema> MakeSchema()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddTransient(_ => Mock.Of<ILoggerFactory>());
        services.AddGraphQLServer();

        var builder = Mock.Of<IRequestExecutorBuilder>(executorBuilder =>
            executorBuilder.Services == services);

        builder.AddQueries();

        return await builder.BuildSchemaAsync().ConfigureAwait(false);
    }
}