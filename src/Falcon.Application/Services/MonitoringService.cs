using AutoMapper;
using Falcon.Application.Abstractions;
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
using Falcon.Domain.Entities;
using Falcon.Domain.Enumerations;
using Falcon.Domain.Repositories;
using Falcon.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NotificationChannelEntity = Falcon.Domain.Entities.NotificationChannel;

namespace Falcon.Application.Services;

/// <summary>
/// Provides orchestration logic for the Falcon monitoring platform.
/// </summary>
public sealed class MonitoringService(IMonitoringRepository repository, IMapper mapper, ILogger<MonitoringService> logger) : IMonitoringService
{
    private readonly IMonitoringRepository repository = repository;
    private readonly IMapper mapper = mapper;
    private readonly ILogger<MonitoringService> logger = logger;

    /// <inheritdoc />
    public async Task<UserProfileDto> GetCurrentUserProfileAsync(CancellationToken cancellationToken)
    {
        var users = await repository.ListUsersAsync(cancellationToken).ConfigureAwait(false);
        var roles = await repository.ListRolesAsync(cancellationToken).ConfigureAwait(false);

        var user = users.FirstOrDefault();
        if (user is null)
        {
            return new UserProfileDto
            {
                Id = Guid.Empty,
                Username = "system",
                DisplayName = "System",
                Email = "noreply@example.com",
                Roles = [],
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        var assignedRoles = user.RoleAssignments
            .Select(assignment => roles.FirstOrDefault(r => r.Id == assignment.RoleId)?.Name ?? assignment.RoleId.ToString())
            .ToArray();

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Roles = assignedRoles,
            CreatedAt = user.CreatedAt
        };
    }

    /// <inheritdoc />
    public async Task<PagedResponseDto<ServerSummaryDto>> GetServersAsync(
        string? environment,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await repository.ListServersAsync(environment, search, page, pageSize, cancellationToken).ConfigureAwait(false);
        return MapPaged(result, mapper.Map<IReadOnlyCollection<ServerSummaryDto>>(result.Items));
    }

    /// <inheritdoc />
    public async Task<ServerDetailDto?> GetServerAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var server = await repository.GetServerAsync(serverId, cancellationToken).ConfigureAwait(false);
        if (server is null)
        {
            return null;
        }

        var detail = mapper.Map<ServerDetailDto>(server);
        detail = detail with
        {
            ServiceSummary = BuildServiceSummary(server),
            TasksSummary = BuildTaskSummary(server),
            IisSummary = BuildIisSummary(server),
            Metrics = new ServerMetricSnapshotDto
            {
                CpuPercent = server.CpuPercent,
                MemoryPercent = server.MemoryPercent
            },
            RecentAlerts = await LoadRecentAlertsAsync(serverId, cancellationToken).ConfigureAwait(false)
        };

        return detail;
    }

    /// <inheritdoc />
    public async Task<ServerDetailDto> CreateServerAsync(RegisterServerRequestDto request, CancellationToken cancellationToken)
    {
        var server = new Server(Guid.NewGuid(), request.Hostname, ParseEnvironment(request.Environment), ServerStatus.Unknown);
        server.UpdateMetadata(request.DisplayName, ParseEnvironment(request.Environment), request.IpAddress, request.Os, request.Tags);
        await repository.AddServerAsync(server, cancellationToken).ConfigureAwait(false);

        logger.LogInformation("Registered server {Hostname} ({ServerId})", request.Hostname, server.Id);

        return mapper.Map<ServerDetailDto>(server);
    }

    /// <inheritdoc />
    public async Task<ServerDetailDto?> UpdateServerAsync(Guid serverId, UpdateServerRequestDto request, CancellationToken cancellationToken)
    {
        var server = await repository.GetServerAsync(serverId, cancellationToken).ConfigureAwait(false);
        if (server is null)
        {
            return null;
        }

        server.UpdateMetadata(request.DisplayName, ParseEnvironment(request.Environment), request.IpAddress, request.Os, request.Tags);
        await repository.UpdateServerAsync(server, cancellationToken).ConfigureAwait(false);

        return mapper.Map<ServerDetailDto>(server);
    }

    /// <inheritdoc />
    public Task DeleteServerAsync(Guid serverId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting server {ServerId}", serverId);
        return repository.DeleteServerAsync(serverId, cancellationToken);
    }

