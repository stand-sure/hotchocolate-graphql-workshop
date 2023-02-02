namespace UnitTests.Models;

using System.ComponentModel.DataAnnotations;

using ConferencePlanner.Models;

using FluentAssertions;

using Xunit.Categories;

[UnitTest(nameof(Speaker))]
public class SpeakerTests
{
    /// <summary>
    /// Gets or sets the Target.
    /// </summary>
    private Speaker Target { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeakerTests"/> class.
    /// </summary>
    public SpeakerTests()
    {
        this.Target = new Speaker();
    }

    [Fact]
    public void IdShouldBeWellBehaved()
    {
        PropertyChecker.CheckProperty<int>(() => this.Target.Id);
        PropertyChecker.CheckInvariance(() => this.Target.Id);
    }

    [Fact]
    public void NameShouldBeWellBehaved()
    {
        PropertyChecker.CheckProperty<string?>(() => this.Target.Name);
        PropertyChecker.CheckInvariance(() => this.Target.Name);
    }

    [Fact]
    public void NameShouldBeRequired()
    {
        PropertyChecker.CheckAttribute<RequiredAttribute>(() => this.Target.Name);
    }

    [Fact]
    public void NameShouldHaveMaxLength200()
    {
        var a = PropertyChecker.CheckAttribute<StringLengthAttribute>(() => this.Target.Name);
        a.MaximumLength.Should().Be(200);
    }

    [Fact]
    public void BioShouldBeWellBehaved()
    {
        PropertyChecker.CheckProperty<string>(() => this.Target.Bio);
        PropertyChecker.CheckInvariance(() => this.Target.Bio);
    }

    [Fact]
    public void BioShouldHaveMaxLength4000()
    {
        var a = PropertyChecker.CheckAttribute<StringLengthAttribute>(() => this.Target.Bio);
        a.MaximumLength.Should().Be(4000);
    }

    [Fact]
    public void BWebsiteShouldBeWellBehaved()
    {
        PropertyChecker.CheckProperty<string>(() => this.Target.Website);
        PropertyChecker.CheckInvariance(() => this.Target.Website);
    }

    [Fact]
    public void WebsiteShouldHaveMaxLength1000()
    {
        var a = PropertyChecker.CheckAttribute<StringLengthAttribute>(() => this.Target.Website);
        a.MaximumLength.Should().Be(1000);
    }
}