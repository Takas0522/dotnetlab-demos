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

    public TagsController(ITagService tagService, ICurrentUserService currentUserService)
    {
        _tagService = tagService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// ユーザーのタグ一覧を取得
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags()
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var tags = await _tagService.GetUserTagsAsync(userId.Value);
        return Ok(tags);
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
            return Unauthorized();
        }

        try
        {
            var tag = await _tagService.CreateTagAsync(userId.Value, createTagDto);
            return CreatedAtAction(nameof(GetTag), new { tagId = tag.TagId }, tag);
        }
        catch (Exception ex)
        {
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
