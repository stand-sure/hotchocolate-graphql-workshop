namespace UnitTests.Services.GraphQl.Speaker;

using System.Reflection;
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
    public void AddSpeakerAsyncShouldUseUseApplicationDbContextAttribute()
    {
        var a = this.GetAddSpeakerAsyncMethod()?
            .GetCustomAttribute<UseApplicationDbContextAttribute>();

        a.Should().NotBeNull();
    }

    [Fact]
    public void AddSpeakerAsyncShouldUseScopedDbContext()
    {
        var parameterInfo = this.GetAddSpeakerAsyncMethod()?
            .GetParameters().Single(info => info.ParameterType == typeof(ApplicationDbContext));

        var a = parameterInfo?.GetCustomAttribute<ScopedServiceAttribute>();

        a.Should().NotBeNull();
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

        string query = @"mutation addSpeaker{addSpeaker(input:{bio:""" +
                       expectedSpeaker.Bio +
                       @""",name:""" +
                       expectedSpeaker.Name +
                       @""",website:""" +
                       expectedSpeaker.Website +
                       @"""}){speaker{bio id name website}}}";

        string result = await this.ExecuteQueryAsync(query).ConfigureAwait(false);

        this.TestOutputHelper.WriteLine($"result {result}");

        var speaker = System.Text.Json.Nodes.JsonNode.Parse(result)!["data"]!["addSpeaker"]!["speaker"];
        var actual = speaker.Deserialize<Speaker>(QueryTestsBase.JsonSerializerOptions);
        this.TestOutputHelper.WriteLine($"Actual {JsonSerializer.Serialize(actual)}");

        expectedSpeaker.Id = actual.Id;
        actual.Should().BeEquivalentTo(expectedSpeaker);
    }

    private MethodInfo? GetAddSpeakerAsyncMethod()
    {
        return this.Target.GetType().GetMethod(
            nameof(this.Target.AddSpeakerAsync),
            BindingFlags.Instance | BindingFlags.Public,
            null,
            new[]
            {
                typeof(AddSpeakerInput),
                typeof(ApplicationDbContext),
            },
            null);
    }
}