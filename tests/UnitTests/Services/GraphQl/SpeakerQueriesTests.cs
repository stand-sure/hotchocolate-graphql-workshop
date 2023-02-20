namespace UnitTests.Services.GraphQl;

using System.Text.Json;

using AutoFixture;

using ConferencePlanner.Models;

using FluentAssertions;

using Xunit.Abstractions;

public class SpeakerQueriesTests : QueryTestsBase
{
    private readonly ITestOutputHelper outputHelper;
    private readonly Fixture fixture;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

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
        var speaker = new Speaker
        {
            Id = this.fixture.Create<int>(),
            Bio = this.fixture.Create<string>(),
            Name = this.fixture.Create<string>(),
            Website = this.fixture.Create<string>(),
        };

        this.ApplicationDbContext.Speakers.Add(speaker);
        await this.ApplicationDbContext.SaveChangesAsync().ConfigureAwait(false);

        const string query = @"query speakers{speakers{id bio website name}}";

        string result = await this.ExecuteQueryAsync(query).ConfigureAwait(false);

        var speakers = System.Text.Json.Nodes.JsonNode.Parse(result)!["data"]!["speakers"]!.AsArray();

        this.outputHelper.WriteLine($"Expected {JsonSerializer.Serialize(speaker)}");

        var actual = speakers.First().Deserialize<Speaker>(SpeakerQueriesTests.JsonSerializerOptions);

        this.outputHelper.WriteLine($"Actual {JsonSerializer.Serialize(actual)}");

        actual.Should().BeEquivalentTo(speaker);
    }
}