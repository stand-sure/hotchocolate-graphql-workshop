namespace ConferencePlanner.Service.Speaker;

using ConferencePlanner.Data;
using ConferencePlanner.Models;
using ConferencePlanner.Service.GraphQl;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

/// <summary>
///     SpeakerQueries
/// </summary>
[PublicAPI]
[ExtendObjectType(OperationTypeNames.Query)]
public class SpeakerQueries
{
    [UseApplicationDbContext]
    public IQueryable<Speaker> GetSpeakers(
        [ScopedService] ApplicationDbContext context)
    {
        return context.Speakers.AsNoTracking();
    }
}