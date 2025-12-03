using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Manages log pattern configuration resources.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/log-patterns")]
public sealed class LogPatternsController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves configured log patterns.
    /// </summary>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of log patterns.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<LogPatternDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<LogPatternDto>>> GetLogPatternsAsync(CancellationToken cancellationToken)
    {
        var patterns = await monitoringService.GetLogPatternsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(patterns);
    }

    /// <summary>
    /// Creates or updates a log pattern definition.
    /// </summary>
    /// <param name="request">Log pattern payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Created or updated pattern.</returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(LogPatternDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<LogPatternDto>> UpsertLogPatternAsync(
        [FromBody] UpsertLogPatternRequestDto request,
        CancellationToken cancellationToken)
    {
        var pattern = await monitoringService.UpsertLogPatternAsync(request, cancellationToken).ConfigureAwait(false);
        return Created(string.Empty, pattern);
    }
}