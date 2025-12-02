using Falcon.Domain.Entities;
using Falcon.Domain.ValueObjects;

namespace Falcon.Domain.Repositories;

/// <summary>
/// Defines the repository contract for interacting with monitoring aggregates.
/// </summary>
public interface IMonitoringRepository
{
    /// <summary>
    /// Retrieves a paged list of servers with optional filtering.
    /// </summary>
    /// <param name="environment">Environment filter.</param>
    /// <param name="search">Search term across hostnames and display names.</param>
    /// <param name="page">Page index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged result containing servers.</returns>
    Task<PagedResult<Server>> ListServersAsync(
        string? environment,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a server aggregate by identifier.
    /// </summary>
    /// <param name="serverId">Unique server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Server aggregate or null if not found.</returns>
    Task<Server?> GetServerAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Persists a new server aggregate.
    /// </summary>
    /// <param name="server">Server aggregate to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Persisted server aggregate.</returns>
    Task<Server> AddServerAsync(Server server, CancellationToken cancellationToken);

    /// <summary>
    /// Persists modifications to an existing server.
    /// </summary>
    /// <param name="server">Server aggregate to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateServerAsync(Server server, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a server aggregate.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteServerAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a monitored service by identifier.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Service entity or null.</returns>
    Task<MonitoredService?> GetServiceAsync(Guid serviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves services for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Service collection.</returns>
    Task<IReadOnlyCollection<MonitoredService>> ListServicesAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds or replaces a monitored service.
    /// </summary>
    /// <param name="service">Service to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddServiceAsync(MonitoredService service, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a monitored service.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveServiceAsync(Guid serviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves recent service events for a service.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="limit">Maximum number of events.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of events ordered descending.</returns>
    Task<IReadOnlyCollection<ServiceEvent>> ListServiceEventsAsync(
        Guid serviceId,
        int limit,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves scheduled tasks for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of scheduled tasks.</returns>
    Task<IReadOnlyCollection<ScheduledTask>> ListScheduledTasksAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a scheduled task by identifier.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Scheduled task or null.</returns>
    Task<ScheduledTask?> GetScheduledTaskAsync(Guid taskId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds or replaces a scheduled task.
    /// </summary>
    /// <param name="task">Scheduled task instance.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddScheduledTaskAsync(ScheduledTask task, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a scheduled task by identifier.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveScheduledTaskAsync(Guid taskId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves recent task runs for a scheduled task.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="limit">Number of runs requested.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of recent task runs.</returns>
    Task<IReadOnlyCollection<TaskRun>> ListTaskRunsAsync(Guid taskId, int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves IIS application pools for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of app pools.</returns>
    Task<IReadOnlyCollection<AppPool>> ListAppPoolsAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an app pool by identifier.
    /// </summary>
    /// <param name="appPoolId">App pool identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>App pool or null.</returns>
    Task<AppPool?> GetAppPoolAsync(Guid appPoolId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves IIS sites for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of sites.</returns>
    Task<IReadOnlyCollection<IisSite>> ListIisSitesAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves monitored log files for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of log files.</returns>
    Task<IReadOnlyCollection<LogFile>> ListLogFilesAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves metric points for a server and metric name.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="metricName">Metric name filter.</param>
    /// <param name="from">Start timestamp.</param>
    /// <param name="to">End timestamp.</param>
    /// <param name="interval">Aggregation interval label.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Metric points ordered by measured timestamp.</returns>
    Task<IReadOnlyCollection<MetricPoint>> ListMetricPointsAsync(
        Guid serverId,
        string? metricName,
        DateTimeOffset? from,
        DateTimeOffset? to,
        string? interval,
        CancellationToken cancellationToken);

    /// <summary>
    /// Performs a log search across servers and files.
    /// </summary>
    /// <param name="serverId">Server filter.</param>
    /// <param name="logFileIds">Specific log files to include.</param>
    /// <param name="severity">Severity filter.</param>
    /// <param name="text">Full text search expression.</param>
    /// <param name="from">Start timestamp.</param>
    /// <param name="to">End timestamp.</param>
    /// <param name="page">Page index.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged log entries.</returns>
    Task<PagedResult<LogEntry>> SearchLogsAsync(
        Guid? serverId,
        IReadOnlyCollection<Guid>? logFileIds,
        string? severity,
        string? text,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves log patterns configured for parsing.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of log patterns.</returns>
    Task<IReadOnlyCollection<LogPattern>> ListLogPatternsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds or updates a log pattern.
    /// </summary>
    /// <param name="pattern">Log pattern to upsert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpsertLogPatternAsync(LogPattern pattern, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves alerts with optional filtering.
    /// </summary>
    /// <param name="status">Status filter.</param>
    /// <param name="severity">Severity filter.</param>
    /// <param name="serverId">Server filter.</param>
    /// <param name="sourceType">Source type filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged alerts.</returns>
    Task<PagedResult<Alert>> ListAlertsAsync(
        string? status,
        string? severity,
        Guid? serverId,
        string? sourceType,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an alert by identifier.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Alert instance or null.</returns>
    Task<Alert?> GetAlertAsync(Guid alertId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new alert to the store.
    /// </summary>
    /// <param name="alert">Alert to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAlertAsync(Alert alert, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing alert.
    /// </summary>
    /// <param name="alert">Alert to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAlertAsync(Alert alert, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves notifications for an alert.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Notifications list.</returns>
    Task<IReadOnlyCollection<Notification>> ListNotificationsAsync(Guid alertId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves configured notification channels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Notification channels.</returns>
    Task<IReadOnlyCollection<NotificationChannel>> ListNotificationChannelsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds or updates a notification channel configuration.
    /// </summary>
    /// <param name="channel">Notification channel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Persisted channel.</returns>
    Task<NotificationChannel> UpsertNotificationChannelAsync(NotificationChannel channel, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves collectors currently registered.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collectors list.</returns>
    Task<IReadOnlyCollection<Collector>> ListCollectorsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a collector by identifier.
    /// </summary>
    /// <param name="collectorId">Collector identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collector instance or null.</returns>
    Task<Collector?> GetCollectorAsync(Guid collectorId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds or updates a collector configuration.
    /// </summary>
    /// <param name="collector">Collector to upsert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpsertCollectorAsync(Collector collector, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves users in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Users list.</returns>
    Task<IReadOnlyCollection<User>> ListUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new user to the store.
    /// </summary>
    /// <param name="user">User entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddUserAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves roles defined in the platform.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Roles collection.</returns>
    Task<IReadOnlyCollection<Role>> ListRolesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a role to the store.
    /// </summary>
    /// <param name="role">Role entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddRoleAsync(Role role, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves maintenance windows.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Maintenance window collection.</returns>
    Task<IReadOnlyCollection<MaintenanceWindow>> ListMaintenanceWindowsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds or updates a maintenance window.
    /// </summary>
    /// <param name="maintenanceWindow">Maintenance window entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpsertMaintenanceWindowAsync(MaintenanceWindow maintenanceWindow, CancellationToken cancellationToken);
}