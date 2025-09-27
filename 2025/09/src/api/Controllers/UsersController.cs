using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Services;
using api.DTOs;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(IUserService userService, ICurrentUserService currentUserService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 現在のユーザー情報を取得
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var entraId = _currentUserService.GetUserEntraId();
        if (string.IsNullOrEmpty(entraId))
        {
            return Unauthorized("User not authenticated");
        }

        var user = await _userService.GetUserByEntraIdAsync(entraId);
        if (user == null)
        {
            // ユーザーが存在しない場合は自動作成
            var createUserDto = new CreateUserDto
            {
                EntraId = entraId,
                UserPrincipalName = _currentUserService.GetUserPrincipalName() ?? "",
                DisplayName = _currentUserService.GetUserDisplayName() ?? "",
                Email = _currentUserService.GetUserPrincipalName()
            };

            user = await _userService.CreateUserAsync(createUserDto);
        }

        return Ok(user);
    }

    /// <summary>
    /// ユーザーIDでユーザー情報を取得
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// 現在のユーザー情報を更新
    /// </summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
    {
        var currentUserId = _currentUserService.GetUserId();
        if (!currentUserId.HasValue)
        {
            return Unauthorized();
        }

        var updatedUser = await _userService.UpdateUserAsync(currentUserId.Value, updateUserDto);
        if (updatedUser == null)
        {
            return NotFound();
        }

        return Ok(updatedUser);
    }

    /// <summary>
    /// ユーザー検索（共有機能で使用）
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<UserDto>>> SearchUsers([FromQuery] string searchText, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return BadRequest("Search text is required");
        }

        var users = await _userService.SearchUsersAsync(searchText, limit);
        return Ok(users);
    }

    /// <summary>
    /// 現在のユーザーアカウントを削除（論理削除）
    /// </summary>
    [HttpDelete("me")]
    public async Task<ActionResult> DeleteCurrentUser()
    {
        var currentUserId = _currentUserService.GetUserId();
        if (!currentUserId.HasValue)
        {
            return Unauthorized();
        }

        var success = await _userService.DeleteUserAsync(currentUserId.Value);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
