namespace Falcon.Domain.Events;

/// <summary>
/// Domain event emitted when a notification is queued for delivery.
/// </summary>
/// <param name="AlertId">Identifier for the associated alert.</param>
/// <param name="NotificationId">Identifier of the queued notification.</param>
public sealed record NotificationQueuedDomainEvent(Guid AlertId, Guid NotificationId);