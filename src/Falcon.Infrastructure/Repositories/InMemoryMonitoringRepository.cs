using Falcon.Domain.Entities;
using Falcon.Domain.Enumerations;
using Falcon.Domain.Repositories;
using Falcon.Domain.ValueObjects;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using NotificationChannelEntity = Falcon.Domain.Entities.NotificationChannel;

namespace Falcon.Infrastructure.Repositories;

/// <summary>
/// Provides an in-memory implementation of <see cref="IMonitoringRepository"/> for development and testing.
/// </summary>
public sealed class InMemoryMonitoringRepository : IMonitoringRepository
{
    private readonly ConcurrentDictionary<Guid, Server> servers = new();
    private readonly ConcurrentDictionary<Guid, MonitoredService> services = new();
    private readonly ConcurrentDictionary<Guid, ServiceEvent> serviceEvents = new();
    private readonly ConcurrentDictionary<Guid, ScheduledTask> tasks = new();
    private readonly ConcurrentDictionary<Guid, TaskRun> taskRuns = new();
    private readonly ConcurrentDictionary<Guid, AppPool> appPools = new();
    private readonly ConcurrentDictionary<Guid, IisSite> iisSites = new();
    private readonly ConcurrentDictionary<Guid, LogFile> logFiles = new();
    private readonly ConcurrentDictionary<Guid, LogEntry> logEntries = new();
    private readonly ConcurrentDictionary<long, MetricPoint> metricPoints = new();
    private readonly ConcurrentDictionary<Guid, Alert> alerts = new();
    private readonly ConcurrentDictionary<Guid, Notification> notifications = new();
    private readonly ConcurrentDictionary<Guid, LogPattern> logPatterns = new();
    private readonly ConcurrentDictionary<Guid, Collector> collectors = new();
    private readonly ConcurrentDictionary<Guid, User> users = new();
    private readonly ConcurrentDictionary<Guid, Role> roles = new();
    private readonly ConcurrentDictionary<Guid, MaintenanceWindow> maintenanceWindows = new();
    private readonly ConcurrentDictionary<Guid, NotificationChannelEntity> channels = new();

    private readonly Lock seedLock = new();
    private bool seeded;

    public InMemoryMonitoringRepository()
    {
        EnsureSeeded();
    }

