using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Handles notification channel configuration endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/notification-channels")]
public sealed class NotificationChannelsController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves configured notification channels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Notification channels.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<NotificationChannelDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<NotificationChannelDto>>> GetChannelsAsync(CancellationToken cancellationToken)
    {
        var channels = await monitoringService.GetNotificationChannelsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(channels);
    }

    /// <summary>
    /// Creates or updates a notification channel definition.
    /// </summary>
    /// <param name="request">Notification channel payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Persisted channel descriptor.</returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(NotificationChannelDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<NotificationChannelDto>> UpsertChannelAsync(
        [FromBody] UpsertNotificationChannelRequestDto request,
        CancellationToken cancellationToken)
    {
        var channel = await monitoringService.UpsertNotificationChannelAsync(request, cancellationToken).ConfigureAwait(false);
        return Created(string.Empty, channel);
    }
}