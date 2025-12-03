using Asp.Versioning;
using Falcon.Application.Abstractions;
using Falcon.Application.Contracts.Common;
using Falcon.Application.Contracts.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Controllers.v1;

/// <summary>
/// Provides endpoints for scheduled task monitoring and control.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/tasks")]
public sealed class TasksController(IMonitoringService monitoringService) : ControllerBase
{
    private readonly IMonitoringService monitoringService = monitoringService;

    /// <summary>
    /// Retrieves a scheduled task by identifier.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Scheduled task DTO or 404.</returns>
    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(ScheduledTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScheduledTaskDto>> GetTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var task = await monitoringService.GetScheduledTaskAsync(taskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    /// <summary>
    /// Removes a scheduled task from monitoring.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>No content status.</returns>
    [HttpDelete("{taskId:guid}")]
    [Authorize(Policy = "RequireAdministrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        await monitoringService.DeleteScheduledTaskAsync(taskId, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Retrieves recent run history for a scheduled task.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="limit">Maximum number of runs to retrieve.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Collection of task runs.</returns>
    [HttpGet("{taskId:guid}/runs")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TaskRunDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TaskRunDto>>> GetTaskRunsAsync(
        Guid taskId,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var runs = await monitoringService.GetTaskRunsAsync(taskId, limit, cancellationToken).ConfigureAwait(false);
        return Ok(runs);
    }

    /// <summary>
    /// Triggers a scheduled task for immediate execution.
    /// </summary>
    /// <param name="taskId">Task identifier.</param>
    /// <param name="cancellationToken">Cancellation notification token.</param>
    /// <returns>Trigger acknowledgement payload.</returns>
    [HttpPost("{taskId:guid}/trigger")]
    [Authorize(Policy = "RequireOperator")]
    [ProducesResponseType(typeof(TaskTriggerResponseDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskTriggerResponseDto>> TriggerTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var task = await monitoringService.GetScheduledTaskAsync(taskId, cancellationToken).ConfigureAwait(false);
        if (task is null)
        {
            return NotFound();
        }

        await monitoringService.TriggerTaskAsync(taskId, new TriggerTaskRequestDto(), cancellationToken).ConfigureAwait(false);

        var response = new TaskTriggerResponseDto
        {
            RunId = Guid.NewGuid(),
            StartTime = DateTimeOffset.UtcNow,
            Status = "queued"
        };

        return Accepted(response);
    }
}