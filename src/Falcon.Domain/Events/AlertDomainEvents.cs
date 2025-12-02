namespace Falcon.Domain.Events;

/// <summary>
/// Domain event emitted when an alert transitions state.
/// </summary>
/// <param name="AlertId">Identifier for the alert.</param>
/// <param name="PreviousStatus">Status before the transition.</param>
/// <param name="CurrentStatus">Status after the transition.</param>
public sealed record AlertStatusChangedDomainEvent(Guid AlertId, string PreviousStatus, string CurrentStatus);

/// <summary>
/// Domain event emitted when a notification is queued for delivery.
/// </summary>
/// <param name="AlertId">Identifier for the associated alert.</param>
/// <param name="NotificationId">Identifier of the queued notification.</param>
public sealed record NotificationQueuedDomainEvent(Guid AlertId, Guid NotificationId);