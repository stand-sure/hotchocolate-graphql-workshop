namespace ConferencePlanner.Service.Speaker;

using ConferencePlanner.Data;
using ConferencePlanner.Models;
using ConferencePlanner.Service.GraphQl;

using JetBrains.Annotations;

[PublicAPI]
[ExtendObjectType(OperationTypeNames.Mutation)]
public class SpeakerMutations
{
    [UseApplicationDbContext]
    public async Task<AddSpeakerPayload> AddSpeakerAsync(
        AddSpeakerInput input,
        [ScopedService] ApplicationDbContext context)
    {
        var speaker = new Speaker
        {
            Name = input.Name,
            Bio = input.Bio,
            Website = input.Website,
        };

        context.Speakers.Add(speaker);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return new AddSpeakerPayload(speaker);
    }
}