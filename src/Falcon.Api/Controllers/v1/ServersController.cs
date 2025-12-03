using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Common;
using Falcon.Application.Contracts.Iis;
using Falcon.Application.Contracts.Servers;
using Falcon.Application.Contracts.Services;
using Falcon.Application.Contracts.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Manages server inventory resources and server-scoped operations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/servers")]
public sealed class ServersController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves a paged list of monitored servers with optional filters.
    /// </summary>
    /// <param name="environment">Optional environment filter (dev/test/prod).</param>
    /// <param name="search">Optional search term across hostname and display name.</param>
    /// <param name="page">Page index (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Paged server summaries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<ServerSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<ServerSummaryDto>>> GetServersAsync(
        [FromQuery] string? environment,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await monitoringService.GetServersAsync(environment, search, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Registers a new server in the inventory.
    /// </summary>
    /// <param name="request">Server registration payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Created server detail.</returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(ServerDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServerDetailDto>> RegisterServerAsync(
        [FromBody] RegisterServerRequestDto request,
        CancellationToken cancellationToken)
    {
        var server = await monitoringService.CreateServerAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetServerAsync), new { serverId = server.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0" }, server);
    }

    /// <summary>
    /// Retrieves a server detail projection by identifier.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Server detail or 404.</returns>
    [HttpGet("{serverId:guid}")]
    [ProducesResponseType(typeof(ServerDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServerDetailDto>> GetServerAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var server = await monitoringService.GetServerAsync(serverId, cancellationToken).ConfigureAwait(false);
        if (server is null)
        {
            return NotFound();
        }

        return Ok(server);
    }

    /// <summary>
    /// Updates metadata associated with a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="request">Server update payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Updated server detail or 404.</returns>
    [HttpPut("{serverId:guid}")]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(ServerDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServerDetailDto>> UpdateServerAsync(
        Guid serverId,
        [FromBody] UpdateServerRequestDto request,
        CancellationToken cancellationToken)
    {
        var updated = await monitoringService.UpdateServerAsync(serverId, request, cancellationToken).ConfigureAwait(false);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    /// <summary>
    /// Deletes a server from the inventory (soft delete envisioned).
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>No content status.</returns>
    [HttpDelete("{serverId:guid}")]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteServerAsync(Guid serverId, CancellationToken cancellationToken)
    {
        await monitoringService.DeleteServerAsync(serverId, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Queues a service restart on the specified server by service name.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="request">Restart request payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Acknowledgement with action metadata.</returns>
    [HttpPost("{serverId:guid}/actions/restart-service")]
    [Authorize(Policy = "RequireOperator")]
    [ProducesResponseType(typeof(ActionAcknowledgementDto), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<ActionAcknowledgementDto>> RestartServiceAsync(
        Guid serverId,
        [FromBody] RestartServerServiceRequestDto request,
        CancellationToken cancellationToken)
    {
        await monitoringService.TriggerServerServiceRestartAsync(serverId, request, cancellationToken).ConfigureAwait(false);

        var response = new ActionAcknowledgementDto
        {
            ActionId = Guid.NewGuid(),
            ScheduledAt = DateTimeOffset.UtcNow,
            Status = "queued"
        };

        return Accepted(response);
    }

    /// <summary>
    /// Retrieves services monitored on a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of monitored services.</returns>
    [HttpGet("{serverId:guid}/services")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MonitoredServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<MonitoredServiceDto>>> GetServerServicesAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var services = await monitoringService.GetServicesAsync(serverId, cancellationToken).ConfigureAwait(false);
        return Ok(services);
    }

    /// <summary>
    /// Registers a monitored service for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="request">Service registration payload.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Created service DTO.</returns>
    [HttpPost("{serverId:guid}/services")]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(typeof(MonitoredServiceDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<MonitoredServiceDto>> RegisterServiceAsync(
        Guid serverId,
        [FromBody] RegisterServiceRequestDto request,
        CancellationToken cancellationToken)
    {
        var service = await monitoringService.RegisterServiceAsync(serverId, request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(ServicesController.GetServiceAsync), "Services", new { serviceId = service.Id, version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0" }, service);
    }

    /// <summary>
    /// Retrieves scheduled tasks associated with a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of scheduled tasks.</returns>
    [HttpGet("{serverId:guid}/tasks")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ScheduledTaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ScheduledTaskDto>>> GetServerTasksAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var tasks = await monitoringService.GetScheduledTasksAsync(serverId, cancellationToken).ConfigureAwait(false);
        return Ok(tasks);
    }

    /// <summary>
    /// Retrieves IIS application pools for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of application pools.</returns>
    [HttpGet("{serverId:guid}/iis/app-pools")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AppPoolDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AppPoolDto>>> GetServerAppPoolsAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var pools = await monitoringService.GetAppPoolsAsync(serverId, cancellationToken).ConfigureAwait(false);
        return Ok(pools);
    }

    /// <summary>
    /// Retrieves IIS sites for a server.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of IIS sites.</returns>
    [HttpGet("{serverId:guid}/iis/sites")]
    [ProducesResponseType(typeof(IReadOnlyCollection<IisSiteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<IisSiteDto>>> GetServerSitesAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var sites = await monitoringService.GetIisSitesAsync(serverId, cancellationToken).ConfigureAwait(false);
        return Ok(sites);
    }

    /// <summary>
    /// Retrieves metric points for a server and optional metric filter.
    /// </summary>
    /// <param name="serverId">Server identifier.</param>
    /// <param name="metric">Optional metric filter.</param>
    /// <param name="from">Optional start timestamp.</param>
    /// <param name="to">Optional end timestamp.</param>
    /// <param name="interval">Optional aggregation interval.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Metric series response.</returns>
    [HttpGet("{serverId:guid}/metrics")]
    [ProducesResponseType(typeof(MetricSeriesResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<MetricSeriesResponseDto>> GetServerMetricsAsync(
        Guid serverId,
        [FromQuery(Name = "metric")] string? metric,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] string? interval,
        CancellationToken cancellationToken)
    {
        var points = await monitoringService.GetMetricsAsync(serverId, metric, from, to, interval, cancellationToken).ConfigureAwait(false);
        var metricName = metric ?? points.FirstOrDefault()?.MetricName ?? "cpu";

        var response = new MetricSeriesResponseDto
        {
            Metric = metricName,
            Points = [.. points
                .Select(p => new MetricSeriesPointDto
                {
                    Timestamp = p.MeasuredAt,
                    Value = p.MetricValue
                })]
        };

        return Ok(response);
    }
}