    public Task<PagedResult<Server>> ListServersAsync(
        string? environment,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = servers.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(environment) && Enum.TryParse(environment, true, out EnvironmentType env))
        {
            query = query.Where(server => server.Environment == env);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(server =>
                server.Hostname.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (server.DisplayName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var ordered = query.OrderBy(server => server.Hostname).ToList();
        var paged = ordered.Skip((Math.Max(page, 1) - 1) * Math.Max(pageSize, 1)).Take(Math.Max(pageSize, 1)).ToList();
        return Task.FromResult(new PagedResult<Server>(ordered.Count, paged));
    }

    public Task<Server?> GetServerAsync(Guid serverId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        servers.TryGetValue(serverId, out var server);
        return Task.FromResult(server);
    }

    public Task<Server> AddServerAsync(Server server, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        servers[server.Id] = server;
        return Task.FromResult(server);
    }

    public Task UpdateServerAsync(Server server, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        servers[server.Id] = server;
        return Task.CompletedTask;
    }

    public Task DeleteServerAsync(Guid serverId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        servers.TryRemove(serverId, out _);
        foreach (var service in services.Values.Where(svc => svc.ServerId == serverId).ToList())
        {
            services.TryRemove(service.Id, out _);
        }

        foreach (var task in tasks.Values.Where(t => t.ServerId == serverId).ToList())
        {
            tasks.TryRemove(task.Id, out _);
        }

        foreach (var pool in appPools.Values.Where(p => p.ServerId == serverId).ToList())
        {
            appPools.TryRemove(pool.Id, out _);
        }

        foreach (var site in iisSites.Values.Where(s => s.ServerId == serverId).ToList())
        {
            iisSites.TryRemove(site.Id, out _);
        }

        foreach (var file in logFiles.Values.Where(f => f.ServerId == serverId).ToList())
        {
            logFiles.TryRemove(file.Id, out _);
        }

        foreach (var entry in logEntries.Values.Where(e => e.ServerId == serverId).ToList())
        {
            logEntries.TryRemove(entry.Id, out _);
        }

        foreach (var metric in metricPoints.Values.Where(m => m.ServerId == serverId).ToList())
        {
            metricPoints.TryRemove(metric.Id, out _);
        }

        return Task.CompletedTask;
    }

    public Task<MonitoredService?> GetServiceAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        services.TryGetValue(serviceId, out var service);
        return Task.FromResult(service);
    }

    public Task<IReadOnlyCollection<MonitoredService>> ListServicesAsync(Guid serverId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = services.Values.Where(service => service.ServerId == serverId).OrderBy(service => service.ServiceName).ToList();
        return Task.FromResult((IReadOnlyCollection<MonitoredService>)result);
    }

    public Task AddServiceAsync(MonitoredService service, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        services[service.Id] = service;

        if (servers.TryGetValue(service.ServerId, out var server))
        {
            server.AddService(service);
        }

        return Task.CompletedTask;
    }

    public Task RemoveServiceAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (services.TryRemove(serviceId, out var service) && servers.TryGetValue(service.ServerId, out var server))
        {
            server.RemoveService(serviceId);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<ServiceEvent>> ListServiceEventsAsync(Guid serviceId, int limit, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = serviceEvents.Values
            .Where(evt => evt.ServiceId == serviceId)
            .OrderByDescending(evt => evt.EventTime)
            .Take(Math.Max(limit, 1))
            .ToList();

        return Task.FromResult((IReadOnlyCollection<ServiceEvent>)result);
    }

    public Task<IReadOnlyCollection<ScheduledTask>> ListScheduledTasksAsync(Guid serverId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = tasks.Values.Where(task => task.ServerId == serverId).OrderBy(task => task.TaskName).ToList();
        return Task.FromResult((IReadOnlyCollection<ScheduledTask>)result);
    }

    public Task<ScheduledTask?> GetScheduledTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        tasks.TryGetValue(taskId, out var task);
        return Task.FromResult(task);
    }

    public Task AddScheduledTaskAsync(ScheduledTask task, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        tasks[task.Id] = task;

        if (servers.TryGetValue(task.ServerId, out var server))
        {
            server.AddScheduledTask(task);
        }

        return Task.CompletedTask;
    }

    public Task RemoveScheduledTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (tasks.TryRemove(taskId, out var task) && servers.TryGetValue(task.ServerId, out var server))
        {
            server.RemoveScheduledTask(taskId);
        }

        foreach (var run in taskRuns.Values.Where(run => run.ScheduledTaskId == taskId).ToList())
        {
            taskRuns.TryRemove(run.Id, out _);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<TaskRun>> ListTaskRunsAsync(Guid taskId, int limit, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = taskRuns.Values
            .Where(run => run.ScheduledTaskId == taskId)
            .OrderByDescending(run => run.StartTime ?? run.EndTime)
            .Take(Math.Max(limit, 1))
            .ToList();

        return Task.FromResult((IReadOnlyCollection<TaskRun>)result);
    }

    public Task<IReadOnlyCollection<AppPool>> ListAppPoolsAsync(Guid serverId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = appPools.Values.Where(pool => pool.ServerId == serverId).OrderBy(pool => pool.Name).ToList();
        return Task.FromResult((IReadOnlyCollection<AppPool>)result);
    }

    public Task<AppPool?> GetAppPoolAsync(Guid appPoolId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        appPools.TryGetValue(appPoolId, out var appPool);
        return Task.FromResult(appPool);
    }

    public Task<IReadOnlyCollection<IisSite>> ListIisSitesAsync(Guid serverId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = iisSites.Values.Where(site => site.ServerId == serverId).OrderBy(site => site.Name).ToList();
        return Task.FromResult((IReadOnlyCollection<IisSite>)result);
    }

    public Task<IReadOnlyCollection<LogFile>> ListLogFilesAsync(Guid serverId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = logFiles.Values.Where(file => file.ServerId == serverId).OrderBy(file => file.Path).ToList();
        return Task.FromResult((IReadOnlyCollection<LogFile>)result);
    }

    public Task<IReadOnlyCollection<MetricPoint>> ListMetricPointsAsync(
        Guid serverId,
        string? metricName,
        DateTimeOffset? from,
        DateTimeOffset? to,
        string? interval,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var query = metricPoints.Values.Where(point => point.ServerId == serverId);

        if (!string.IsNullOrWhiteSpace(metricName))
        {
            query = query.Where(point => point.MetricName.Equals(metricName, StringComparison.OrdinalIgnoreCase));
        }

        if (from is not null)
        {
            query = query.Where(point => point.MeasuredAt >= from);
        }

        if (to is not null)
        {
            query = query.Where(point => point.MeasuredAt <= to);
        }

        var result = query.OrderBy(point => point.MeasuredAt).ToList();
        return Task.FromResult((IReadOnlyCollection<MetricPoint>)result);
    }

    public Task<PagedResult<LogEntry>> SearchLogsAsync(
        IReadOnlyCollection<Guid>? serverIds,
        IReadOnlyCollection<Guid>? logFileIds,
        IReadOnlyCollection<string>? severities,
        string? text,
        DateTimeOffset? from,
        DateTimeOffset? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = logEntries.Values.AsEnumerable();

        if (serverIds is not null && serverIds.Count > 0)
        {
            var set = serverIds.ToImmutableHashSet();
            query = query.Where(entry => set.Contains(entry.ServerId));
        }

        if (logFileIds is not null && logFileIds.Count > 0)
        {
            var set = logFileIds.ToImmutableHashSet();
            query = query.Where(entry => set.Contains(entry.LogFileId));
        }

        if (severities is not null && severities.Count > 0)
        {
            var severitySet = severities.Select(item => item.ToUpperInvariant()).ToImmutableHashSet();
            query = query.Where(entry => severitySet.Contains(entry.Severity.ToUpperInvariant()));
        }

        if (!string.IsNullOrWhiteSpace(text))
        {
            query = query.Where(entry => entry.Message.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        if (from is not null)
        {
            query = query.Where(entry => entry.Timestamp >= from);
        }

        if (to is not null)
        {
            query = query.Where(entry => entry.Timestamp <= to);
        }

        var ordered = query.OrderByDescending(entry => entry.Timestamp).ToList();
        var paged = ordered.Skip((Math.Max(page, 1) - 1) * Math.Max(pageSize, 1)).Take(Math.Max(pageSize, 1)).ToList();

        return Task.FromResult(new PagedResult<LogEntry>(ordered.Count, paged));
    }

    public Task<IReadOnlyCollection<LogPattern>> ListLogPatternsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = logPatterns.Values.OrderBy(pattern => pattern.Name).ToList();
        return Task.FromResult((IReadOnlyCollection<LogPattern>)result);
    }

    public Task UpsertLogPatternAsync(LogPattern pattern, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        logPatterns[pattern.Id] = pattern;
        return Task.CompletedTask;
    }

    public Task<PagedResult<Alert>> ListAlertsAsync(
        string? status,
        string? severity,
        Guid? serverId,
        string? sourceType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var query = alerts.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(alert => alert.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            query = query.Where(alert => alert.Severity.ToString().Equals(severity, StringComparison.OrdinalIgnoreCase));
        }

        if (serverId is not null)
        {
            query = query.Where(alert => alert.ServerId == serverId);
        }

        if (!string.IsNullOrWhiteSpace(sourceType))
        {
            query = query.Where(alert => alert.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase));
        }

        var ordered = query.OrderByDescending(alert => alert.CreatedAt).ToList();
        return Task.FromResult(new PagedResult<Alert>(ordered.Count, ordered));
    }

    public Task<Alert?> GetAlertAsync(Guid alertId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        alerts.TryGetValue(alertId, out var alert);
        return Task.FromResult(alert);
    }

    public Task AddAlertAsync(Alert alert, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        alerts[alert.Id] = alert;
        return Task.CompletedTask;
    }

    public Task UpdateAlertAsync(Alert alert, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        alerts[alert.Id] = alert;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Notification>> ListNotificationsAsync(Guid alertId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = notifications.Values.Where(notification => notification.AlertId == alertId)
            .OrderByDescending(notification => notification.LastAttempt)
            .ToList();
        return Task.FromResult((IReadOnlyCollection<Notification>)result);
    }

    public Task<IReadOnlyCollection<NotificationChannelEntity>> ListNotificationChannelsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = channels.Values.OrderBy(channel => channel.Channel).ToList();
        return Task.FromResult((IReadOnlyCollection<NotificationChannelEntity>)result);
    }

    public Task<NotificationChannelEntity> UpsertNotificationChannelAsync(NotificationChannelEntity channel, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        channels[channel.Id] = channel;
        return Task.FromResult(channel);
    }

    public Task<IReadOnlyCollection<Collector>> ListCollectorsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = collectors.Values.OrderBy(collector => collector.Name).ToList();
        return Task.FromResult((IReadOnlyCollection<Collector>)result);
    }

    public Task<Collector?> GetCollectorAsync(Guid collectorId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        collectors.TryGetValue(collectorId, out var collector);
        return Task.FromResult(collector);
    }

    public Task UpsertCollectorAsync(Collector collector, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        collectors[collector.Id] = collector;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<User>> ListUsersAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = users.Values.OrderBy(user => user.Username).ToList();
        return Task.FromResult((IReadOnlyCollection<User>)result);
    }

    public Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Role>> ListRolesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = roles.Values.OrderBy(role => role.Name).ToList();
        return Task.FromResult((IReadOnlyCollection<Role>)result);
    }

