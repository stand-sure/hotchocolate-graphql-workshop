namespace ConferencePlanner.Service.ProgramConfiguration;

using ConferencePlanner.Service.GraphQl;
using ConferencePlanner.Service.Speaker;

using HotChocolate.Diagnostics;
using HotChocolate.Execution.Configuration;
using HotChocolate.Execution.Options;
using HotChocolate.Execution.Pipeline.Complexity;
using HotChocolate.Types.Pagination;

internal static class ServiceCollectionExtensionsGraphQl
{
    private static ILogger logger = null!;

    public static IServiceCollection AddGraphQl(this IServiceCollection serviceCollection)
    {
        MakeLogger(serviceCollection);

        IRequestExecutorBuilder builder = serviceCollection.AddGraphQLServer()
            .ModifyOptions(ConfigureSchemaOptions)
            .AddQueries()
            // .AddGlobalObjectIdentification()
            // .AddFiltering()
            // .AddSorting()
            .ModifyRequestOptions(ConfigureRequestOptions)
            .SetPagingOptions(ConfigurePagingOptions())
            .AddDiagnosticEventListener(ConfigureDiagnosticEventListener)
            .AddErrorFilters();

        builder.AddInstrumentation(ConfigureInstrumentationOptions);

        return serviceCollection;
    }

    internal static IRequestExecutorBuilder AddQueries(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddQueryType(descriptor => descriptor.Name(OperationTypeNames.Query))
            .AddTypeExtension<SpeakerQueries>();
    }

    private static IRequestExecutorBuilder AddErrorFilters(this IRequestExecutorBuilder builder)
    {
        return builder.AddErrorFilter<ExceptionMessageErrorFilter>();
    }

    private static int CalculateComplexity(ComplexityContext context)
    {
        int cost = context.Complexity + context.ChildComplexity;
        var message = $"Cost: {context.Selection.Name} {cost}";

        ServiceCollectionExtensionsGraphQl.logger.LogInformation("{message}", message);

        return cost;
    }

    private static QueryLoggerDiagnosticEventListener ConfigureDiagnosticEventListener(IServiceProvider provider)
    {
        return new QueryLoggerDiagnosticEventListener(
            ActivatorUtilities.GetServiceOrCreateInstance<ILogger<QueryLoggerDiagnosticEventListener>>(provider));
    }

    private static void ConfigureInstrumentationOptions(InstrumentationOptions options)
    {
        options.Scopes = ActivityScopes.All;
        options.IncludeDataLoaderKeys = true;
        options.IncludeDocument = true;
        options.RenameRootActivity = true;
        options.RequestDetails = RequestDetails.All;
    }

    private static PagingOptions ConfigurePagingOptions()
    {
        return new PagingOptions
        {
            MaxPageSize = 1000,
        };
    }

    // ReSharper disable once UnusedParameter.Local
#pragma warning disable S1172 //signature mattes
    private static void ConfigureRequestOptions(IServiceProvider serviceProvider, RequestExecutorOptions options)
#pragma warning restore S1172
    {
        options.IncludeExceptionDetails = true;
        options.Complexity.Enable = true;
        options.Complexity.ApplyDefaults = true;

        options.Complexity.Calculation = CalculateComplexity;
    }

    private static void ConfigureSchemaOptions(ISchemaOptions options)
    {
        options.SortFieldsByName = true;
        options.EnableFlagEnums = true;
    }

    private static void MakeLogger(IServiceCollection serviceCollection)
    {
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        var factory = ActivatorUtilities.GetServiceOrCreateInstance<ILoggerFactory>(serviceProvider);
        ServiceCollectionExtensionsGraphQl.logger = factory.CreateLogger("GraphQL");
    }
}