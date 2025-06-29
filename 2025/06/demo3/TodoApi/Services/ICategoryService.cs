using TodoApi.DTOs;

namespace TodoApi.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesByUserIdAsync(int userId);
        Task<CategoryDto?> GetCategoryByIdAsync(int id, int userId);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto?> UpdateCategoryAsync(int id, int userId, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int id, int userId);
    }
}