    public Task AddRoleAsync(Role role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        roles[role.Id] = role;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<MaintenanceWindow>> ListMaintenanceWindowsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = maintenanceWindows.Values.OrderBy(window => window.StartTime).ToList();
        return Task.FromResult((IReadOnlyCollection<MaintenanceWindow>)result);
    }

    public Task UpsertMaintenanceWindowAsync(MaintenanceWindow maintenanceWindow, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        maintenanceWindows[maintenanceWindow.Id] = maintenanceWindow;
        return Task.CompletedTask;
    }

    private void EnsureSeeded()
    {
        if (seeded)
        {
            return;
        }

        lock (seedLock)
        {
            if (seeded)
            {
                return;
            }

            Seed();
            seeded = true;
        }
    }

    private void Seed()
    {
        var prodServer = new Server(Guid.Parse("f0b7a9a0-4f5b-4a64-8a68-9a6aaf8c8d01"), "falcon-app-01", EnvironmentType.Prod, ServerStatus.Healthy);
        prodServer.UpdateMetadata(
            "Falcon App 01",
            EnvironmentType.Prod,
            "10.0.10.15",
            "Windows Server 2025",
            ["production", "app"]);
        prodServer.ApplyHeartbeat(ServerStatus.Healthy, DateTimeOffset.UtcNow.AddMinutes(-1), 24.5, 58.1);

        var webService = new MonitoredService(
            Guid.Parse("56a9d873-18a0-4d04-8f3b-8e20538f01dc"),
            prodServer.Id,
            "Falcon.Scheduler",
            ServiceDesiredState.Running,
            ServiceState.Running,
            true)
        {
            DisplayName = "Falcon Scheduler"
        };
        var serviceEvent = new ServiceEvent(
            Guid.Parse("4bb3b541-3b6f-4c6b-902a-871ae8246ba5"),
            webService.Id,
            ServiceState.Running.ToString(),
            "Service started successfully",
            DateTimeOffset.UtcNow.AddMinutes(-5));
        serviceEvents[serviceEvent.Id] = serviceEvent;
        webService.AddEvent(serviceEvent);

        prodServer.AddService(webService);
        services[webService.Id] = webService;

        var task = new ScheduledTask(
            Guid.Parse("a2c0fced-1b7c-47f2-83b2-4a3ebc5a8d88"),
            prodServer.Id,
            "Falcon Inventory Sweep",
            true);
        task.UpdateSchedule("Every 15 minutes", DateTimeOffset.UtcNow.AddMinutes(10));
        var taskRun = new TaskRun(
            Guid.Parse("b9d58bcb-42c0-4ec6-b606-6a181dadc6c1"),
            task.Id,
            DateTimeOffset.UtcNow.AddMinutes(-30),
            DateTimeOffset.UtcNow.AddMinutes(-28),
            TaskRunResult.Success,
            0,
            "Scan completed");
        task.RecordRun(taskRun);
        tasks[task.Id] = task;
        taskRuns[taskRun.Id] = taskRun;
        prodServer.AddScheduledTask(task);

        var pool = new AppPool(Guid.Parse("c6b3f631-44cb-4f23-ab1f-9cc38c33df65"), prodServer.Id, "FalconAppPool", "Started");
        pool.UpdateState("Started", DateTimeOffset.UtcNow.AddHours(-2));
        appPools[pool.Id] = pool;
        prodServer.AddAppPool(pool);

        var site = new IisSite(Guid.Parse("0a6f5555-1b07-4c56-b3aa-d99c7a595fe5"), prodServer.Id, "Falcon Portal", "Started");
        site.UpdateMetadata(
            "Started",
            new Dictionary<string, string> { ["https"] = "*:443:falcon.monitoring.local" },
            "https://falcon.monitoring.local/health",
            200,
            DateTimeOffset.UtcNow.AddMinutes(-2));
        iisSites[site.Id] = site;
        prodServer.AddIisSite(site);

        var logFile = new LogFile(Guid.Parse("d4a3feec-5df0-46f0-bd1a-31f9fab63ba1"), prodServer.Id, "C:/logs/falcon-app.log", "json");
        var logEntry = new LogEntry(
            Guid.Parse("df2106f3-8a8b-4442-9c17-37f9f196a1f4"),
            logFile.Id,
            prodServer.Id,
            DateTimeOffset.UtcNow.AddMinutes(-3),
            "info",
            "Scheduled sweep completed",
            new Dictionary<string, object> { ["durationMs"] = 1200 });
        logEntries[logEntry.Id] = logEntry;
        logFile.AddEntry(logEntry);
        logFiles[logFile.Id] = logFile;
        prodServer.AddLogFile(logFile);

        var cpuMetric = new MetricPoint(1, prodServer.Id, "cpu", 24.5, DateTimeOffset.UtcNow.AddMinutes(-1));
        metricPoints[cpuMetric.Id] = cpuMetric;
        prodServer.AddMetricPoint(cpuMetric);

        servers[prodServer.Id] = prodServer;

        var alert = new Alert(
            Guid.Parse("0b4c46b6-5a85-46a3-a52c-b2df2d8f4f61"),
            prodServer.Id,
            "service",
            webService.Id,
            "service_down",
            AlertSeverity.Warning,
            AlertStatus.Open.ToString().ToLowerInvariant(),
            "Service detected restart",
            DateTimeOffset.UtcNow.AddMinutes(-15));
        alerts[alert.Id] = alert;

        var notificationChannel = new NotificationChannelEntity(Guid.Parse("1de0b4e0-7b31-4ec9-8764-262de5f1f027"), "email", new Dictionary<string, object> { ["from"] = "falcon@monitoring.local" });
        channels[notificationChannel.Id] = notificationChannel;

        var notification = new Notification(
            Guid.Parse("c9bf0665-0e0d-47cf-9ec8-6df6b48d8761"),
            alert.Id,
            notificationChannel,
            "oncall@monitoring.local",
            NotificationStatus.Sent,
            1,
            DateTimeOffset.UtcNow.AddMinutes(-14),
            new Dictionary<string, object> { ["medium"] = "email" });
        notifications[notification.Id] = notification;
        alert.AddNotification(notification);

        var collector = new Collector(Guid.Parse("ecb04a80-8624-4ffd-8894-62465b3686d7"), "Prod Agent", MonitoringEnums.Agent);
        collector.UpdateLastSeen(DateTimeOffset.UtcNow.AddMinutes(-1));
        collectors[collector.Id] = collector;

        var adminRole = new Role(Guid.Parse("83165704-1a7b-4f4b-8de5-0213a1ddf4ad"), "Administrator", "Full platform access");
        roles[adminRole.Id] = adminRole;

        var operatorRole = new Role(Guid.Parse("f8a64b52-82ae-4e88-961a-0a4dee1d469b"), "Operator", "Day-to-day operations");
        roles[operatorRole.Id] = operatorRole;

        var adminUser = new User(Guid.Parse("5e7ee4f1-04bb-4c41-b730-efd99b9dbd32"), "falcon.admin", "falcon.admin@monitoring.local");
        adminUser.UpdateProfile("Falcon Administrator", "falcon.admin@monitoring.local");
        adminUser.AddRole(new RoleAssignment(Guid.NewGuid(), adminUser.Id, adminRole.Id, null));
        users[adminUser.Id] = adminUser;

        var maintenance = new MaintenanceWindow(
            Guid.Parse("db7f5e2b-8a78-43d3-9f6e-1d820c47d2fb"),
            "Monthly Patch", DateTimeOffset.UtcNow.AddDays(7), DateTimeOffset.UtcNow.AddDays(7).AddHours(2), true);
        maintenance.SetServerScope([prodServer.Id]);
        maintenanceWindows[maintenance.Id] = maintenance;

        var logPattern = new LogPattern(Guid.Parse("8f774935-602f-4ce1-802f-587a8df238c5"), "Unhandled Exception", "Exception:", "critical", true);
        logPatterns[logPattern.Id] = logPattern;
    }
}