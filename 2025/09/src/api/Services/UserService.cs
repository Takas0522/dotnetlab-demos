using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.DTOs;

namespace api.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByEntraIdAsync(string entraId);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<List<UserDto>> SearchUsersAsync(string searchText, int limit = 10);
}

public class UserService : IUserService
{
    private readonly TodoDbContext _context;

    public UserService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetUserByEntraIdAsync(string entraId)
    {
        var user = await _context.Users
            .Where(u => u.EntraId == entraId && u.IsActive)
            .FirstOrDefaultAsync();

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users
            .Where(u => u.UserId == userId && u.IsActive)
            .FirstOrDefaultAsync();

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // 既存ユーザーをチェック
        var existingUser = await _context.Users
            .Where(u => u.EntraId == createUserDto.EntraId)
            .FirstOrDefaultAsync();

        if (existingUser != null)
        {
            // 既存ユーザーが非アクティブな場合は再アクティブ化
            if (!existingUser.IsActive)
            {
                existingUser.IsActive = true;
                existingUser.DisplayName = createUserDto.DisplayName;
                existingUser.Email = createUserDto.Email;
                existingUser.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return MapToDto(existingUser);
            }
            
            throw new InvalidOperationException("User with this EntraId already exists");
        }

        var user = new User
        {
            EntraId = createUserDto.EntraId,
            UserPrincipalName = createUserDto.UserPrincipalName,
            DisplayName = createUserDto.DisplayName,
            Email = createUserDto.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
    {
        var user = await _context.Users
            .Where(u => u.UserId == userId && u.IsActive)
            .FirstOrDefaultAsync();

        if (user == null) return null;

        if (updateUserDto.DisplayName != null)
            user.DisplayName = updateUserDto.DisplayName;
        
        if (updateUserDto.Email != null)
            user.Email = updateUserDto.Email;
        
        if (updateUserDto.IsActive.HasValue)
            user.IsActive = updateUserDto.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users
            .Where(u => u.UserId == userId && u.IsActive)
            .FirstOrDefaultAsync();

        if (user == null) return false;

        // 論理削除
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<UserDto>> SearchUsersAsync(string searchText, int limit = 10)
    {
        var users = await _context.Users
            .Where(u => u.IsActive && 
                       (u.DisplayName.Contains(searchText) || 
                        u.UserPrincipalName.Contains(searchText) ||
                        (u.Email != null && u.Email.Contains(searchText))))
            .Take(limit)
            .ToListAsync();

        return users.Select(MapToDto).ToList();
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            EntraId = user.EntraId,
            UserPrincipalName = user.UserPrincipalName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };
    }
}
