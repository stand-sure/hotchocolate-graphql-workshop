namespace ConferencePlanner.Service;

using ConferencePlanner.Service.Speaker;

using HotChocolate.Diagnostics;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Pagination;

public static class GraphQlStartupExtensions
{
    private static ILogger logger = null!;

    public static IServiceCollection ConfigureGraphServices(this IServiceCollection serviceCollection, IWebHostEnvironment? environment)
    {
        MakeLogger(serviceCollection);

        IRequestExecutorBuilder builder = serviceCollection.AddGraphQLServer()
            .ModifyOptions(options =>
            {
                options.SortFieldsByName = true;
                options.EnableFlagEnums = true;
            })
            .AddQueries()
            // .AddGlobalObjectIdentification()
            // .AddFiltering()
            // .AddSorting()
            .ModifyRequestOptions((_, options) =>
            {
                options.IncludeExceptionDetails = true;
                options.Complexity.Enable = true;
                options.Complexity.ApplyDefaults = true;

                options.Complexity.Calculation = context =>
                {
                    int cost = context.Complexity + context.ChildComplexity;
                    var message = $"Cost: {context.Selection.Name} {cost}";

                    GraphQlStartupExtensions.logger.LogInformation("{message}", message);

                    return cost;
                };
            })
            .SetPagingOptions(new PagingOptions
            {
                MaxPageSize = 1000,
            })
            .AddDiagnosticEventListener(provider =>
                new QueryLoggerDiagnosticEventListener(
                    ActivatorUtilities.GetServiceOrCreateInstance<ILogger<QueryLoggerDiagnosticEventListener>>(provider)))
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

    private static void ConfigureInstrumentationOptions(InstrumentationOptions options)
    {
        options.Scopes = ActivityScopes.All;
        options.IncludeDataLoaderKeys = true;
        options.IncludeDocument = true;
        options.RenameRootActivity = true;
        options.RequestDetails = RequestDetails.All;
    }

    private static void MakeLogger(IServiceCollection serviceCollection)
    {
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        var factory = ActivatorUtilities.GetServiceOrCreateInstance<ILoggerFactory>(serviceProvider);
        GraphQlStartupExtensions.logger = factory.CreateLogger("GraphQL");
    }

    private static IRequestExecutorBuilder AddErrorFilters(this IRequestExecutorBuilder builder)
    {
        return builder.AddErrorFilter<ExceptionMessageErrorFilter>();
    }
}