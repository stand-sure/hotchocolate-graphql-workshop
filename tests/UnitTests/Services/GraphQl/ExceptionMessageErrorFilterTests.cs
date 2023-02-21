namespace UnitTests.Services.GraphQl;

using AutoFixture;

using ConferencePlanner.Service.GraphQl;

using FluentAssertions;

using HotChocolate;

using Moq;

using Xunit.Categories;

[UnitTest(nameof(ExceptionMessageErrorFilter))]
public class ExceptionMessageErrorFilterTests
{
    private ExceptionMessageErrorFilter Target { get; } = new();

    private Fixture Fixture { get; } = new();

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OnErrorShouldAddExceptionMessage(bool hasException)
    {
        Exception? ex = hasException ? new Exception(this.Fixture.Create<string>()) : null;

        var error = Mock.Of<IError>(e =>
            e.Message == this.Fixture.Create<string>() &&
            e.Exception == ex);

        string? actualMessage = null;
        var expectedMessage = $"{error.Message}{(ex == null ? null : $" - {ex.Message}")}";

        Mock.Get(error)
            .Setup(e => e.WithMessage(It.IsAny<string>()))
            .Callback<string>((message) =>
            {
                actualMessage = message;
            });

        this.Target.OnError(error);

        actualMessage.Should().Be(expectedMessage);
    }
}