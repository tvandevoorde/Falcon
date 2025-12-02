using Falcon.Application.Contracts.Admin;
using Falcon.Application.Contracts.Alerts;
using Falcon.Application.Contracts.Auth;
using Falcon.Application.Contracts.Collectors;
using Falcon.Application.Contracts.Common;
using Falcon.Application.Contracts.Iis;
using Falcon.Application.Contracts.Logs;
using Falcon.Application.Contracts.Maintenance;
using Falcon.Application.Contracts.Servers;
using Falcon.Application.Contracts.Services;
using Falcon.Application.Contracts.Tasks;

namespace Falcon.Application.Abstractions;

/// <summary>
/// Defines the application service responsible for orchestrating monitoring workflows.
/// </summary>
public interface IMonitoringService
{
    /// <summary>
    /// Retrieves the authenticated user's profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User profile DTO.</returns>
    Task<UserProfileDto> GetCurrentUserProfileAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paged list of monitored servers.
    /// </summary>
    /// <param name="environment">Environment filter.</param>
    /// <param name="search">Search term.</param>
    /// <param name="page">Page index.</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged server summaries.</returns>
    Task<PagedResponseDto<ServerSummaryDto>> GetServersAsync(
        string? environment,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a server detail projection.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Server detail DTO or null.</returns>
    Task<ServerDetailDto?> GetServerAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Registers a new server with the platform.
    /// </summary>
    /// <param name="request">Server registration payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created server detail.</returns>
    Task<ServerDetailDto> CreateServerAsync(RegisterServerRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates server metadata.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="request">Update payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated server detail or null if not found.</returns>
    Task<ServerDetailDto?> UpdateServerAsync(Guid serverId, UpdateServerRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a server from the inventory.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteServerAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Schedules a service restart on a server by name.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="request">Restart request payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task TriggerServerServiceRestartAsync(Guid serverId, RestartServerServiceRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves services monitored on a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of services.</returns>
    Task<IReadOnlyCollection<MonitoredServiceDto>> GetServicesAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Registers a service for monitoring.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="request">Registration payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created service DTO.</returns>
    Task<MonitoredServiceDto> RegisterServiceAsync(Guid serverId, RegisterServiceRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a monitored service by identifier.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Service DTO or null.</returns>
    Task<MonitoredServiceDto?> GetServiceAsync(Guid serviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a monitored service.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteServiceAsync(Guid serviceId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves events for a monitored service.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="limit">Maximum number of events.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of service events.</returns>
    Task<IReadOnlyCollection<ServiceEventDto>> GetServiceEventsAsync(Guid serviceId, int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Queues a service restart command.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="request">Restart payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task TriggerServiceRestartAsync(Guid serviceId, RestartServiceRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves scheduled tasks for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of scheduled tasks.</returns>
    Task<IReadOnlyCollection<ScheduledTaskDto>> GetScheduledTasksAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a scheduled task by identifier.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Scheduled task DTO or null.</returns>
    Task<ScheduledTaskDto?> GetScheduledTaskAsync(Guid taskId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a scheduled task from monitoring.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteScheduledTaskAsync(Guid taskId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves task run history for a scheduled task.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="limit">Maximum number of runs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of task runs.</returns>
    Task<IReadOnlyCollection<TaskRunDto>> GetTaskRunsAsync(Guid taskId, int limit, CancellationToken cancellationToken);

    /// <summary>
    /// Triggers a manual task execution.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="request">Trigger payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task TriggerTaskAsync(Guid taskId, TriggerTaskRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves IIS application pools for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of app pools.</returns>
    Task<IReadOnlyCollection<AppPoolDto>> GetAppPoolsAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Requests an IIS application pool recycle.
    /// </summary>
    /// <param name="appPoolId">App pool identifier.</param>
    /// <param name="request">Recycle payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RecycleAppPoolAsync(Guid appPoolId, RecycleAppPoolRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves IIS sites for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of IIS sites.</returns>
    Task<IReadOnlyCollection<IisSiteDto>> GetIisSitesAsync(Guid serverId, CancellationToken cancellationToken);

    /// <summary>
    /// Executes a log search request.
    /// </summary>
    /// <param name="request">Log search payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged log entries.</returns>
    Task<PagedResponseDto<LogEntryDto>> SearchLogsAsync(LogSearchRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves log patterns configured in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of log patterns.</returns>
    Task<IReadOnlyCollection<LogPatternDto>> GetLogPatternsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates or updates a log pattern definition.
    /// </summary>
    /// <param name="request">Log pattern details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Persisted log pattern DTO.</returns>
    Task<LogPatternDto> UpsertLogPatternAsync(UpsertLogPatternRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves alerts with filters applied.
    /// </summary>
    /// <param name="status">Status filter.</param>
    /// <param name="severity">Severity filter.</param>
    /// <param name="serverId">Server filter.</param>
    /// <param name="sourceType">Source type filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged alerts.</returns>
    Task<PagedResponseDto<AlertDto>> GetAlertsAsync(
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
    /// <returns>Alert DTO or null.</returns>
    Task<AlertDto?> GetAlertAsync(Guid alertId, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a manual alert record.
    /// </summary>
    /// <param name="request">Alert creation payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created alert DTO.</returns>
    Task<AlertDto> CreateAlertAsync(CreateAlertRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Acknowledges an alert.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="request">Acknowledgement payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AcknowledgeAlertAsync(Guid alertId, AcknowledgeAlertRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Closes an alert.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="request">Closure payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CloseAlertAsync(Guid alertId, CloseAlertRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves notifications associated with an alert.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Notification DTOs.</returns>
    Task<IReadOnlyCollection<NotificationDto>> GetAlertNotificationsAsync(Guid alertId, CancellationToken cancellationToken);

    /// <summary>
    /// Resends a notification associated with an alert.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="notificationId">Notification identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ResendNotificationAsync(Guid alertId, Guid notificationId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves configured notification channels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Notification channel DTOs.</returns>
    Task<IReadOnlyCollection<NotificationChannelDto>> GetNotificationChannelsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates or updates a notification channel configuration.
    /// </summary>
    /// <param name="request">Channel payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Persisted channel DTO.</returns>
    Task<NotificationChannelDto> UpsertNotificationChannelAsync(UpsertNotificationChannelRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves server metric series.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="metricName">Metric name filter.</param>
    /// <param name="from">Start timestamp.</param>
    /// <param name="to">End timestamp.</param>
    /// <param name="interval">Aggregation interval label.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Metric point collection.</returns>
    Task<IReadOnlyCollection<MetricPointDto>> GetMetricsAsync(
        Guid serverId,
        string? metricName,
        DateTimeOffset? from,
        DateTimeOffset? to,
        string? interval,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves collectors registered with the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collector DTOs.</returns>
    Task<IReadOnlyCollection<CollectorDto>> GetCollectorsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Registers a collector with the platform.
    /// </summary>
    /// <param name="request">Collector registration payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created collector DTO.</returns>
    Task<CollectorDto> RegisterCollectorAsync(RegisterCollectorRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Records a collector heartbeat.
    /// </summary>
    /// <param name="collectorId">Collector identifier.</param>
    /// <param name="request">Heartbeat payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RecordCollectorHeartbeatAsync(Guid collectorId, CollectorHeartbeatRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Records a batch of service events from a collector.
    /// </summary>
    /// <param name="collectorId">Collector identifier.</param>
    /// <param name="request">Service event batch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RecordCollectorServiceEventsAsync(Guid collectorId, CollectorServiceEventBatchDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Records a batch of log entries from a collector.
    /// </summary>
    /// <param name="collectorId">Collector identifier.</param>
    /// <param name="request">Log batch payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RecordCollectorLogsAsync(Guid collectorId, CollectorLogBatchDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves application users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User DTOs.</returns>
    Task<IReadOnlyCollection<UserDto>> GetUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates an application user record.
    /// </summary>
    /// <param name="request">User creation payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created user DTO.</returns>
    Task<UserDto> CreateUserAsync(CreateUserRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves application roles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Role DTOs.</returns>
    Task<IReadOnlyCollection<RoleDto>> GetRolesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates a role definition.
    /// </summary>
    /// <param name="request">Role creation payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created role DTO.</returns>
    Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves maintenance windows.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Maintenance window DTOs.</returns>
    Task<IReadOnlyCollection<MaintenanceWindowDto>> GetMaintenanceWindowsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates or updates a maintenance window.
    /// </summary>
    /// <param name="request">Maintenance window payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Persisted maintenance window DTO.</returns>
    Task<MaintenanceWindowDto> UpsertMaintenanceWindowAsync(UpsertMaintenanceWindowRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a tail of log entries for streaming scenarios.
    /// </summary>
    /// <param name="logFileId">Log file identifier.</param>
    /// <param name="from">Starting timestamp.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of log entries in chronological order.</returns>
    Task<IReadOnlyCollection<LogEntryDto>> TailLogsAsync(Guid logFileId, DateTimeOffset? from, CancellationToken cancellationToken);
}