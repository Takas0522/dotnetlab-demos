using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly TodoDbContext _context;

        public CategoryService(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesByUserIdAsync(int userId)
        {
            var categories = await _context.Categories
                .Include(c => c.Todos.Where(t => !t.IsDeleted))
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id, int userId)
        {
            var category = await _context.Categories
                .Include(c => c.Todos.Where(t => !t.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.IsActive);

            return category != null ? MapToDto(category) : null;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                UserId = createCategoryDto.UserId,
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description,
                Color = createCategoryDto.Color,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, int userId, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _context.Categories
                .Include(c => c.Todos.Where(t => !t.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.IsActive);

            if (category == null)
                return null;

            if (!string.IsNullOrEmpty(updateCategoryDto.Name))
                category.Name = updateCategoryDto.Name;
            if (updateCategoryDto.Description != null)
                category.Description = updateCategoryDto.Description;
            if (updateCategoryDto.Color != null)
                category.Color = updateCategoryDto.Color;

            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id, int userId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.IsActive);

            if (category == null)
                return false;

            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                UserId = category.UserId,
                Name = category.Name,
                Description = category.Description,
                Color = category.Color,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                TodoCount = category.Todos?.Count ?? 0
            };
        }
    }
}
