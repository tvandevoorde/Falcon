using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Maintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Manages maintenance window definitions for suppressing alert noise.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/maintenance-windows")]
public sealed class MaintenanceWindowsController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves maintenance windows currently defined.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Maintenance window collection.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<MaintenanceWindowDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<MaintenanceWindowDto>>> GetMaintenanceWindowsAsync(CancellationToken cancellationToken)
    {
        var windows = await monitoringService.GetMaintenanceWindowsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(windows);
    }

    /// <summary>
    /// Creates or updates a maintenance window definition.
    /// </summary>
    /// <param name="request">Maintenance window payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Persisted maintenance window descriptor.</returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(MaintenanceWindowDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<MaintenanceWindowDto>> UpsertMaintenanceWindowAsync(
        [FromBody] UpsertMaintenanceWindowRequestDto request,
        CancellationToken cancellationToken)
    {
        var window = await monitoringService.UpsertMaintenanceWindowAsync(request, cancellationToken).ConfigureAwait(false);
        return Created(string.Empty, window);
    }
}