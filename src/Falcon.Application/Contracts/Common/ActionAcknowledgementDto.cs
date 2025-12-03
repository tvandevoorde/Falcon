namespace Falcon.Application.Contracts.Common;

/// <summary>
/// Represents a standardized acknowledgement response for accepted background actions.
/// </summary>
public sealed class ActionAcknowledgementDto
{
    public Guid ActionId { get; init; }

    public DateTimeOffset ScheduledAt { get; init; }

    public string? Status { get; init; }
}
