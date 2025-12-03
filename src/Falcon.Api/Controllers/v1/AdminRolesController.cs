using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Provides administration endpoints for role management.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/admin/roles")]
public sealed class AdminRolesController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves role definitions available within the platform.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Role collection.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await monitoringService.GetRolesAsync(cancellationToken).ConfigureAwait(false);
        return Ok(roles);
    }

    /// <summary>
    /// Creates a new role definition.
    /// </summary>
    /// <param name="request">Role creation payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created role descriptor.</returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<RoleDto>> CreateRoleAsync(
        [FromBody] CreateRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        var role = await monitoringService.CreateRoleAsync(request, cancellationToken).ConfigureAwait(false);
        return Created(string.Empty, role);
    }
}