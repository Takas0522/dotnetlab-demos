using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoDbContext _context;

        public TodoService(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoDto>> GetTodosByUserIdAsync(int userId, int? categoryId = null, byte? status = null)
        {
            var query = _context.Todos
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && !t.IsDeleted);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            var todos = await query
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();

            return todos.Select(MapToDto);
        }

        public async Task<TodoDto?> GetTodoByIdAsync(int id, int userId)
        {
            var todo = await _context.Todos
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && !t.IsDeleted);

            return todo != null ? MapToDto(todo) : null;
        }

        public async Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto)
        {
            var todo = new Todo
            {
                UserId = createTodoDto.UserId,
                CategoryId = createTodoDto.CategoryId,
                Title = createTodoDto.Title,
                Description = createTodoDto.Description,
                Priority = createTodoDto.Priority,
                DueDate = createTodoDto.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            // Reload with category for DTO mapping
            await _context.Entry(todo)
                .Reference(t => t.Category)
                .LoadAsync();

            return MapToDto(todo);
        }

        public async Task<TodoDto?> UpdateTodoAsync(int id, int userId, UpdateTodoDto updateTodoDto)
        {
            var todo = await _context.Todos
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && !t.IsDeleted);

            if (todo == null)
                return null;

            if (updateTodoDto.CategoryId.HasValue)
                todo.CategoryId = updateTodoDto.CategoryId;
            if (!string.IsNullOrEmpty(updateTodoDto.Title))
                todo.Title = updateTodoDto.Title;
            if (updateTodoDto.Description != null)
                todo.Description = updateTodoDto.Description;
            if (updateTodoDto.Priority.HasValue)
                todo.Priority = updateTodoDto.Priority.Value;
            if (updateTodoDto.Status.HasValue)
            {
                todo.Status = updateTodoDto.Status.Value;
                if (updateTodoDto.Status.Value == 3) // 完了
                    todo.CompletedAt = DateTime.UtcNow;
                else if (todo.CompletedAt.HasValue)
                    todo.CompletedAt = null;
            }
            if (updateTodoDto.DueDate.HasValue)
                todo.DueDate = updateTodoDto.DueDate;

            todo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(todo);
        }

        public async Task<bool> DeleteTodoAsync(int id, int userId)
        {
            var todo = await _context.Todos
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && !t.IsDeleted);

            if (todo == null)
                return false;

            todo.IsDeleted = true;
            todo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTodoStatusAsync(int id, int userId, byte status)
        {
            var todo = await _context.Todos
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && !t.IsDeleted);

            if (todo == null)
                return false;

            todo.Status = status;
            if (status == 3) // 完了
                todo.CompletedAt = DateTime.UtcNow;
            else if (todo.CompletedAt.HasValue)
                todo.CompletedAt = null;

            todo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static TodoDto MapToDto(Todo todo)
        {
            return new TodoDto
            {
                Id = todo.Id,
                UserId = todo.UserId,
                CategoryId = todo.CategoryId,
                Title = todo.Title,
                Description = todo.Description,
                Priority = todo.Priority,
                Status = todo.Status,
                DueDate = todo.DueDate,
                CompletedAt = todo.CompletedAt,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt,
                CategoryName = todo.Category?.Name,
                CategoryColor = todo.Category?.Color
            };
        }
    }
}
