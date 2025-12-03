using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Collectors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Manages collector registration and ingestion endpoints.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/collectors")]
public sealed class CollectorsController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Lists collectors registered with the platform.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collectors collection.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CollectorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CollectorDto>>> GetCollectorsAsync(CancellationToken cancellationToken)
    {
        var collectors = await monitoringService.GetCollectorsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(collectors);
    }

    /// <summary>
    /// Registers a collector with the monitoring platform.
    /// </summary>
    /// <param name="request">Collector registration payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created collector descriptor.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CollectorDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CollectorDto>> RegisterCollectorAsync(
        [FromBody] RegisterCollectorRequestDto request,
        CancellationToken cancellationToken)
    {
        var collector = await monitoringService.RegisterCollectorAsync(request, cancellationToken).ConfigureAwait(false);
        return Created(string.Empty, collector);
    }

    /// <summary>
    /// Records a collector heartbeat.
    /// </summary>
    /// <param name="collectorId">Collector identifier.</param>
    /// <param name="request">Heartbeat payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Accepted status.</returns>
    [HttpPost("{collectorId:guid}/heartbeat")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RecordHeartbeatAsync(
        Guid collectorId,
        [FromBody] CollectorHeartbeatApiRequest request,
        CancellationToken cancellationToken)
    {
        var dto = new CollectorHeartbeatRequestDto
        {
            Health = request.Health,
            LastSeen = request.LastSeen
        };

        await monitoringService.RecordCollectorHeartbeatAsync(collectorId, dto, cancellationToken).ConfigureAwait(false);
        return Accepted();
    }

    /// <summary>
    /// Records collector service state events as a batch.
    /// </summary>
    /// <param name="collectorId">Collector identifier.</param>
    /// <param name="events">Events payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Accepted status.</returns>
    [HttpPost("{collectorId:guid}/push/service-events")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PushServiceEventsAsync(
        Guid collectorId,
        [FromBody] IReadOnlyCollection<CollectorServiceEventApiRequest> events,
        CancellationToken cancellationToken)
    {
        var dto = new CollectorServiceEventBatchDto
        {
            Events = events?.Select(e => new CollectorServiceEventDto
            {
                ServiceId = e.ServiceId,
                ServiceName = e.ServiceName,
                State = e.State ?? string.Empty,
                Message = e.Message,
                EventTime = e.Timestamp ?? DateTimeOffset.UtcNow
            }).ToArray() ?? []
        };

        await monitoringService.RecordCollectorServiceEventsAsync(collectorId, dto, cancellationToken).ConfigureAwait(false);
        return Accepted();
    }

    /// <summary>
    /// Records a batch of log entries pushed by a collector.
    /// </summary>
    /// <param name="collectorId">Collector identifier.</param>
    /// <param name="entries">Log entries payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Accepted status.</returns>
    [HttpPost("{collectorId:guid}/push/logs")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PushLogsAsync(
        Guid collectorId,
        [FromBody] IReadOnlyCollection<CollectorLogEntryApiRequest> entries,
        CancellationToken cancellationToken)
    {
        var dto = new CollectorLogBatchDto
        {
            Entries = entries?.Select(e => new CollectorLogEntryDto
            {
                LogFileId = e.LogFileId,
                ServerId = e.ServerId,
                Timestamp = e.Timestamp,
                Severity = e.Severity ?? "INFO",
                Message = e.Message ?? string.Empty,
                JsonPayload = e.JsonPayload
            }).ToArray() ?? []
        };

        await monitoringService.RecordCollectorLogsAsync(collectorId, dto, cancellationToken).ConfigureAwait(false);
        return Accepted();
    }

    /// <summary>
    /// Represents the heartbeat payload posted by collectors.
    /// </summary>
    public sealed class CollectorHeartbeatApiRequest
    {
        public DateTimeOffset? LastSeen { get; init; }

        public IDictionary<string, object>? Health { get; init; }
    }

    /// <summary>
    /// Represents a service event payload pushed by collectors.
    /// </summary>
    public sealed class CollectorServiceEventApiRequest
    {
        public Guid? ServiceId { get; init; }

        public string? ServiceName { get; init; }

        public string? State { get; init; }

        public string? Message { get; init; }

        public DateTimeOffset? Timestamp { get; init; }
    }

    /// <summary>
    /// Represents a log entry payload pushed by collectors.
    /// </summary>
    public sealed class CollectorLogEntryApiRequest
    {
        public Guid LogFileId { get; init; }

        public Guid ServerId { get; init; }

        public DateTimeOffset Timestamp { get; init; }

        public string? Severity { get; init; }

        public string? Message { get; init; }

        public IDictionary<string, object>? JsonPayload { get; init; }
    }
}