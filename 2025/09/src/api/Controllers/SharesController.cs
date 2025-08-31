using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Services;
using api.DTOs;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SharesController : ControllerBase
{
    private readonly ITodoItemShareService _shareService;
    private readonly ICurrentUserService _currentUserService;

    public SharesController(ITodoItemShareService shareService, ICurrentUserService currentUserService)
    {
        _shareService = shareService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// ToDoアイテムを他のユーザーと共有
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TodoItemShareDto>> ShareTodoItem([FromBody] CreateTodoItemShareDto createShareDto)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var share = await _shareService.ShareTodoItemAsync(userId.Value, createShareDto);
            return CreatedAtAction(nameof(GetShare), new { shareId = share.TodoItemShareId }, share);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 特定の共有を取得
    /// </summary>
    [HttpGet("{shareId}")]
    public async Task<ActionResult<TodoItemShareDto>> GetShare(Guid shareId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var share = await _shareService.GetShareByIdAsync(shareId, userId.Value);
        if (share == null)
        {
            return NotFound();
        }

        return Ok(share);
    }

    /// <summary>
    /// ユーザーが共有したToDoアイテム一覧を取得
    /// </summary>
    [HttpGet("shared-by-me")]
    public async Task<ActionResult<List<TodoItemShareDto>>> GetSharedByMe()
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var shares = await _shareService.GetSharesByOwnerAsync(userId.Value);
        return Ok(shares);
    }

    /// <summary>
    /// ユーザーと共有されたToDoアイテム一覧を取得
    /// </summary>
    [HttpGet("shared-with-me")]
    public async Task<ActionResult<List<TodoItemShareDto>>> GetSharedWithMe()
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var shares = await _shareService.GetSharesBySharedUserAsync(userId.Value);
        return Ok(shares);
    }

    /// <summary>
    /// 特定のToDoアイテムの共有一覧を取得
    /// </summary>
    [HttpGet("todoitems/{todoItemId}")]
    public async Task<ActionResult<List<TodoItemShareDto>>> GetSharesByTodoItem(Guid todoItemId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var shares = await _shareService.GetSharesByTodoItemAsync(todoItemId, userId.Value);
        return Ok(shares);
    }

    /// <summary>
    /// 共有の権限を更新
    /// </summary>
    [HttpPut("{shareId}")]
    public async Task<ActionResult<TodoItemShareDto>> UpdateShare(Guid shareId, [FromBody] UpdateTodoItemShareDto updateShareDto)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var updatedShare = await _shareService.UpdateShareAsync(shareId, userId.Value, updateShareDto);
            if (updatedShare == null)
            {
                return NotFound();
            }

            return Ok(updatedShare);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 共有を削除
    /// </summary>
    [HttpDelete("{shareId}")]
    public async Task<ActionResult> DeleteShare(Guid shareId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var success = await _shareService.DeleteShareAsync(shareId, userId.Value);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// 共有を受け入れる
    /// </summary>
    [HttpPost("{shareId}/accept")]
    public async Task<ActionResult<TodoItemShareDto>> AcceptShare(Guid shareId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var acceptedShare = await _shareService.AcceptShareAsync(shareId, userId.Value);
        if (acceptedShare == null)
        {
            return NotFound();
        }

        return Ok(acceptedShare);
    }

    /// <summary>
    /// 共有を拒否する
    /// </summary>
    [HttpPost("{shareId}/reject")]
    public async Task<ActionResult<TodoItemShareDto>> RejectShare(Guid shareId)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var rejectedShare = await _shareService.RejectShareAsync(shareId, userId.Value);
        if (rejectedShare == null)
        {
            return NotFound();
        }

        return Ok(rejectedShare);
    }
}
