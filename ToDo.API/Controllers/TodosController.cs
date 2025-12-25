using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Produces("application/json")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodosController> _logger;

    public TodosController(ITodoService todoService, ILogger<TodosController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all todos
    /// </summary>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(IEnumerable<Todo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Todo>>> GetAll()
    {
        var todos = await _todoService.GetAllTodosAsync();
        return Ok(todos);
    }

    /// <summary>
    /// Gets todo statistics (v2 only)
    /// </summary>
    [HttpGet("statistics")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(TodoStatistics), StatusCodes.Status200OK)]
    public async Task<ActionResult<TodoStatistics>> GetStatistics()
    {
        var stats = await _todoService.GetStatisticsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Search todos (v2 only)
    /// </summary>
    [HttpGet("search")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(IEnumerable<Todo>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Todo>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { Message = "Query parameter is required" });

        var results = await _todoService.SearchTodosAsync(query);
        return Ok(results);
    }

    /// <summary>
    /// Reorder todos (v2 only)
    /// </summary>
    [HttpPost("reorder")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Reorder([FromBody] List<string> orderedIds)
    {
        await _todoService.ReorderTodosAsync(orderedIds);
        return Ok();
    }

    [HttpGet("{id}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Todo>> GetById(string id)
    {
        var todo = await _todoService.GetTodoByIdAsync(id);

        if (todo == null)
        {
            _logger.LogWarning("Todo with id {TodoId} not found", id);
            return NotFound(new { Message = $"Todo with id '{id}' not found" });
        }

        return Ok(todo);
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Todo>> Create(CreateTodoDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var todo = await _todoService.CreateTodoAsync(createDto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = todo.Id, version = "1.0" },
            todo
        );
    }

    [HttpPatch("{id}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Todo>> Update(string id, UpdateTodoDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var todo = await _todoService.UpdateTodoAsync(id, updateDto);

        if (todo == null)
        {
            _logger.LogWarning("Todo with id {TodoId} not found for update", id);
            return NotFound(new { Message = $"Todo with id '{id}' not found" });
        }

        return Ok(todo);
    }

    [HttpDelete("{id}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _todoService.DeleteTodoAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Todo with id {TodoId} not found for deletion", id);
            return NotFound(new { Message = $"Todo with id '{id}' not found" });
        }

        return NoContent();
    }
}
