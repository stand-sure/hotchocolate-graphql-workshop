namespace UnitTests.Services.GraphQl.Speaker;

using System.Text.Json;

using AutoFixture;

using ConferencePlanner.Data;
using ConferencePlanner.Models;
using ConferencePlanner.Service.GraphQl;
using ConferencePlanner.Service.Speaker;

using FluentAssertions;

using HotChocolate;

using Xunit.Abstractions;
using Xunit.Categories;

[UnitTest(nameof(SpeakerMutations))]
public class SpeakerMutationsTests : QueryTestsBase
{
    private ITestOutputHelper TestOutputHelper { get; }
    private SpeakerMutations Target { get; } = new();
    private Fixture Fixture { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeakerMutationsTests"/> class.
    /// </summary>
    public SpeakerMutationsTests(ITestOutputHelper testOutputHelper)
    {
        this.TestOutputHelper = testOutputHelper;
    }

    [Fact]
    public void AddSpeakerAsyncShouldReturnTaskAddSpeakerPayload()
    {
        MethodChecker.VerifyMethod<Task<AddSpeakerPayload>>(() => this.Target.AddSpeakerAsync(default!, default!));
    }

    [Fact]
    public void AddSpeakerAsyncShouldUseUseApplicationDbContextAttribute()
    {
        MethodChecker.VerifyMethodAttribute<UseApplicationDbContextAttribute>(() => this.Target.AddSpeakerAsync(default!, default!));
    }

    [Fact]
    public void AddSpeakerAsyncShouldUseScopedDbContext()
    {
        MethodChecker.VerifyParameterAttribute<ScopedServiceAttribute>(
            () => this.Target.AddSpeakerAsync(default!, default!),
            typeof(ApplicationDbContext));
    }

    [Fact]
    public async Task AddSpeakerAsyncShouldSaveSpeaker()
    {
        var expected = this.Fixture.Create<Speaker>();
        var input = new AddSpeakerInput(expected.Name!, expected.Bio!, expected.Website!);

        var result = await this.Target.AddSpeakerAsync(
            input,
            this.ApplicationDbContext).ConfigureAwait(false);

        int? id = result.Speaker.Id;

        var actual = await this.ApplicationDbContext.Speakers.FindAsync(id).ConfigureAwait(false);

        actual.Should().BeEquivalentTo(expected, options => options.Excluding(speaker => speaker.Id));
    }

    [Fact]
    public async Task AddSpeakerAsyncShouldReturnCorrectResult()
    {
        var expectedSpeaker = this.Fixture.Create<Speaker>();
        this.TestOutputHelper.WriteLine($"Expected {JsonSerializer.Serialize(expectedSpeaker)}");

        var query =
            $@"mutation addSpeaker{{addSpeaker(input:{{bio:""{expectedSpeaker.Bio}"",name:""{expectedSpeaker.Name}"",website:""{expectedSpeaker.Website}""}}){{speaker{{bio id name website}}}}}}";

        string result = await this.ExecuteQueryAsync(query).ConfigureAwait(false);

        this.TestOutputHelper.WriteLine($"result {result}");

        var speaker = System.Text.Json.Nodes.JsonNode.Parse(result)!["data"]!["addSpeaker"]!["speaker"];
        var actual = speaker.Deserialize<Speaker>(QueryTestsBase.JsonSerializerOptions);
        this.TestOutputHelper.WriteLine($"Actual {JsonSerializer.Serialize(actual)}");

        actual.Should().NotBeNull();
        expectedSpeaker.Id = actual!.Id;
        actual.Should().BeEquivalentTo(expectedSpeaker);
    }
}