using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Provides administration endpoints for managing users.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/admin/users")]
public sealed class AdminUsersController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves application users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Users collection.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<UserDto>>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await monitoringService.GetUsersAsync(cancellationToken).ConfigureAwait(false);
        return Ok(users);
    }

    /// <summary>
    /// Creates a new application user.
    /// </summary>
    /// <param name="request">User creation payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created user descriptor.</returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDto>> CreateUserAsync(
        [FromBody] CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var user = await monitoringService.CreateUserAsync(request, cancellationToken).ConfigureAwait(false);
        return Created(string.Empty, user);
    }
}