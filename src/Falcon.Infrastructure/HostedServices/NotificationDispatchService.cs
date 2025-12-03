using Falcon.Domain.Enumerations;
using Falcon.Domain.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Falcon.Infrastructure.HostedServices;

/// <summary>
/// Simulates notification dispatch cycle by scanning pending notifications.
/// </summary>
public sealed class NotificationDispatchService(
    IMonitoringRepository repository,
    ILogger<NotificationDispatchService> logger) : BackgroundService
{
    private readonly IMonitoringRepository repository = repository;
    private readonly ILogger<NotificationDispatchService> logger = logger;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Notification dispatcher started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var alerts = await repository.ListAlertsAsync(null, null, null, null, stoppingToken).ConfigureAwait(false);

                foreach (var alert in alerts.Items)
                {
                    var pendingNotifications = alert.Notifications
                        .Where(notification => notification.Status == NotificationStatus.Pending)
                        .ToArray();

                    if (pendingNotifications.Length == 0)
                    {
                        continue;
                    }

                    foreach (var notification in pendingNotifications)
                    {
                        logger.LogInformation(
                            "Dispatching notification {NotificationId} for alert {AlertId} via {Channel}",
                            notification.Id,
                            alert.Id,
                            notification.Channel.Channel);

                        // Placeholder for integration with SMTP, Teams, Slack, etc.
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while dispatching notifications");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ConfigureAwait(false);
        }

        logger.LogInformation("Notification dispatcher stopping");
    }
}