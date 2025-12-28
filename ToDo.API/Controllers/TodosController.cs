using ToDo.Application.Todos.Queries;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Todos.Commands;
using TodoApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Produces("application/json")]
public class TodosController : ControllerBase
{
    private readonly ISender _mediator;

    public TodosController(ISender mediator)
    {
        _mediator = mediator;
    }

    // --- Version 1.0 & 2.0 Shared Endpoints ---

    [HttpGet]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllTodosQuery()));
    }

    [HttpGet("{id}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<TodoDto>> GetById(string id)
    {
        var result = await _mediator.Send(new GetTodoByIdQuery(id));
        return result != null ? Ok(result) : NotFound(new { Message = $"Todo with id '{id}' not found" });
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<TodoDto>> Create(CreateTodoCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id, version = "1.0" }, result);
    }

    [HttpPatch("{id}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<TodoDto>> Update(string id, UpdateTodoDto dto)
    {
        // Mapping route ID and body DTO to a single Command
        var command = new UpdateTodoCommand { Id = id, Title = dto.Title, IsCompleted = dto.IsCompleted };
        var result = await _mediator.Send(command);

        return result != null ? Ok(result) : NotFound(new { Message = $"Todo with id '{id}' not found" });
    }

    [HttpDelete("{id}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _mediator.Send(new DeleteTodoCommand(id));
        return success ? NoContent() : NotFound(new { Message = $"Todo with id '{id}' not found" });
    }

    // --- Version 2.0 Exclusive Endpoints ---

    [HttpGet("statistics")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<TodoStatisticsDto>> GetStatistics()
    {
        return Ok(await _mediator.Send(new GetTodoStatisticsQuery()));
    }

    [HttpGet("search")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { Message = "Query parameter is required" });

        return Ok(await _mediator.Send(new SearchTodosQuery(query)));
    }

    [HttpPost("reorder")]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Reorder([FromBody] List<string> orderedIds)
    {
        await _mediator.Send(new ReorderTodosCommand(orderedIds));
        return Ok();
    }
}