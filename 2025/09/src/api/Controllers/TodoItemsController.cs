using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Services;
using api.DTOs;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoItemService _todoItemService;
    private readonly ICurrentUserService _currentUserService;

    public TodoItemsController(ITodoItemService todoItemService, ICurrentUserService currentUserService)
    {
        _todoItemService = todoItemService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// ユーザーのToDoアイテム一覧を取得
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TodoItemDto>>> GetTodoItems([FromQuery] TodoItemFilterDto filter)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var todoItems = await _todoItemService.GetUserTodoItemsAsync(userId.Value, filter);
        return Ok(todoItems);
    }

    /// <summary>
    /// 特定のToDoアイテムを取得
    /// </summary>
    [HttpGet("{todoItemId}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoItem(Guid todoItemId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var todoItem = await _todoItemService.GetTodoItemByIdAsync(todoItemId, userId.Value);
        if (todoItem == null)
        {
            return NotFound();
        }

        return Ok(todoItem);
    }

    /// <summary>
    /// 新しいToDoアイテムを作成
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> CreateTodoItem([FromBody] CreateTodoItemDto createTodoItemDto)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var todoItem = await _todoItemService.CreateTodoItemAsync(userId.Value, createTodoItemDto);
            return CreatedAtAction(nameof(GetTodoItem), new { todoItemId = todoItem.TodoItemId }, todoItem);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// ToDoアイテムを更新
    /// </summary>
    [HttpPut("{todoItemId}")]
    public async Task<ActionResult<TodoItemDto>> UpdateTodoItem(Guid todoItemId, [FromBody] UpdateTodoItemDto updateTodoItemDto)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var updatedTodoItem = await _todoItemService.UpdateTodoItemAsync(todoItemId, userId.Value, updateTodoItemDto);
            if (updatedTodoItem == null)
            {
                return NotFound();
            }

            return Ok(updatedTodoItem);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// ToDoアイテムを削除
    /// </summary>
    [HttpDelete("{todoItemId}")]
    public async Task<ActionResult> DeleteTodoItem(Guid todoItemId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var success = await _todoItemService.DeleteTodoItemAsync(todoItemId, userId.Value);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// 複数のToDoアイテムを一括削除
    /// </summary>
    [HttpDelete("bulk")]
    public async Task<ActionResult> DeleteMultipleTodoItems([FromBody] List<Guid> todoItemIds)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        if (!todoItemIds.Any())
        {
            return BadRequest("TodoItem IDs are required");
        }

        var success = await _todoItemService.DeleteMultipleTodoItemsAsync(todoItemIds, userId.Value);
        if (!success)
        {
            return NotFound("No todo items were found or deleted");
        }

        return NoContent();
    }

    /// <summary>
    /// ToDoアイテムを完了状態にする
    /// </summary>
    [HttpPost("{todoItemId}/complete")]
    public async Task<ActionResult<TodoItemDto>> CompleteTodoItem(Guid todoItemId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var completedTodoItem = await _todoItemService.CompleteTodoItemAsync(todoItemId, userId.Value);
        if (completedTodoItem == null)
        {
            return NotFound();
        }

        return Ok(completedTodoItem);
    }

    /// <summary>
    /// 共有されたToDoアイテム一覧を取得
    /// </summary>
    [HttpGet("shared")]
    public async Task<ActionResult<List<TodoItemDto>>> GetSharedTodoItems()
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var sharedTodoItems = await _todoItemService.GetSharedTodoItemsAsync(userId.Value);
        return Ok(sharedTodoItems);
    }
}
