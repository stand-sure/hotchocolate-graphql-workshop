namespace UnitTests.Services.GraphQl;

using System.Text.RegularExpressions;

using ConferencePlanner.Data;
using ConferencePlanner.Service;
using ConferencePlanner.Service.ProgramConfiguration;

using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Language;
using HotChocolate.Resolvers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

public abstract partial class QueryTestsBase : IDisposable
{
    private const string DatabaseName = "InMemoryDb";

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueryTestsBase" /> class.
    /// </summary>
    protected QueryTestsBase()
    {
        var environment = Mock.Of<IWebHostEnvironment>(env => env.EnvironmentName == Environments.Development);

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseInMemoryDatabase(QueryTestsBase.DatabaseName);
        builder.UseInternalServiceProvider(this.ServiceProvider);

        this.ServiceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .AddScoped(_ => Mock.Of<ILogger<QueryLoggerDiagnosticEventListener>>())
            .AddDbContextFactory<ApplicationDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseInternalServiceProvider(this.ServiceProvider);
                optionsBuilder.UseInMemoryDatabase(QueryTestsBase.DatabaseName);

                this.DbContextOptions = (DbContextOptions<ApplicationDbContext>)optionsBuilder.Options;
            })
            .AddScoped<ApplicationDbContext>(_ => this.ApplicationDbContext!)
            .ConfigureGraphServices(environment)
            .AddSingleton(provider => new RequestExecutorProxy(
                provider.GetRequiredService<IRequestExecutorResolver>(),
                Schema.DefaultName))
            .BuildServiceProvider();

        this.ApplicationDbContext = this.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext();

        this.ExecutorProxy = this.ServiceProvider.GetRequiredService<RequestExecutorProxy>();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected ApplicationDbContext ApplicationDbContext { get; }

    protected DbContextOptions<ApplicationDbContext> DbContextOptions { get; private set; }

    private RequestExecutorProxy ExecutorProxy { get; }

    private IServiceProvider? ServiceProvider { get; }

    protected static string CollapseWhitespace(string text)
    {
        return WhitespaceRegex().Replace(text, " ");
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        this.ExecutorProxy.Dispose();
        this.ApplicationDbContext.Dispose();
    }

    protected async Task<string> ExecuteQueryAsync(string query, IDictionary<string, object?>? variableValues = default)
    {
        string result = await this.ExecuteRequestAsync(builder =>
        {
            builder.SetQuery(query);

            if (variableValues != null)
            {
                builder.SetVariableValues(variableValues);
            }
        });

        return result;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    protected async Task<string> ExecuteRequestAsync(
        Action<IQueryRequestBuilder> configureRequest,
        CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = this.ServiceProvider!.CreateAsyncScope();

        var requestBuilder = new QueryRequestBuilder();
        requestBuilder.SetServices(scope.ServiceProvider);

        configureRequest(requestBuilder);

        IReadOnlyQueryRequest request = requestBuilder.Create();

        using IExecutionResult result = await this.ExecutorProxy.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);

        result.ExpectQueryResult();

        string json = await result.ToJsonAsync();

        return json;
    }

    // ReSharper disable once UnusedMember.Global
    protected static IResolverContext MakeResolverContext()
    {
        var resolverContext = Mock.Of<IResolverContext>();
        Mock.Get(resolverContext).Setup(context => context.ArgumentLiteral<IValueNode>(It.IsAny<NameString>())).Returns(Mock.Of<IValueNode>());

        return resolverContext;
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex WhitespaceRegex();
}