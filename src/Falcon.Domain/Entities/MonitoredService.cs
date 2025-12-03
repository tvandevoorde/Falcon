using Falcon.Domain.Enumerations;

namespace Falcon.Domain.Entities;

/// <summary>
/// Represents a Windows service monitored by the platform.
/// </summary>
public sealed class MonitoredService(
    Guid id,
    Guid serverId,
    string serviceName,
    ServiceDesiredState desiredState,
    ServiceState currentState,
    bool critical)
{
    private readonly List<ServiceEvent> events = [];

    public Guid Id { get; } = id;

    public Guid ServerId { get; } = serverId;

    public string ServiceName { get; } = serviceName;

    public string? DisplayName { get; init; }

    public ServiceDesiredState DesiredState { get; private set; } = desiredState;

    public ServiceState CurrentState { get; private set; } = currentState;

    public bool Critical { get; private set; } = critical;

    public DateTimeOffset? LastChange { get; private set; }

    public IReadOnlyCollection<ServiceEvent> Events => events.AsReadOnly();

    /// <summary>
    /// Updates the runtime state for the service.
    /// </summary>
    /// <param name="state">New runtime state.</param>
    /// <param name="changedAt">Timestamp of the change.</param>
    public void UpdateState(ServiceState state, DateTimeOffset? changedAt)
    {
        CurrentState = state;
        LastChange = changedAt;
    }

    /// <summary>
    /// Adjusts the desired state for orchestration purposes.
    /// </summary>
    /// <param name="desiredState">Desired target state.</param>
    public void UpdateDesiredState(ServiceDesiredState desiredState)
    {
        DesiredState = desiredState;
    }

    /// <summary>
    /// Updates the criticality flag for alerting purposes.
    /// </summary>
    /// <param name="critical">Whether the service is critical.</param>
    public void UpdateCriticality(bool critical)
    {
        Critical = critical;
    }

    /// <summary>
    /// Records a service event in chronological order.
    /// </summary>
    /// <param name="serviceEvent">Event to append.</param>
    public void AddEvent(ServiceEvent serviceEvent)
    {
        events.Add(serviceEvent);
    }
}
