using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.DTOs;

namespace api.Services;

public interface ITodoItemService
{
    Task<List<TodoItemDto>> GetUserTodoItemsAsync(Guid userId, TodoItemFilterDto filter);
    Task<TodoItemDto?> GetTodoItemByIdAsync(Guid todoItemId, Guid userId);
    Task<TodoItemDto> CreateTodoItemAsync(Guid userId, CreateTodoItemDto createTodoItemDto);
    Task<TodoItemDto?> UpdateTodoItemAsync(Guid todoItemId, Guid userId, UpdateTodoItemDto updateTodoItemDto);
    Task<bool> DeleteTodoItemAsync(Guid todoItemId, Guid userId);
    Task<bool> DeleteMultipleTodoItemsAsync(List<Guid> todoItemIds, Guid userId);
    Task<TodoItemDto?> CompleteTodoItemAsync(Guid todoItemId, Guid userId);
    Task<List<TodoItemDto>> GetSharedTodoItemsAsync(Guid userId);
}

public class TodoItemService : ITodoItemService
{
    private readonly TodoDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public TodoItemService(TodoDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<TodoItemDto>> GetUserTodoItemsAsync(Guid userId, TodoItemFilterDto filter)
    {
        var query = _context.TodoItems
            .Include(t => t.User)
            .Include(t => t.TodoItemTags)
                .ThenInclude(tt => tt.Tag)
            .Include(t => t.TodoItemShares)
            .Where(t => !t.IsDeleted)
            .Where(t => t.UserId == userId || 
                       t.TodoItemShares.Any(s => s.SharedUserId == userId && s.IsActive));

        // フィルター適用
        if (filter.IsCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == filter.IsCompleted.Value);

        if (filter.Priority.HasValue)
            query = query.Where(t => t.Priority == filter.Priority.Value);

        if (filter.DueDateFrom.HasValue)
            query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);

        if (filter.DueDateTo.HasValue)
            query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);

        if (filter.TagIds != null && filter.TagIds.Any())
            query = query.Where(t => t.TodoItemTags.Any(tt => filter.TagIds.Contains(tt.TagId)));

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
            query = query.Where(t => t.Title.Contains(filter.SearchText) || 
                                   (t.Description != null && t.Description.Contains(filter.SearchText)));

