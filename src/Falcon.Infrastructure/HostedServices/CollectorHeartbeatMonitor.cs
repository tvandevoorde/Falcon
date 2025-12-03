using Falcon.Domain.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Falcon.Infrastructure.HostedServices;

/// <summary>
/// Periodically inspects collector heartbeats to surface stale agents.
/// </summary>
public sealed class CollectorHeartbeatMonitor(
    IMonitoringRepository repository,
    ILogger<CollectorHeartbeatMonitor> logger) : BackgroundService
{
    private readonly IMonitoringRepository repository = repository;
    private readonly ILogger<CollectorHeartbeatMonitor> logger = logger;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Collector heartbeat monitor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var collectors = await repository.ListCollectorsAsync(stoppingToken).ConfigureAwait(false);
                var cutoff = DateTimeOffset.UtcNow.AddMinutes(-5);

                foreach (var collector in collectors)
                {
                    if (collector.LastSeen is not null && collector.LastSeen < cutoff)
                    {
                        logger.LogWarning(
                            "Collector {CollectorId} ({CollectorName}) is stale. Last seen at {LastSeen}",
                            collector.Id,
                            collector.Name,
                            collector.LastSeen);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown.
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while monitoring collector heartbeats");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
        }

        logger.LogInformation("Collector heartbeat monitor stopping");
    }
}