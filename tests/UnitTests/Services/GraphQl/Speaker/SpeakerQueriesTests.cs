namespace UnitTests.Services.GraphQl.Speaker;

using System.Text.Json;

using AutoFixture;

using ConferencePlanner.Models;
using ConferencePlanner.Service.Speaker;

using FluentAssertions;

using Xunit.Abstractions;
using Xunit.Categories;

[UnitTest(nameof(SpeakerQueries))]
public class SpeakerQueriesTests : QueryTestsBase
{
    private readonly ITestOutputHelper outputHelper;
    private readonly Fixture fixture;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeakerQueriesTests"/> class.
    /// </summary>
    public SpeakerQueriesTests(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper;
        this.fixture = new Fixture();
    }

    [Fact]
    public async Task GetSpeakersShouldReturnCorrectData()
    {
        var speaker = this.fixture.Create<Speaker>();

        this.ApplicationDbContext.Speakers.Add(speaker);
        await this.ApplicationDbContext.SaveChangesAsync().ConfigureAwait(false);

        const string query = @"query speakers{speakers{id bio website name}}";

        string result = await this.ExecuteQueryAsync(query).ConfigureAwait(false);

        var speakers = System.Text.Json.Nodes.JsonNode.Parse(result)!["data"]!["speakers"]!.AsArray();

        this.outputHelper.WriteLine($"Expected {JsonSerializer.Serialize(speaker)}");

        var actual = speakers.First().Deserialize<Speaker>(QueryTestsBase.JsonSerializerOptions);

        this.outputHelper.WriteLine($"Actual {JsonSerializer.Serialize(actual)}");

        actual.Should().BeEquivalentTo(speaker,
            options =>
            {
                options.Including(s => s.Id);
                options.Including(s => s.Bio);
                options.Including(s => s.Website);
                options.Including(s => s.Name);

                return options;
            });
    }
}