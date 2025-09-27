using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Models;
using api.DTOs;

namespace api.Services;

public interface ITagService
{
    Task<List<TagDto>> GetUserTagsAsync(Guid userId);
    Task<TagDto?> GetTagByIdAsync(Guid tagId, Guid userId);
    Task<TagDto> CreateTagAsync(Guid userId, CreateTagDto createTagDto);
    Task<TagDto?> UpdateTagAsync(Guid tagId, Guid userId, UpdateTagDto updateTagDto);
    Task<bool> DeleteTagAsync(Guid tagId, Guid userId);
    Task<List<TagDto>> GetPopularTagsAsync(Guid userId, int limit = 10);
    Task<List<TodoItemDto>?> GetTodoItemsByTagAsync(Guid tagId, Guid userId);
    Task<bool> AddTagToTodoItemAsync(Guid todoItemId, Guid tagId, Guid userId);
    Task<bool> RemoveTagFromTodoItemAsync(Guid todoItemId, Guid tagId, Guid userId);
}

public class TagService : ITagService
{
    private readonly TodoDbContext _context;

    public TagService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<List<TagDto>> GetUserTagsAsync(Guid userId)
    {
        var tags = await _context.Tags
            .Include(t => t.TodoItemTags)
                .ThenInclude(tt => tt.TodoItem)
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .OrderBy(t => t.TagName)
            .ToListAsync();

        return tags.Select(MapToDto).ToList();
    }

    public async Task<TagDto?> GetTagByIdAsync(Guid tagId, Guid userId)
    {
        var tag = await _context.Tags
            .Include(t => t.TodoItemTags)
                .ThenInclude(tt => tt.TodoItem)
            .Where(t => t.TagId == tagId && t.UserId == userId && !t.IsDeleted)
            .FirstOrDefaultAsync();

        return tag == null ? null : MapToDto(tag);
    }

