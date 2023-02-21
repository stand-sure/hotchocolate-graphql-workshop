namespace UnitTests.Services.GraphQl.Speaker;

using AutoFixture;

using ConferencePlanner.Models;
using ConferencePlanner.Service.Speaker;

using FluentAssertions;

using Xunit.Categories;

[UnitTest(nameof(AddSpeakerPayload))]
public class AddSpeakerPayloadTests
{
    private readonly Speaker speaker;
    private AddSpeakerPayload Target { get; }
    private Fixture Fixture { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AddSpeakerPayloadTests"/> class.
    /// </summary>
    public AddSpeakerPayloadTests()
    {
        this.speaker = this.Fixture.Create<Speaker>();
        this.Target = new AddSpeakerPayload(this.speaker);
    }

    [Fact]
    public void SpeakerShouldBeReadOnlyProp()
    {
        PropertyChecker.CheckProperty<Speaker>(() => this.Target.Speaker,
            readOnly: true,
            expectedDefaultValue: this.speaker);
    }

    [Fact]
    public void SpeakerShouldReturnValueSet()
    {
        this.Target.Speaker.Should().Be(this.speaker);
    }
}