    /// <inheritdoc />
    public Task TriggerServerServiceRestartAsync(Guid serverId, RestartServerServiceRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Queued service restart for {Service} on server {ServerId} because {Reason}", request.ServiceName, serverId, request.Reason);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MonitoredServiceDto>> GetServicesAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var services = await repository.ListServicesAsync(serverId, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<MonitoredServiceDto>>(services);
    }

    /// <inheritdoc />
    public async Task<MonitoredServiceDto> RegisterServiceAsync(Guid serverId, RegisterServiceRequestDto request, CancellationToken cancellationToken)
    {
        var service = new MonitoredService(Guid.NewGuid(), serverId, request.ServiceName, ParseDesiredState(request.DesiredState), ServiceState.Unknown, request.Critical)
        {
            DisplayName = request.DisplayName
        };

        await repository.AddServiceAsync(service, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Registered service {ServiceName} for server {ServerId}", request.ServiceName, serverId);
        return mapper.Map<MonitoredServiceDto>(service);
    }

    /// <inheritdoc />
    public async Task<MonitoredServiceDto?> GetServiceAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var service = await repository.GetServiceAsync(serviceId, cancellationToken).ConfigureAwait(false);
        return service is null ? null : mapper.Map<MonitoredServiceDto>(service);
    }

    /// <inheritdoc />
    public Task DeleteServiceAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing monitored service {ServiceId}", serviceId);
        return repository.RemoveServiceAsync(serviceId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ServiceEventDto>> GetServiceEventsAsync(Guid serviceId, int limit, CancellationToken cancellationToken)
    {
        var events = await repository.ListServiceEventsAsync(serviceId, limit, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<ServiceEventDto>>(events.OrderByDescending(e => e.EventTime).ToList());
    }

    /// <inheritdoc />
    public Task TriggerServiceRestartAsync(Guid serviceId, RestartServiceRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Queued restart for service {ServiceId}: {Reason}", serviceId, request.Reason);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ScheduledTaskDto>> GetScheduledTasksAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var tasks = await repository.ListScheduledTasksAsync(serverId, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<ScheduledTaskDto>>(tasks);
    }

    /// <inheritdoc />
    public async Task<ScheduledTaskDto?> GetScheduledTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var task = await repository.GetScheduledTaskAsync(taskId, cancellationToken).ConfigureAwait(false);
        return task is null ? null : mapper.Map<ScheduledTaskDto>(task);
    }

    /// <inheritdoc />
    public Task DeleteScheduledTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing scheduled task {TaskId}", taskId);
        return repository.RemoveScheduledTaskAsync(taskId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<TaskRunDto>> GetTaskRunsAsync(Guid taskId, int limit, CancellationToken cancellationToken)
    {
        var runs = await repository.ListTaskRunsAsync(taskId, limit, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<TaskRunDto>>(runs.OrderByDescending(r => r.StartTime ?? r.EndTime).ToList());
    }

    /// <inheritdoc />
    public Task TriggerTaskAsync(Guid taskId, TriggerTaskRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Manual trigger requested for task {TaskId}: {Reason}", taskId, request.Reason);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AppPoolDto>> GetAppPoolsAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var pools = await repository.ListAppPoolsAsync(serverId, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<AppPoolDto>>(pools);
    }

    /// <inheritdoc />
    public Task RecycleAppPoolAsync(Guid appPoolId, RecycleAppPoolRequestDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Queued recycle for app pool {AppPoolId}: {Reason}", appPoolId, request.Reason);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IisSiteDto>> GetIisSitesAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var sites = await repository.ListIisSitesAsync(serverId, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<IisSiteDto>>(sites);
    }

    /// <inheritdoc />
    public async Task<PagedResponseDto<LogEntryDto>> SearchLogsAsync(LogSearchRequestDto request, CancellationToken cancellationToken)
    {
        var result = await repository.SearchLogsAsync(
            request.ServerIds,
            request.LogFileIds,
            request.Severities,
            request.Query,
            request.From,
            request.To,
            request.Page,
            request.PageSize,
            cancellationToken).ConfigureAwait(false);

        return MapPaged(result, mapper.Map<IReadOnlyCollection<LogEntryDto>>(result.Items));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<LogPatternDto>> GetLogPatternsAsync(CancellationToken cancellationToken)
    {
        var patterns = await repository.ListLogPatternsAsync(cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<LogPatternDto>>(patterns);
    }

    /// <inheritdoc />
    public async Task<LogPatternDto> UpsertLogPatternAsync(UpsertLogPatternRequestDto request, CancellationToken cancellationToken)
    {
        var pattern = new LogPattern(Guid.NewGuid(), request.Name, request.Pattern, request.SeverityDefault, request.Enabled);
        await repository.UpsertLogPatternAsync(pattern, cancellationToken).ConfigureAwait(false);
        return mapper.Map<LogPatternDto>(pattern);
    }

    /// <inheritdoc />
    public async Task<PagedResponseDto<AlertDto>> GetAlertsAsync(string? status, string? severity, Guid? serverId, string? sourceType, CancellationToken cancellationToken)
    {
        var result = await repository.ListAlertsAsync(status, severity, serverId, sourceType, cancellationToken).ConfigureAwait(false);
        var enrichedAlerts = mapper.Map<List<AlertDto>>(result.Items)
            .Select(alertDto =>
            {
                var alert = result.Items.First(a => a.Id == alertDto.Id);
                var notifications = mapper.Map<IReadOnlyCollection<NotificationDto>>(alert.Notifications);
                return alertDto with { Notifications = notifications };
            })
            .ToList();

        return MapPaged(result, enrichedAlerts);
    }

    /// <inheritdoc />
    public async Task<AlertDto?> GetAlertAsync(Guid alertId, CancellationToken cancellationToken)
    {
        var alert = await repository.GetAlertAsync(alertId, cancellationToken).ConfigureAwait(false);
        if (alert is null)
        {
            return null;
        }

        var dto = mapper.Map<AlertDto>(alert);
        var notifications = mapper.Map<IReadOnlyCollection<NotificationDto>>(alert.Notifications);
        return dto with { Notifications = notifications };
    }

    /// <inheritdoc />
    public async Task<AlertDto> CreateAlertAsync(CreateAlertRequestDto request, CancellationToken cancellationToken)
    {
        var alert = new Alert(
            Guid.NewGuid(),
            request.ServerId,
            request.SourceType,
            request.SourceId,
            request.AlertType,
            ParseAlertSeverity(request.Severity),
            AlertStatus.Open.ToString().ToLowerInvariant(),
            request.Message,
            DateTimeOffset.UtcNow);

        await repository.AddAlertAsync(alert, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Created manual alert {AlertId}", alert.Id);
        return mapper.Map<AlertDto>(alert);
    }

    /// <inheritdoc />
    public async Task AcknowledgeAlertAsync(Guid alertId, AcknowledgeAlertRequestDto request, CancellationToken cancellationToken)
    {
        var alert = await repository.GetAlertAsync(alertId, cancellationToken).ConfigureAwait(false);
        if (alert is null)
        {
            return;
        }

        alert.UpdateStatus(AlertStatus.Acknowledged.ToString().ToLowerInvariant(), alert.ResolvedAt);
        await repository.UpdateAlertAsync(alert, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Acknowledged alert {AlertId}: {Comment}", alertId, request.Comment);
    }

    /// <inheritdoc />
    public async Task CloseAlertAsync(Guid alertId, CloseAlertRequestDto request, CancellationToken cancellationToken)
    {
        var alert = await repository.GetAlertAsync(alertId, cancellationToken).ConfigureAwait(false);
        if (alert is null)
        {
            return;
        }

        alert.UpdateStatus(AlertStatus.Closed.ToString().ToLowerInvariant(), DateTimeOffset.UtcNow);
        await repository.UpdateAlertAsync(alert, cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Closed alert {AlertId}: {Resolution}", alertId, request.Resolution);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<NotificationDto>> GetAlertNotificationsAsync(Guid alertId, CancellationToken cancellationToken)
    {
        var notifications = await repository.ListNotificationsAsync(alertId, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<NotificationDto>>(notifications);
    }

    /// <inheritdoc />
    public Task ResendNotificationAsync(Guid alertId, Guid notificationId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Resend requested for notification {NotificationId} on alert {AlertId}", notificationId, alertId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<NotificationChannelDto>> GetNotificationChannelsAsync(CancellationToken cancellationToken)
    {
        var channels = await repository.ListNotificationChannelsAsync(cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<NotificationChannelDto>>(channels);
    }

    /// <inheritdoc />
    public async Task<NotificationChannelDto> UpsertNotificationChannelAsync(UpsertNotificationChannelRequestDto request, CancellationToken cancellationToken)
    {
        var channel = new NotificationChannelEntity(request.Id ?? Guid.NewGuid(), request.Channel, request.Settings);
        var persisted = await repository.UpsertNotificationChannelAsync(channel, cancellationToken).ConfigureAwait(false);
        return mapper.Map<NotificationChannelDto>(persisted);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MetricPointDto>> GetMetricsAsync(Guid serverId, string? metricName, DateTimeOffset? from, DateTimeOffset? to, string? interval, CancellationToken cancellationToken)
    {
        var metrics = await repository.ListMetricPointsAsync(serverId, metricName, from, to, interval, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<MetricPointDto>>(metrics.OrderBy(p => p.MeasuredAt).ToList());
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CollectorDto>> GetCollectorsAsync(CancellationToken cancellationToken)
    {
        var collectors = await repository.ListCollectorsAsync(cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<CollectorDto>>(collectors);
    }

    /// <inheritdoc />
    public async Task<CollectorDto> RegisterCollectorAsync(RegisterCollectorRequestDto request, CancellationToken cancellationToken)
    {
        var collector = new Collector(Guid.NewGuid(), request.Name, ParseCollectorType(request.Type));
        collector.ApplyConfiguration(request.Config);
        await repository.UpsertCollectorAsync(collector, cancellationToken).ConfigureAwait(false);
        return mapper.Map<CollectorDto>(collector);
    }

    /// <inheritdoc />
    public async Task RecordCollectorHeartbeatAsync(Guid collectorId, CollectorHeartbeatRequestDto request, CancellationToken cancellationToken)
    {
        var collector = await repository.GetCollectorAsync(collectorId, cancellationToken).ConfigureAwait(false);
        if (collector is null)
        {
            return;
        }

        collector.UpdateLastSeen(request.LastSeen ?? DateTimeOffset.UtcNow);
        await repository.UpsertCollectorAsync(collector, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task RecordCollectorServiceEventsAsync(Guid collectorId, CollectorServiceEventBatchDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Collector {CollectorId} submitted {Count} service events", collectorId, request.Events.Count);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RecordCollectorLogsAsync(Guid collectorId, CollectorLogBatchDto request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Collector {CollectorId} submitted {Count} log entries", collectorId, request.Entries.Count);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<UserDto>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await repository.ListUsersAsync(cancellationToken).ConfigureAwait(false);
        var roles = await repository.ListRolesAsync(cancellationToken).ConfigureAwait(false);

        return [.. users.Select(user => new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Roles = [.. user.RoleAssignments.Select(assignment => roles.FirstOrDefault(role => role.Id == assignment.RoleId)?.Name ?? assignment.RoleId.ToString())]
        })];
    }

    /// <inheritdoc />
    public async Task<UserDto> CreateUserAsync(CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        var user = new User(Guid.NewGuid(), request.Username, request.Email);
        user.UpdateProfile(request.DisplayName, request.Email);
        if (request.RoleIds is not null)
        {
            foreach (var roleId in request.RoleIds)
            {
                user.AddRole(new RoleAssignment(Guid.NewGuid(), user.Id, roleId, null));
            }
        }

        await repository.AddUserAsync(user, cancellationToken).ConfigureAwait(false);

        var dto = mapper.Map<UserDto>(user);
        dto = dto with
        {
            Roles = request.RoleIds?.Select(roleId => roleId.ToString()).ToArray() ?? []
        };
        return dto;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<RoleDto>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await repository.ListRolesAsync(cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<RoleDto>>(roles);
    }

    /// <inheritdoc />
    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequestDto request, CancellationToken cancellationToken)
    {
        var role = new Role(Guid.NewGuid(), request.Name, request.Description);
        await repository.AddRoleAsync(role, cancellationToken).ConfigureAwait(false);
        return mapper.Map<RoleDto>(role);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MaintenanceWindowDto>> GetMaintenanceWindowsAsync(CancellationToken cancellationToken)
    {
        var windows = await repository.ListMaintenanceWindowsAsync(cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<MaintenanceWindowDto>>(windows);
    }

    /// <inheritdoc />
    public async Task<MaintenanceWindowDto> UpsertMaintenanceWindowAsync(UpsertMaintenanceWindowRequestDto request, CancellationToken cancellationToken)
    {
        var window = new MaintenanceWindow(Guid.NewGuid(), request.Name, request.StartTime, request.EndTime, request.Muted);
        if (request.Servers is not null)
        {
            window.SetServerScope(request.Servers);
        }

        await repository.UpsertMaintenanceWindowAsync(window, cancellationToken).ConfigureAwait(false);
        return mapper.Map<MaintenanceWindowDto>(window);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<LogEntryDto>> TailLogsAsync(Guid logFileId, DateTimeOffset? from, CancellationToken cancellationToken)
    {
        var logs = await repository.SearchLogsAsync(null, [logFileId], null, null, from, null, 1, 100, cancellationToken).ConfigureAwait(false);
        return mapper.Map<IReadOnlyCollection<LogEntryDto>>(logs.Items.OrderBy(e => e.Timestamp).ToList());
    }

    private static EnvironmentType ParseEnvironment(string value)
    {
        if (Enum.TryParse<EnvironmentType>(value, true, out var environment))
        {
            return environment;
        }

        return EnvironmentType.Dev;
    }

    private static ServiceDesiredState ParseDesiredState(string value)
    {
        if (Enum.TryParse<ServiceDesiredState>(value, true, out var state))
        {
            return state;
        }

        return ServiceDesiredState.Running;
    }

    private static AlertSeverity ParseAlertSeverity(string value)
    {
        if (Enum.TryParse<AlertSeverity>(value, true, out var severity))
        {
            return severity;
        }

        return AlertSeverity.Warning;
    }

    private static MonitoringEnums ParseCollectorType(string value)
    {
        if (Enum.TryParse<MonitoringEnums>(value, true, out var type))
        {
            return type;
        }

        return MonitoringEnums.Agent;
    }

    private static ServiceSummaryDto BuildServiceSummary(Server server)
    {
        var total = server.Services.Count;
        var running = server.Services.Count(s => s.CurrentState == ServiceState.Running);
        var stopped = server.Services.Count(s => s.CurrentState == ServiceState.Stopped);
        var criticalAlerts = server.Services.Count(s => s.Critical && s.CurrentState != ServiceState.Running);

        return new ServiceSummaryDto
        {
            Total = total,
            Running = running,
            Stopped = stopped,
            CriticalAlerts = criticalAlerts
        };
    }

    private static TaskSummaryDto BuildTaskSummary(Server server)
    {
        var total = server.ScheduledTasks.Count;
        var enabled = server.ScheduledTasks.Count(t => t.IsEnabled);
        var disabled = total - enabled;
        var failed = server.ScheduledTasks.Count(t => string.Equals(t.LastRunResult, TaskRunResult.Failure.ToString(), StringComparison.OrdinalIgnoreCase));

        return new TaskSummaryDto
        {
            Total = total,
            Enabled = enabled,
            Disabled = disabled,
            FailedLastRun = failed
        };
    }

    private static IisSummaryDto BuildIisSummary(Server server)
    {
        var pools = server.AppPools.Count;
        var sites = server.IisSites.Count;
        var stoppedPools = server.AppPools.Count(p => !string.Equals(p.State, "Started", StringComparison.OrdinalIgnoreCase));
        var unhealthySites = server.IisSites.Count(s => !string.Equals(s.Status, "Started", StringComparison.OrdinalIgnoreCase) || (s.LastHttpStatus is >= 400));

        return new IisSummaryDto
        {
            AppPools = pools,
            Sites = sites,
            StoppedAppPools = stoppedPools,
            UnhealthySites = unhealthySites
        };
    }

    private async Task<IReadOnlyCollection<AlertSummaryDto>> LoadRecentAlertsAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var alerts = await repository.ListAlertsAsync(null, null, serverId, null, cancellationToken).ConfigureAwait(false);
        var ordered = alerts.Items.OrderByDescending(a => a.CreatedAt).Take(5).ToList();
        return mapper.Map<IReadOnlyCollection<AlertSummaryDto>>(ordered);
    }

    private static PagedResponseDto<TDestination> MapPaged<TSource, TDestination>(PagedResult<TSource> source, IReadOnlyCollection<TDestination> items)
    {
        return new PagedResponseDto<TDestination>(source.Total, items);
    }
}