        // ページング
        var todoItems = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return todoItems.Select(t => MapToDto(t, userId)).ToList();
    }

    public async Task<TodoItemDto?> GetTodoItemByIdAsync(Guid todoItemId, Guid userId)
    {
        var todoItem = await _context.TodoItems
            .Include(t => t.User)
            .Include(t => t.TodoItemTags)
                .ThenInclude(tt => tt.Tag)
            .Include(t => t.TodoItemShares)
            .Where(t => !t.IsDeleted && t.TodoItemId == todoItemId)
            .Where(t => t.UserId == userId || 
                       t.TodoItemShares.Any(s => s.SharedUserId == userId && s.IsActive))
            .FirstOrDefaultAsync();

        return todoItem == null ? null : MapToDto(todoItem, userId);
    }

    public async Task<TodoItemDto> CreateTodoItemAsync(Guid userId, CreateTodoItemDto createTodoItemDto)
    {
        // ユーザーの存在確認と自動作成
        await EnsureUserExistsAsync(userId);

        var todoItem = new TodoItem
        {
            UserId = userId,
            Title = createTodoItemDto.Title,
            Description = createTodoItemDto.Description,
            Priority = createTodoItemDto.Priority,
            DueDate = createTodoItemDto.DueDate
        };

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        // タグの関連付け
        if (createTodoItemDto.TagIds.Any())
        {
            await AddTagsToTodoItemAsync(todoItem.TodoItemId, createTodoItemDto.TagIds, userId);
        }

        // 作成されたアイテムを再取得
        var createdItem = await GetTodoItemByIdAsync(todoItem.TodoItemId, userId);
        return createdItem!;
    }

    private async Task EnsureUserExistsAsync(Guid userId)
    {
        var existingUser = await _context.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefaultAsync();

        if (existingUser == null)
        {
            // ユーザーが存在しない場合、認証情報から基本情報を取得して作成
            var user = new User
            {
                UserId = userId,
                EntraId = _currentUserService.GetUserEntraId() ?? userId.ToString(),
                UserPrincipalName = _currentUserService.GetUserPrincipalName() ?? $"user-{userId}@unknown.com",
                DisplayName = _currentUserService.GetUserDisplayName() ?? "Unknown User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<TodoItemDto?> UpdateTodoItemAsync(Guid todoItemId, Guid userId, UpdateTodoItemDto updateTodoItemDto)
    {
        var todoItem = await _context.TodoItems
            .Include(t => t.TodoItemShares)
            .Where(t => !t.IsDeleted && t.TodoItemId == todoItemId)
            .FirstOrDefaultAsync();

        if (todoItem == null) return null;

        // 権限チェック
        var hasWritePermission = todoItem.UserId == userId || 
                               todoItem.TodoItemShares.Any(s => s.SharedUserId == userId && 
                                                               s.IsActive && s.Permission == "ReadWrite");

        if (!hasWritePermission) return null;

        // 更新処理
        if (updateTodoItemDto.Title != null)
            todoItem.Title = updateTodoItemDto.Title;

        if (updateTodoItemDto.Description != null)
            todoItem.Description = updateTodoItemDto.Description;

        if (updateTodoItemDto.IsCompleted.HasValue)
        {
            todoItem.IsCompleted = updateTodoItemDto.IsCompleted.Value;
            todoItem.CompletedAt = updateTodoItemDto.IsCompleted.Value ? DateTime.UtcNow : null;
        }

        if (updateTodoItemDto.Priority.HasValue)
            todoItem.Priority = updateTodoItemDto.Priority.Value;

        if (updateTodoItemDto.DueDate.HasValue)
            todoItem.DueDate = updateTodoItemDto.DueDate.Value;

        todoItem.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // タグの更新
        if (updateTodoItemDto.TagIds != null)
        {
            await UpdateTodoItemTagsAsync(todoItemId, updateTodoItemDto.TagIds, userId);
        }

        return await GetTodoItemByIdAsync(todoItemId, userId);
    }

    public async Task<bool> DeleteTodoItemAsync(Guid todoItemId, Guid userId)
    {
        var todoItem = await _context.TodoItems
            .Where(t => !t.IsDeleted && t.TodoItemId == todoItemId && t.UserId == userId)
            .FirstOrDefaultAsync();

        if (todoItem == null) return false;

        todoItem.IsDeleted = true;
        todoItem.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteMultipleTodoItemsAsync(List<Guid> todoItemIds, Guid userId)
    {
        var todoItems = await _context.TodoItems
            .Where(t => !t.IsDeleted && todoItemIds.Contains(t.TodoItemId) && t.UserId == userId)
            .ToListAsync();

        if (!todoItems.Any()) return false;

        foreach (var todoItem in todoItems)
        {
            todoItem.IsDeleted = true;
            todoItem.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TodoItemDto?> CompleteTodoItemAsync(Guid todoItemId, Guid userId)
    {
        var updateDto = new UpdateTodoItemDto { IsCompleted = true };
        return await UpdateTodoItemAsync(todoItemId, userId, updateDto);
    }

    public async Task<List<TodoItemDto>> GetSharedTodoItemsAsync(Guid userId)
    {
        var sharedItems = await _context.TodoItems
            .Include(t => t.User)
            .Include(t => t.TodoItemTags)
                .ThenInclude(tt => tt.Tag)
            .Include(t => t.TodoItemShares)
            .Where(t => !t.IsDeleted && 
                       t.TodoItemShares.Any(s => s.SharedUserId == userId && s.IsActive))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return sharedItems.Select(t => MapToDto(t, userId)).ToList();
    }

    private async Task AddTagsToTodoItemAsync(Guid todoItemId, List<Guid> tagIds, Guid userId)
    {
        // ユーザーが所有するタグのみを対象とする
        var validTags = await _context.Tags
            .Where(t => tagIds.Contains(t.TagId) && t.UserId == userId && !t.IsDeleted)
            .Select(t => t.TagId)
            .ToListAsync();

        var todoItemTags = validTags.Select(tagId => new TodoItemTag
        {
            TodoItemId = todoItemId,
            TagId = tagId
        }).ToList();

        _context.TodoItemTags.AddRange(todoItemTags);
        await _context.SaveChangesAsync();
    }

    private async Task UpdateTodoItemTagsAsync(Guid todoItemId, List<Guid> tagIds, Guid userId)
    {
        // 既存のタグ関連を削除
        var existingTags = await _context.TodoItemTags
            .Where(tt => tt.TodoItemId == todoItemId)
            .ToListAsync();

        _context.TodoItemTags.RemoveRange(existingTags);

        // 新しいタグを追加
        if (tagIds.Any())
        {
            await AddTagsToTodoItemAsync(todoItemId, tagIds, userId);
        }
        else
        {
            await _context.SaveChangesAsync();
        }
    }

    private static TodoItemDto MapToDto(TodoItem todoItem, Guid currentUserId)
    {
        var accessType = todoItem.UserId == currentUserId ? "Owner" : "Shared";
        var permission = todoItem.UserId == currentUserId ? "ReadWrite" : 
                        todoItem.TodoItemShares
                            .Where(s => s.SharedUserId == currentUserId && s.IsActive)
                            .Select(s => s.Permission)
                            .FirstOrDefault() ?? "ReadOnly";

        return new TodoItemDto
        {
            TodoItemId = todoItem.TodoItemId,
            UserId = todoItem.UserId,
            UserDisplayName = todoItem.User?.DisplayName ?? "",
            Title = todoItem.Title,
            Description = todoItem.Description,
            IsCompleted = todoItem.IsCompleted,
            Priority = todoItem.Priority,
            DueDate = todoItem.DueDate,
            CompletedAt = todoItem.CompletedAt,
            CreatedAt = todoItem.CreatedAt,
            UpdatedAt = todoItem.UpdatedAt,
            Tags = todoItem.TodoItemTags?.Select(tt => new TagDto
            {
                TagId = tt.Tag.TagId,
                UserId = tt.Tag.UserId,
                TagName = tt.Tag.TagName,
                ColorCode = tt.Tag.ColorCode,
                CreatedAt = tt.Tag.CreatedAt,
                UpdatedAt = tt.Tag.UpdatedAt
            }).ToList() ?? [],
            AccessType = accessType,
            Permission = permission
        };
    }
}
