using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Common;
using Falcon.Application.Contracts.Iis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Provides endpoints related to IIS application pools.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/iis/app-pools")]
public sealed class IisAppPoolsController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Queues a recycle operation for the specified IIS application pool.
    /// </summary>
    /// <param name="appPoolId">Application pool identifier.</param>
    /// <param name="request">Recycle request payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Acknowledgement of the recycle request.</returns>
    [HttpPost("{appPoolId:guid}/actions/recycle")]
    [Authorize(Policy = "RequireOperator")]
    [ProducesResponseType(typeof(ActionAcknowledgementDto), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<ActionAcknowledgementDto>> RecycleAppPoolAsync(
        Guid appPoolId,
        [FromBody] RecycleAppPoolRequestDto request,
        CancellationToken cancellationToken)
    {
        await monitoringService.RecycleAppPoolAsync(appPoolId, request, cancellationToken).ConfigureAwait(false);

        var response = new ActionAcknowledgementDto
        {
            ActionId = Guid.NewGuid(),
            ScheduledAt = DateTimeOffset.UtcNow,
            Status = "queued"
        };

        return Accepted(response);
    }
}