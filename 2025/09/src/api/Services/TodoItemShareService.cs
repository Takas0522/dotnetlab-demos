using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.DTOs;

namespace api.Services;

public interface ITodoItemShareService
{
    Task<List<TodoItemShareDto>> GetSharesByOwnerAsync(Guid userId);
    Task<List<TodoItemShareDto>> GetSharesBySharedUserAsync(Guid userId);
    Task<List<TodoItemShareDto>> GetSharesByTodoItemAsync(Guid todoItemId, Guid userId);
    Task<TodoItemShareDto?> GetShareByIdAsync(Guid shareId, Guid userId);
    Task<TodoItemShareDto> ShareTodoItemAsync(Guid ownerId, CreateTodoItemShareDto createShareDto);
    Task<TodoItemShareDto?> UpdateShareAsync(Guid shareId, Guid ownerId, UpdateTodoItemShareDto updateShareDto);
    Task<bool> DeleteShareAsync(Guid shareId, Guid ownerId);
    Task<TodoItemShareDto?> AcceptShareAsync(Guid shareId, Guid userId);
    Task<TodoItemShareDto?> RejectShareAsync(Guid shareId, Guid userId);
}

public class TodoItemShareService : ITodoItemShareService
{
    private readonly TodoDbContext _context;

    public TodoItemShareService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<List<TodoItemShareDto>> GetSharesByOwnerAsync(Guid userId)
    {
        var shares = await _context.TodoItemShares
            .Include(s => s.TodoItem)
            .Include(s => s.SharedUser)
            .Where(s => s.OwnerUserId == userId && s.IsActive)
            .Select(s => new TodoItemShareDto
            {
                TodoItemShareId = s.TodoItemShareId,
                TodoItemId = s.TodoItemId,
                TodoItemTitle = s.TodoItem!.Title,
                OwnerUserId = s.OwnerUserId,
                OwnerDisplayName = s.OwnerUser!.DisplayName,
                SharedUserId = s.SharedUserId,
                SharedUserDisplayName = s.SharedUser!.DisplayName,
                Permission = s.Permission,
                SharedAt = s.SharedAt,
                IsActive = s.IsActive
            })
            .ToListAsync();

        return shares;
    }

    public async Task<List<TodoItemShareDto>> GetSharesBySharedUserAsync(Guid userId)
    {
        var shares = await _context.TodoItemShares
            .Include(s => s.TodoItem)
            .Include(s => s.OwnerUser)
            .Where(s => s.SharedUserId == userId && s.IsActive)
            .Select(s => new TodoItemShareDto
            {
                TodoItemShareId = s.TodoItemShareId,
                TodoItemId = s.TodoItemId,
                TodoItemTitle = s.TodoItem!.Title,
                OwnerUserId = s.OwnerUserId,
                OwnerDisplayName = s.OwnerUser!.DisplayName,
                SharedUserId = s.SharedUserId,
                SharedUserDisplayName = s.SharedUser!.DisplayName,
                Permission = s.Permission,
                SharedAt = s.SharedAt,
                IsActive = s.IsActive
            })
            .ToListAsync();

        return shares;
    }

