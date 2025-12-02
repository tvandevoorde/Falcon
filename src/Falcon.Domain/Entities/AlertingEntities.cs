using Falcon.Domain.Enumerations;

namespace Falcon.Domain.Entities;

/// <summary>
/// Represents an alert raised by the monitoring platform.
/// </summary>
public sealed class Alert
{
    private readonly List<Notification> notifications = new();

    public Alert(
        Guid id,
        Guid? serverId,
        string sourceType,
        Guid? sourceId,
        string alertType,
        AlertSeverity severity,
        string status,
        string message,
        DateTimeOffset createdAt)
    {
        Id = id;
        ServerId = serverId;
        SourceType = sourceType;
        SourceId = sourceId;
        AlertType = alertType;
        Severity = severity;
        Status = status;
        Message = message;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public Guid? ServerId { get; }

    public string SourceType { get; }

    public Guid? SourceId { get; }

    public string AlertType { get; }

    public AlertSeverity Severity { get; private set; }

    public string Status { get; private set; }

    public string Message { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? ResolvedAt { get; private set; }

    public IReadOnlyCollection<Notification> Notifications => notifications.AsReadOnly();

    public IReadOnlyCollection<Guid> RelatedLogIds => relatedLogIds.AsReadOnly();

    private readonly List<Guid> relatedLogIds = new();

    /// <summary>
    /// Associates a log entry with the alert for easier correlation.
    /// </summary>
    /// <param name="logId">Log entry identifier.</param>
    public void AddRelatedLog(Guid logId)
    {
        if (!relatedLogIds.Contains(logId))
        {
            relatedLogIds.Add(logId);
        }
    }

    /// <summary>
    /// Updates the human readable message and severity of the alert.
    /// </summary>
    /// <param name="message">New message to store.</param>
    /// <param name="severity">New severity classification.</param>
    public void UpdateMessage(string message, AlertSeverity severity)
    {
        Message = message;
        Severity = severity;
    }

    /// <summary>
    /// Transitions the alert to a new status.
    /// </summary>
    /// <param name="status">Lifecycle status (open, acknowledged, closed).</param>
    /// <param name="resolvedAt">Optional resolution timestamp.</param>
    public void UpdateStatus(string status, DateTimeOffset? resolvedAt)
    {
        Status = status;
        ResolvedAt = resolvedAt;
    }

    /// <summary>
    /// Adds a notification record to the alert history.
    /// </summary>
    /// <param name="notification">Notification instance.</param>
    public void AddNotification(Notification notification)
    {
        notifications.Add(notification);
    }
}

/// <summary>
/// Represents a notification delivery attempt for an alert.
/// </summary>
public sealed class Notification
{
    public Notification(
        Guid id,
        Guid alertId,
        NotificationChannel channel,
        string recipient,
        NotificationStatus status,
        int attemptCount,
        DateTimeOffset? lastAttempt,
        IDictionary<string, object>? payload)
    {
        Id = id;
        AlertId = alertId;
        Channel = channel;
        Recipient = recipient;
        Status = status;
        AttemptCount = attemptCount;
        LastAttempt = lastAttempt;
        Payload = payload;
    }

    public Guid Id { get; }

    public Guid AlertId { get; }

    public NotificationChannel Channel { get; }

    public string Recipient { get; }

    public NotificationStatus Status { get; private set; }

    public int AttemptCount { get; private set; }

    public DateTimeOffset? LastAttempt { get; private set; }

    public IDictionary<string, object>? Payload { get; }

    /// <summary>
    /// Updates the delivery status of the notification.
    /// </summary>
    /// <param name="status">New status value.</param>
    /// <param name="attemptedAt">Timestamp of the attempt.</param>
    public void UpdateStatus(NotificationStatus status, DateTimeOffset? attemptedAt)
    {
        Status = status;
        LastAttempt = attemptedAt;
        AttemptCount++;
    }
}

/// <summary>
/// Represents a log pattern used to detect known issues.
/// </summary>
public sealed class LogPattern
{
    public LogPattern(Guid id, string name, string pattern, string severityDefault, bool enabled)
    {
        Id = id;
        Name = name;
        Pattern = pattern;
        SeverityDefault = severityDefault;
        Enabled = enabled;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public string Pattern { get; private set; }

    public string SeverityDefault { get; private set; }

    public bool Enabled { get; private set; }

    /// <summary>
    /// Updates the log pattern definition.
    /// </summary>
    /// <param name="name">Friendly name.</param>
    /// <param name="pattern">Regular expression or pattern.</param>
    /// <param name="severityDefault">Default severity label.</param>
    /// <param name="enabled">Enabled flag.</param>
    public void Update(string name, string pattern, string severityDefault, bool enabled)
    {
        Name = name;
        Pattern = pattern;
        SeverityDefault = severityDefault;
        Enabled = enabled;
    }
}

/// <summary>
/// Represents a monitoring collector agent or integration configuration.
/// </summary>
public sealed class Collector
{
    public Collector(Guid id, string name, CollectorType type)
    {
        Id = id;
        Name = name;
        Type = type;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public CollectorType Type { get; private set; }

    public IDictionary<string, object>? Config { get; private set; }

    public DateTimeOffset? LastSeen { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Applies configuration payload to the collector.
    /// </summary>
    /// <param name="config">Arbitrary configuration object tree.</param>
    public void ApplyConfiguration(IDictionary<string, object>? config)
    {
        Config = config;
    }

    /// <summary>
    /// Updates the last seen heartbeat metadata.
    /// </summary>
    /// <param name="lastSeen">Timestamp when collector was last online.</param>
    public void UpdateLastSeen(DateTimeOffset lastSeen)
    {
        LastSeen = lastSeen;
    }

    /// <summary>
    /// Updates the collector display metadata.
    /// </summary>
    /// <param name="name">Collector friendly name.</param>
    /// <param name="type">Collector type enumeration.</param>
    public void UpdateMetadata(string name, CollectorType type)
    {
        Name = name;
        Type = type;
    }
}

/// <summary>
/// Represents a planned maintenance window used to mute alerts.
/// </summary>
public sealed class MaintenanceWindow
{
    public MaintenanceWindow(Guid id, string name, DateTimeOffset startTime, DateTimeOffset endTime, bool muted)
    {
        Id = id;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        Muted = muted;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public DateTimeOffset StartTime { get; private set; }

    public DateTimeOffset EndTime { get; private set; }

    public bool Muted { get; private set; }

    public IReadOnlyCollection<Guid> ServerScope => serverScope.AsReadOnly();

    private readonly List<Guid> serverScope = new();

    /// <summary>
    /// Assigns the servers included in the maintenance window scope.
    /// </summary>
    /// <param name="serverIds">Collection of server identifiers.</param>
    public void SetServerScope(IEnumerable<Guid> serverIds)
    {
        serverScope.Clear();
        serverScope.AddRange(serverIds);
    }

    /// <summary>
    /// Updates the scheduling metadata for the window.
    /// </summary>
    /// <param name="name">Window name.</param>
    /// <param name="start">Start timestamp.</param>
    /// <param name="end">End timestamp.</param>
    /// <param name="muted">Whether alerts should be muted.</param>
    public void Update(string name, DateTimeOffset start, DateTimeOffset end, bool muted)
    {
        Name = name;
        StartTime = start;
        EndTime = end;
        Muted = muted;
    }
}

/// <summary>
/// Represents an application role used for RBAC.
/// </summary>
public sealed class Role
{
    public Role(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    /// <summary>
    /// Updates descriptive metadata about the role.
    /// </summary>
    /// <param name="name">Role name.</param>
    /// <param name="description">Role description.</param>
    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}

/// <summary>
/// Represents an authenticated user inside the platform.
/// </summary>
public sealed class User
{
    private readonly List<RoleAssignment> roleAssignments = new();

    public User(Guid id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }

    public string Username { get; private set; }

    public string? DisplayName { get; private set; }

    public string Email { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public IReadOnlyCollection<RoleAssignment> RoleAssignments => roleAssignments.AsReadOnly();

    /// <summary>
    /// Updates the profile metadata for the user.
    /// </summary>
    /// <param name="displayName">Friendly display name.</param>
    /// <param name="email">Updated email address.</param>
    public void UpdateProfile(string? displayName, string email)
    {
        DisplayName = displayName;
        Email = email;
    }

    /// <summary>
    /// Assigns a role to the user with optional scope data.
    /// </summary>
    /// <param name="roleAssignment">Role assignment payload.</param>
    public void AddRole(RoleAssignment roleAssignment)
    {
        roleAssignments.RemoveAll(r => r.RoleId == roleAssignment.RoleId);
        roleAssignments.Add(roleAssignment);
    }
}

/// <summary>
/// Represents the association between a user and a role with optional scope.
/// </summary>
public sealed class RoleAssignment
{
    public RoleAssignment(Guid id, Guid userId, Guid roleId, IDictionary<string, object>? scope)
    {
        Id = id;
        UserId = userId;
        RoleId = roleId;
        Scope = scope;
    }

    public Guid Id { get; }

    public Guid UserId { get; }

    public Guid RoleId { get; }

    public IDictionary<string, object>? Scope { get; }
}

/// <summary>
/// Represents an outbound notification channel configuration.
/// </summary>
public sealed class NotificationChannel
{
    public NotificationChannel(Guid id, string channel, IDictionary<string, object>? settings)
    {
        Id = id;
        Channel = channel;
        Settings = settings;
    }

    public Guid Id { get; }

    public string Channel { get; private set; }

    public IDictionary<string, object>? Settings { get; private set; }

    /// <summary>
    /// Updates channel metadata and settings.
    /// </summary>
    /// <param name="channel">Channel identifier.</param>
    /// <param name="settings">Channel settings.</param>
    public void Update(string channel, IDictionary<string, object>? settings)
    {
        Channel = channel;
        Settings = settings;
    }
}