    public async Task<TagDto> CreateTagAsync(Guid userId, CreateTagDto createTagDto)
    {
        // 同じ名前のタグが既に存在するかチェック
        var existingTag = await _context.Tags
            .Where(t => t.UserId == userId && t.TagName == createTagDto.TagName && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (existingTag != null)
        {
            throw new InvalidOperationException($"Tag with name '{createTagDto.TagName}' already exists");
        }

        var tag = new Tag
        {
            UserId = userId,
            TagName = createTagDto.TagName,
            ColorCode = createTagDto.ColorCode
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return MapToDto(tag);
    }

    public async Task<TagDto?> UpdateTagAsync(Guid tagId, Guid userId, UpdateTagDto updateTagDto)
    {
        var tag = await _context.Tags
            .Where(t => t.TagId == tagId && t.UserId == userId && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (tag == null) return null;

        // タグ名の重複チェック
        if (updateTagDto.TagName != null && updateTagDto.TagName != tag.TagName)
        {
            var existingTag = await _context.Tags
                .Where(t => t.UserId == userId && t.TagName == updateTagDto.TagName && 
                           t.TagId != tagId && !t.IsDeleted)
                .FirstOrDefaultAsync();

            if (existingTag != null)
            {
                throw new InvalidOperationException($"Tag with name '{updateTagDto.TagName}' already exists");
            }

            tag.TagName = updateTagDto.TagName;
        }

        if (updateTagDto.ColorCode != null)
            tag.ColorCode = updateTagDto.ColorCode;

        tag.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return await GetTagByIdAsync(tagId, userId);
    }

    public async Task<bool> DeleteTagAsync(Guid tagId, Guid userId)
    {
        var tag = await _context.Tags
            .Where(t => t.TagId == tagId && t.UserId == userId && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (tag == null) return false;

        // タグが使用されているかチェック
        var isUsed = await _context.TodoItemTags
            .AnyAsync(tt => tt.TagId == tagId);

        if (isUsed)
        {
            // 使用されている場合は論理削除
            tag.IsDeleted = true;
            tag.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // 使用されていない場合は物理削除
            _context.Tags.Remove(tag);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TagDto>> GetPopularTagsAsync(Guid userId, int limit = 10)
    {
        var popularTags = await _context.Tags
            .Include(t => t.TodoItemTags)
                .ThenInclude(tt => tt.TodoItem)
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .Where(t => t.TodoItemTags.Any(tt => !tt.TodoItem.IsDeleted))
            .OrderByDescending(t => t.TodoItemTags.Count(tt => !tt.TodoItem.IsDeleted))
            .Take(limit)
            .ToListAsync();

        return popularTags.Select(MapToDto).ToList();
    }

    public async Task<List<TodoItemDto>?> GetTodoItemsByTagAsync(Guid tagId, Guid userId)
    {
        // タグの存在と権限チェック
        var tag = await _context.Tags
            .Where(t => t.TagId == tagId && t.UserId == userId)
            .FirstOrDefaultAsync();

        if (tag == null)
        {
            return null;
        }

        var todoItems = await _context.TodoItemTags
            .Include(tt => tt.TodoItem)
                .ThenInclude(t => t.TodoItemTags)
                    .ThenInclude(tt => tt.Tag)
            .Where(tt => tt.TagId == tagId && 
                        !tt.TodoItem.IsDeleted && 
                        tt.TodoItem.UserId == userId)
            .Select(tt => new TodoItemDto
            {
                TodoItemId = tt.TodoItem.TodoItemId,
                UserId = tt.TodoItem.UserId,
                Title = tt.TodoItem.Title,
                Description = tt.TodoItem.Description,
                IsCompleted = tt.TodoItem.IsCompleted,
                Priority = tt.TodoItem.Priority,
                DueDate = tt.TodoItem.DueDate,
                CreatedAt = tt.TodoItem.CreatedAt,
                UpdatedAt = tt.TodoItem.UpdatedAt,
                Tags = tt.TodoItem.TodoItemTags.Select(tit => new TagDto
                {
                    TagId = tit.Tag.TagId,
                    UserId = tit.Tag.UserId,
                    TagName = tit.Tag.TagName,
                    ColorCode = tit.Tag.ColorCode,
                    CreatedAt = tit.Tag.CreatedAt,
                    UpdatedAt = tit.Tag.UpdatedAt,
                    UsageCount = 0
                }).ToList()
            })
            .ToListAsync();

        return todoItems;
    }

    public async Task<bool> AddTagToTodoItemAsync(Guid todoItemId, Guid tagId, Guid userId)
    {
        // ToDoアイテムの存在と権限チェック
        var todoItem = await _context.TodoItems
            .Where(t => t.TodoItemId == todoItemId && t.UserId == userId && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (todoItem == null)
        {
            return false;
        }

        // タグの存在と権限チェック
        var tag = await _context.Tags
            .Where(t => t.TagId == tagId && t.UserId == userId)
            .FirstOrDefaultAsync();

        if (tag == null)
        {
            return false;
        }

        // 既存の関連をチェック
        var existingRelation = await _context.TodoItemTags
            .Where(tt => tt.TodoItemId == todoItemId && tt.TagId == tagId)
            .FirstOrDefaultAsync();

        if (existingRelation != null)
        {
            // 既に関連している場合は何もしない
            return true;
        }

        var todoItemTag = new TodoItemTag
        {
            TodoItemId = todoItemId,
            TagId = tagId
        };

        _context.TodoItemTags.Add(todoItemTag);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveTagFromTodoItemAsync(Guid todoItemId, Guid tagId, Guid userId)
    {
        // ToDoアイテムの存在と権限チェック
        var todoItem = await _context.TodoItems
            .Where(t => t.TodoItemId == todoItemId && t.UserId == userId && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (todoItem == null)
        {
            return false;
        }

        // タグの存在と権限チェック
        var tag = await _context.Tags
            .Where(t => t.TagId == tagId && t.UserId == userId)
            .FirstOrDefaultAsync();

        if (tag == null)
        {
            return false;
        }

        // 関連の削除
        var todoItemTag = await _context.TodoItemTags
            .Where(tt => tt.TodoItemId == todoItemId && tt.TagId == tagId)
            .FirstOrDefaultAsync();

        if (todoItemTag == null)
        {
            return false;
        }

        _context.TodoItemTags.Remove(todoItemTag);
        await _context.SaveChangesAsync();

        return true;
    }

    private static TagDto MapToDto(Tag tag)
    {
        return new TagDto
        {
            TagId = tag.TagId,
            UserId = tag.UserId,
            TagName = tag.TagName,
            ColorCode = tag.ColorCode,
            CreatedAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt,
            UsageCount = tag.TodoItemTags?.Count(tt => !tt.TodoItem.IsDeleted) ?? 0
        };
    }
}
