using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Services;
using api.DTOs;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TagsController> _logger;

    public TagsController(ITagService tagService, ICurrentUserService currentUserService, ILogger<TagsController> logger)
    {
        _tagService = tagService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// ユーザーのタグ一覧を取得
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags()
    {
        try
        {
            _logger.LogInformation("GetTags endpoint called");
            
            var userId = _currentUserService.GetUserId();
            _logger.LogInformation("Retrieved userId: {UserId}", userId);
            
            if (!userId.HasValue)
            {
                _logger.LogWarning("UserId is null, returning Unauthorized");
                return Unauthorized();
            }

            var tags = await _tagService.GetUserTagsAsync(userId.Value);
            _logger.LogInformation("Retrieved {TagCount} tags for user {UserId}", tags.Count, userId.Value);
            
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetTags endpoint");
            return StatusCode(500, new { error = "Internal server error", message = ex.Message });
        }
    }

    /// <summary>
    /// 特定のタグを取得
    /// </summary>
    [HttpGet("{tagId}")]
    public async Task<ActionResult<TagDto>> GetTag(Guid tagId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var tag = await _tagService.GetTagByIdAsync(tagId, userId.Value);
        if (tag == null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    /// <summary>
    /// 新しいタグを作成
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag([FromBody] CreateTagDto createTagDto)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            _logger.LogWarning("User not found or not authenticated when creating tag");
            return Unauthorized("User not found. Please ensure you are logged in.");
        }

        try
        {
            // Application Insights用の意図的な遅延（3-5秒）
            _logger.LogInformation("Creating tag with intentional delay for Application Insights testing");
            var delayMilliseconds = Random.Shared.Next(5000, 10001); // 5-10秒のランダム遅延
            _logger.LogInformation("Applying delay of {DelayMs}ms to tag creation", delayMilliseconds);
            await Task.Delay(delayMilliseconds);

            var tag = await _tagService.CreateTagAsync(userId.Value, createTagDto);
            _logger.LogInformation("Tag created successfully after {DelayMs}ms delay", delayMilliseconds);
            return CreatedAtAction(nameof(GetTag), new { tagId = tag.TagId }, tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tag for user {UserId}", userId);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// タグを更新
    /// </summary>
    [HttpPut("{tagId}")]
    public async Task<ActionResult<TagDto>> UpdateTag(Guid tagId, [FromBody] UpdateTagDto updateTagDto)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var updatedTag = await _tagService.UpdateTagAsync(tagId, userId.Value, updateTagDto);
            if (updatedTag == null)
            {
                return NotFound();
            }

            return Ok(updatedTag);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// タグを削除
    /// </summary>
    [HttpDelete("{tagId}")]
    public async Task<ActionResult> DeleteTag(Guid tagId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var success = await _tagService.DeleteTagAsync(tagId, userId.Value);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// タグに関連するToDoアイテム一覧を取得
    /// </summary>
    [HttpGet("{tagId}/todoitems")]
    public async Task<ActionResult<List<TodoItemDto>>> GetTodoItemsByTag(Guid tagId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var todoItems = await _tagService.GetTodoItemsByTagAsync(tagId, userId.Value);
        if (todoItems == null)
        {
            return NotFound("Tag not found");
        }

        return Ok(todoItems);
    }

    /// <summary>
    /// ToDoアイテムにタグを追加
    /// </summary>
    [HttpPost("todoitems/{todoItemId}/tags/{tagId}")]
    public async Task<ActionResult> AddTagToTodoItem(Guid todoItemId, Guid tagId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var success = await _tagService.AddTagToTodoItemAsync(todoItemId, tagId, userId.Value);
            if (!success)
            {
                return NotFound("TodoItem or Tag not found");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// ToDoアイテムからタグを削除
    /// </summary>
    [HttpDelete("todoitems/{todoItemId}/tags/{tagId}")]
    public async Task<ActionResult> RemoveTagFromTodoItem(Guid todoItemId, Guid tagId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var success = await _tagService.RemoveTagFromTodoItemAsync(todoItemId, tagId, userId.Value);
        if (!success)
        {
            return NotFound("TodoItem, Tag, or association not found");
        }

        return NoContent();
    }
}
