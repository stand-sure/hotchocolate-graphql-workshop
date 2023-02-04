namespace UnitTests.Services.GraphQl;

using ConferencePlanner.Data;
using ConferencePlanner.Service.GraphQl;

using FluentAssertions;

using HotChocolate.Resolvers;
using HotChocolate.Types;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Xunit.Abstractions;
using Xunit.Categories;

using ObjectFieldDescriptorExtensions = ConferencePlanner.Service.GraphQl.ObjectFieldDescriptorExtensions;

[UnitTest(nameof(ObjectFieldDescriptorExtensions))]
public class ObjectFieldDescriptorExtensionsTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public ObjectFieldDescriptorExtensionsTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void UseDbContextShouldCallUseScopedService()
    {
        var descriptor = Mock.Of<IObjectFieldDescriptor>();

        var middlewareTargetArgList = new List<Type[]>();

        Mock.Get(descriptor)
            .Setup(fieldDescriptor => fieldDescriptor.Use(It.IsAny<FieldMiddleware>()))
            .Callback<FieldMiddleware>(middleware =>
            {
                if (middleware.Target is not null)
                {
                    middlewareTargetArgList.Add(middleware.Target.GetType().GetGenericArguments());
                }

                this.testOutputHelper.WriteLine($"{middleware.Target?.GetType()}");
            });

        descriptor.UseDbContext<ApplicationDbContext>();

        var typeArgs = middlewareTargetArgList.SelectMany(types => types.Select(type => type));

        typeArgs.Should().Contain(typeof(ApplicationDbContext));
    }

    [Fact]
    public void DisposeDbContextAsyncShouldReturnCompletedTask()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("conference");

        DbContextOptions<ApplicationDbContext> options = optionsBuilder.Options;

        var context = new ApplicationDbContext(options);

        IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();

        ValueTask result = ObjectFieldDescriptorExtensions.DisposeDbContextAsync(serviceProvider, context);

        result.Should().Be(ValueTask.CompletedTask);
    }
}