    public async Task<List<TodoItemShareDto>> GetSharesByTodoItemAsync(Guid todoItemId, Guid userId)
    {
        // ユーザーがオーナーであることを確認
        var todoItem = await _context.TodoItems
            .Where(t => t.TodoItemId == todoItemId && t.UserId == userId && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (todoItem == null)
        {
            return new List<TodoItemShareDto>();
        }

        var shares = await _context.TodoItemShares
            .Include(s => s.TodoItem)
            .Include(s => s.SharedUser)
            .Where(s => s.TodoItemId == todoItemId && s.IsActive)
            .Select(s => new TodoItemShareDto
            {
                TodoItemShareId = s.TodoItemShareId,
                TodoItemId = s.TodoItemId,
                TodoItemTitle = s.TodoItem!.Title,
                OwnerUserId = s.OwnerUserId,
                OwnerDisplayName = s.OwnerUser!.DisplayName,
                SharedUserId = s.SharedUserId,
                SharedUserDisplayName = s.SharedUser!.DisplayName,
                Permission = s.Permission,
                SharedAt = s.SharedAt,
                IsActive = s.IsActive
            })
            .ToListAsync();

        return shares;
    }

    public async Task<TodoItemShareDto?> GetShareByIdAsync(Guid shareId, Guid userId)
    {
        var share = await _context.TodoItemShares
            .Include(s => s.TodoItem)
            .Include(s => s.OwnerUser)
            .Include(s => s.SharedUser)
            .Where(s => s.TodoItemShareId == shareId && 
                       (s.OwnerUserId == userId || s.SharedUserId == userId) && 
                       s.IsActive)
            .Select(s => new TodoItemShareDto
            {
                TodoItemShareId = s.TodoItemShareId,
                TodoItemId = s.TodoItemId,
                TodoItemTitle = s.TodoItem!.Title,
                OwnerUserId = s.OwnerUserId,
                OwnerDisplayName = s.OwnerUser!.DisplayName,
                SharedUserId = s.SharedUserId,
                SharedUserDisplayName = s.SharedUser!.DisplayName,
                Permission = s.Permission,
                SharedAt = s.SharedAt,
                IsActive = s.IsActive
            })
            .FirstOrDefaultAsync();

        return share;
    }

    public async Task<TodoItemShareDto> ShareTodoItemAsync(Guid ownerId, CreateTodoItemShareDto createShareDto)
    {
        // ToDoアイテムの存在と権限チェック
        var todoItem = await _context.TodoItems
            .Where(t => t.TodoItemId == createShareDto.TodoItemId && 
                       t.UserId == ownerId && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (todoItem == null)
        {
            throw new InvalidOperationException("TodoItem not found or you don't have permission");
        }

        // 共有先ユーザーの存在チェック（メールアドレスから検索）
        var sharedUser = await _context.Users
            .Where(u => u.Email == createShareDto.SharedWithEmail && u.IsActive)
            .FirstOrDefaultAsync();

        if (sharedUser == null)
        {
            throw new InvalidOperationException("User with specified email not found");
        }

        // 自分自身への共有チェック
        if (ownerId == sharedUser.UserId)
        {
            throw new InvalidOperationException("Cannot share with yourself");
        }

        // 既存の共有チェック
        var existingShare = await _context.TodoItemShares
            .Where(s => s.TodoItemId == createShareDto.TodoItemId && 
                       s.SharedUserId == sharedUser.UserId)
            .FirstOrDefaultAsync();

        if (existingShare != null)
        {
            if (existingShare.IsActive)
            {
                throw new InvalidOperationException("Already shared with this user");
            }
            else
            {
                // 非アクティブな共有を再アクティブ化
                existingShare.IsActive = true;
                existingShare.Permission = createShareDto.Permission;
                existingShare.SharedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                return (await GetShareByIdAsync(existingShare.TodoItemShareId, ownerId))!;
            }
        }

        // 権限の検証
        if (createShareDto.Permission != "ReadOnly" && createShareDto.Permission != "ReadWrite")
        {
            throw new ArgumentException("Permission must be 'ReadOnly' or 'ReadWrite'");
        }

        var share = new TodoItemShare
        {
            TodoItemId = createShareDto.TodoItemId,
            OwnerUserId = ownerId,
            SharedUserId = sharedUser.UserId,
            Permission = createShareDto.Permission,
            SharedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.TodoItemShares.Add(share);
        await _context.SaveChangesAsync();

        return (await GetShareByIdAsync(share.TodoItemShareId, ownerId))!;
    }

    public async Task<TodoItemShareDto?> UpdateShareAsync(Guid shareId, Guid ownerId, UpdateTodoItemShareDto updateShareDto)
    {
        var share = await _context.TodoItemShares
            .Where(s => s.TodoItemShareId == shareId && s.OwnerUserId == ownerId && s.IsActive)
            .FirstOrDefaultAsync();

        if (share == null)
        {
            return null;
        }

        if (updateShareDto.Permission != null)
        {
            if (updateShareDto.Permission != "ReadOnly" && updateShareDto.Permission != "ReadWrite")
            {
                throw new ArgumentException("Permission must be 'ReadOnly' or 'ReadWrite'");
            }
            share.Permission = updateShareDto.Permission;
        }

        if (updateShareDto.IsActive.HasValue)
        {
            share.IsActive = updateShareDto.IsActive.Value;
        }

        await _context.SaveChangesAsync();

        return await GetShareByIdAsync(shareId, ownerId);
    }

    public async Task<bool> DeleteShareAsync(Guid shareId, Guid ownerId)
    {
        var share = await _context.TodoItemShares
            .Where(s => s.TodoItemShareId == shareId && s.OwnerUserId == ownerId)
            .FirstOrDefaultAsync();

        if (share == null)
        {
            return false;
        }

        share.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TodoItemShareDto?> AcceptShareAsync(Guid shareId, Guid userId)
    {
        var share = await _context.TodoItemShares
            .Where(s => s.TodoItemShareId == shareId && s.SharedUserId == userId && s.IsActive)
            .FirstOrDefaultAsync();

        if (share == null)
        {
            return null;
        }

        // すでに受け入れ済みの場合はそのまま返す
        return await GetShareByIdAsync(shareId, userId);
    }

    public async Task<TodoItemShareDto?> RejectShareAsync(Guid shareId, Guid userId)
    {
        var share = await _context.TodoItemShares
            .Where(s => s.TodoItemShareId == shareId && s.SharedUserId == userId && s.IsActive)
            .FirstOrDefaultAsync();

        if (share == null)
        {
            return null;
        }

        share.IsActive = false;
        await _context.SaveChangesAsync();

        return await GetShareByIdAsync(shareId, userId);
    }
}
