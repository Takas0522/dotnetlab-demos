using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoDto>> GetTodosByUserIdAsync(int userId, int? categoryId = null, byte? status = null);
        Task<TodoDto?> GetTodoByIdAsync(int id, int userId);
        Task<TodoDto> CreateTodoAsync(CreateTodoDto createTodoDto);
        Task<TodoDto?> UpdateTodoAsync(int id, int userId, UpdateTodoDto updateTodoDto);
        Task<bool> DeleteTodoAsync(int id, int userId);
        Task<bool> UpdateTodoStatusAsync(int id, int userId, byte status);
    }
}
