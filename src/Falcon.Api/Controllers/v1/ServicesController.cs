using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Common;
using Falcon.Application.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Handles operations related to monitored Windows services.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/services")]
public sealed class ServicesController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves a monitored service by identifier.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Service DTO or 404.</returns>
    [HttpGet("{serviceId:guid}")]
    [ProducesResponseType(typeof(MonitoredServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MonitoredServiceDto>> GetServiceAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var service = await monitoringService.GetServiceAsync(serviceId, cancellationToken).ConfigureAwait(false);
        if (service is null)
        {
            return NotFound();
        }

        return Ok(service);
    }

    /// <summary>
    /// Removes a monitored service from the platform.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>No content status.</returns>
    [HttpDelete("{serviceId:guid}")]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteServiceAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        await monitoringService.DeleteServiceAsync(serviceId, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Retrieves the recent state change events for a monitored service.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="limit">Maximum number of events to return.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of service events.</returns>
    [HttpGet("{serviceId:guid}/events")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ServiceEventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ServiceEventDto>>> GetServiceEventsAsync(
        Guid serviceId,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var events = await monitoringService.GetServiceEventsAsync(serviceId, limit, cancellationToken).ConfigureAwait(false);
        return Ok(events);
    }

    /// <summary>
    /// Queues a restart operation for the specified service.
    /// </summary>
    /// <param name="serviceId">Service identifier.</param>
    /// <param name="request">Restart request payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Acknowledgement with action metadata.</returns>
    [HttpPost("{serviceId:guid}/actions/restart")]
    [Authorize(Policy = "RequireOperator")]
    [ProducesResponseType(typeof(ActionAcknowledgementDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActionAcknowledgementDto>> RestartServiceAsync(
        Guid serviceId,
        [FromBody] RestartServiceRequestDto request,
        CancellationToken cancellationToken)
    {
        var service = await monitoringService.GetServiceAsync(serviceId, cancellationToken).ConfigureAwait(false);
        if (service is null)
        {
            return NotFound();
        }

        await monitoringService.TriggerServiceRestartAsync(serviceId, request, cancellationToken).ConfigureAwait(false);

        var response = new ActionAcknowledgementDto
        {
            ActionId = Guid.NewGuid(),
            ScheduledAt = DateTimeOffset.UtcNow,
            Status = "queued"
        };

        return Accepted(response);
    }
}