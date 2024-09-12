namespace UnitTests.Services.GraphQl.Speaker;

using AutoFixture;

using ConferencePlanner.Service.Speaker;

using Xunit.Categories;

[UnitTest(nameof(AddSpeakerInput))]
public class AddSpeakerInputTests
{
    private AddSpeakerInput Target { get; }
    private Fixture Fixture { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AddSpeakerInputTests"/> class.
    /// </summary>
    public AddSpeakerInputTests()
    {
        this.Target = this.Fixture.Create<AddSpeakerInput>();
    }

    [Fact]
    public void NameShouldBeInitOnlyStringProp()
    {
        PropertyChecker.CheckProperty<string>(() => this.Target.Name, expectedDefaultValue: this.Target.Name);

        PropertyChecker.CheckInitOnly(() => this.Target.Name);
    }

    [Fact]
    public void BioShouldBeInitOnlyStringProp()
    {
        PropertyChecker.CheckProperty<string>(() => this.Target.Bio, expectedDefaultValue: this.Target.Bio);

        PropertyChecker.CheckInitOnly(() => this.Target.Bio);
    }

    [Fact]
    public void WebsiteShouldBeInitOnlyStringProp()
    {
        PropertyChecker.CheckProperty<string>(() => this.Target.Website, expectedDefaultValue: this.Target.Website);

        PropertyChecker.CheckInitOnly(() => this.Target.Website);
    }
}