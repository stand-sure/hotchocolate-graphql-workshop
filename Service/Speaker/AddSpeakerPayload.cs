namespace ConferencePlanner.Service.Speaker;

using ConferencePlanner.Models;

public class AddSpeakerPayload
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddSpeakerPayload"/> class.
    /// </summary>
    public AddSpeakerPayload(Speaker speaker)
    {
        this.Speaker = speaker;
    }

    public Speaker Speaker { get; }
}