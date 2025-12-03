using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Alerts;
using Falcon.Application.Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Provides alert monitoring and lifecycle management endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/alerts")]
public sealed class AlertsController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves alerts applying optional query filters.
    /// </summary>
    /// <param name="status">Optional status filter (open, acknowledged, closed).</param>
    /// <param name="severity">Optional severity filter (info, warning, critical).</param>
    /// <param name="serverId">Optional server identifier filter.</param>
    /// <param name="sourceType">Optional source type filter.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Paged alert list.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<AlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<AlertDto>>> GetAlertsAsync(
        [FromQuery] string? status,
        [FromQuery] string? severity,
        [FromQuery] Guid? serverId,
        [FromQuery] string? sourceType,
        CancellationToken cancellationToken)
    {
        var alerts = await monitoringService.GetAlertsAsync(status, severity, serverId, sourceType, cancellationToken).ConfigureAwait(false);
        return Ok(alerts);
    }

    /// <summary>
    /// Creates a manual alert entry.
    /// </summary>
    /// <param name="request">Alert creation payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Created alert DTO.</returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AlertDto>> CreateAlertAsync(
        [FromBody] CreateAlertRequestDto request,
        CancellationToken cancellationToken)
    {
        var alert = await monitoringService.CreateAlertAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAlertByIdAsync), new { alertId = alert.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0" }, alert);
    }

    /// <summary>
    /// Retrieves an alert by identifier.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Alert DTO or 404.</returns>
    [HttpGet("{alertId:guid}")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertDto>> GetAlertByIdAsync(Guid alertId, CancellationToken cancellationToken)
    {
        var alert = await monitoringService.GetAlertAsync(alertId, cancellationToken).ConfigureAwait(false);
        if (alert is null)
        {
            return NotFound();
        }

        return Ok(alert);
    }

    /// <summary>
    /// Acknowledges an alert with an optional operator comment.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="request">Acknowledgement payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Updated alert DTO.</returns>
    [HttpPost("{alertId:guid}/ack")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AlertDto>> AcknowledgeAlertAsync(
        Guid alertId,
        [FromBody] AcknowledgeAlertRequestDto request,
        CancellationToken cancellationToken)
    {
        await monitoringService.AcknowledgeAlertAsync(alertId, request, cancellationToken).ConfigureAwait(false);
        var alert = await monitoringService.GetAlertAsync(alertId, cancellationToken).ConfigureAwait(false);
        return alert is null ? NotFound() : Ok(alert);
    }

    /// <summary>
    /// Closes an alert with an optional resolution note.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="request">Closure payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Updated alert DTO.</returns>
    [HttpPost("{alertId:guid}/close")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AlertDto>> CloseAlertAsync(
        Guid alertId,
        [FromBody] CloseAlertRequestDto request,
        CancellationToken cancellationToken)
    {
        await monitoringService.CloseAlertAsync(alertId, request, cancellationToken).ConfigureAwait(false);
        var alert = await monitoringService.GetAlertAsync(alertId, cancellationToken).ConfigureAwait(false);
        return alert is null ? NotFound() : Ok(alert);
    }

    /// <summary>
    /// Retrieves notifications that were sent for the specified alert.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of notifications.</returns>
    [HttpGet("{alertId:guid}/notifications")]
    [ProducesResponseType(typeof(IReadOnlyCollection<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<NotificationDto>>> GetAlertNotificationsAsync(
        Guid alertId,
        CancellationToken cancellationToken)
    {
        var notifications = await monitoringService.GetAlertNotificationsAsync(alertId, cancellationToken).ConfigureAwait(false);
        return Ok(notifications);
    }

    /// <summary>
    /// Queues a resend attempt for an alert notification.
    /// </summary>
    /// <param name="alertId">Alert identifier.</param>
    /// <param name="notificationId">Notification identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Accepted status.</returns>
    [HttpPost("{alertId:guid}/notifications/{notificationId:guid}/resend")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ResendNotificationAsync(Guid alertId, Guid notificationId, CancellationToken cancellationToken)
    {
        await monitoringService.ResendNotificationAsync(alertId, notificationId, cancellationToken).ConfigureAwait(false);
        return Accepted();
    }
}