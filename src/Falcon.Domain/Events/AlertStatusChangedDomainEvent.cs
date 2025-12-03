namespace Falcon.Domain.Events;

/// <summary>
/// Domain event emitted when an alert transitions state.
/// </summary>
/// <param name="AlertId">Identifier for the alert.</param>
/// <param name="PreviousStatus">Status before the transition.</param>
/// <param name="CurrentStatus">Status after the transition.</param>
public sealed record AlertStatusChangedDomainEvent(Guid AlertId, string PreviousStatus, string CurrentStatus);
