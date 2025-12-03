using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Common;
using Falcon.Application.Contracts.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Exposes log search and streaming endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/logs")]
public sealed class LogsController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Executes a log search across configured servers and log files.
    /// </summary>
    /// <param name="request">Log search payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Paged log search results.</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PagedResponseDto<LogEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponseDto<LogEntryDto>>> SearchLogsAsync(
        [FromBody] LogSearchApiRequest request,
        CancellationToken cancellationToken)
    {
        var dto = new LogSearchRequestDto
        {
            ServerIds = request.ServerIds,
            LogFileIds = request.LogFileIds,
            Severities = request.Severity,
            Query = request.Query,
            From = request.From,
            To = request.To,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var result = await monitoringService.SearchLogsAsync(dto, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Returns a chronological tail of log entries for streaming scenarios.
    /// </summary>
    /// <param name="logFileId">Log file identifier.</param>
    /// <param name="from">Optional starting timestamp.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of log entries ordered by timestamp.</returns>
    [HttpGet("stream")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LogEntryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<LogEntryDto>>> StreamLogsAsync(
        [FromQuery][Required] Guid logFileId,
        [FromQuery] DateTimeOffset? from,
        CancellationToken cancellationToken)
    {
        var entries = await monitoringService.TailLogsAsync(logFileId, from, cancellationToken).ConfigureAwait(false);
        return Ok(entries);
    }

    /// <summary>
    /// Represents the payload accepted by the log search endpoint.
    /// </summary>
    public sealed class LogSearchApiRequest
    {
        public IReadOnlyCollection<Guid>? ServerIds { get; init; }

        public IReadOnlyCollection<Guid>? LogFileIds { get; init; }

        public IReadOnlyCollection<string>? Severity { get; init; }

        public string? Query { get; init; }

        public DateTimeOffset? From { get; init; }

        public DateTimeOffset? To { get; init; }

        public int Page { get; init; } = 1;

        public int PageSize { get; init; } = 50;
    }
}