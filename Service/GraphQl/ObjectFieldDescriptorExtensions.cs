namespace ConferencePlanner.Service.GraphQl;

using Microsoft.EntityFrameworkCore;

public static class ObjectFieldDescriptorExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IObjectFieldDescriptor UseDbContext<TDbContext>(
        this IObjectFieldDescriptor descriptor)
        where TDbContext : DbContext
    {
        return descriptor.UseScopedService(
            provider => provider.GetRequiredService<IDbContextFactory<TDbContext>>().CreateDbContext(),
            disposeAsync: DisposeDbContextAsync);
    }

    private static ValueTask DisposeDbContextAsync<TDbContext>(IServiceProvider serviceProvider, TDbContext dbContext) where TDbContext : DbContext
    {
        return ValueTask.CompletedTask;
    }
}