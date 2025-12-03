using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Exposes authentication-related endpoints for retrieving information about the current user.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves the authenticated user profile including assigned roles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Authenticated user profile.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var profile = await monitoringService.GetCurrentUserProfileAsync(cancellationToken).ConfigureAwait(false);
        return Ok(profile);
    }
}