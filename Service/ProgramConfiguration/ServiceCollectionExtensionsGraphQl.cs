namespace ConferencePlanner.Service.ProgramConfiguration;

using ConferencePlanner.Service.GraphQl;
using ConferencePlanner.Service.Speaker;

using HotChocolate.Diagnostics;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Pagination;

internal static class ServiceCollectionExtensionsGraphQl
{
    private static ILogger logger = null!;

    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddGraphQl(this IServiceCollection serviceCollection)
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

                    ServiceCollectionExtensionsGraphQl.logger.LogInformation("{message}", message);

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

    private static IRequestExecutorBuilder AddQueries(this IRequestExecutorBuilder builder)
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
        ServiceCollectionExtensionsGraphQl.logger = factory.CreateLogger("GraphQL");
    }

    private static IRequestExecutorBuilder AddErrorFilters(this IRequestExecutorBuilder builder)
    {
        return builder.AddErrorFilter<ExceptionMessageErrorFilter>();
    }
}