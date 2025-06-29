using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// 指定ユーザーのカテゴリ一覧を取得
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesByUserId(int userId)
        {
            var categories = await _categoryService.GetCategoriesByUserIdAsync(userId);
            return Ok(categories);
        }

        /// <summary>
        /// 指定IDのカテゴリを取得
        /// </summary>
        [HttpGet("{id}/user/{userId}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id, int userId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id, userId);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// 新しいカテゴリを作成
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id, userId = category.UserId }, category);
        }

        /// <summary>
        /// カテゴリを更新
        /// </summary>
        [HttpPut("{id}/user/{userId}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, int userId, UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.UpdateCategoryAsync(id, userId, updateCategoryDto);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// カテゴリを削除（論理削除）
        /// </summary>
        [HttpDelete("{id}/user/{userId}")]
        public async Task<IActionResult> DeleteCategory(int id, int userId)
        {
            var result = await _categoryService.DeleteCategoryAsync(id, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
