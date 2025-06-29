using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        /// <summary>
        /// 指定ユーザーのTodo一覧を取得
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodosByUserId(
            int userId, 
            [FromQuery] int? categoryId = null, 
            [FromQuery] byte? status = null)
        {
            var todos = await _todoService.GetTodosByUserIdAsync(userId, categoryId, status);
            return Ok(todos);
        }

        /// <summary>
        /// 指定IDのTodoを取得
        /// </summary>
        [HttpGet("{id}/user/{userId}")]
        public async Task<ActionResult<TodoDto>> GetTodo(int id, int userId)
        {
            var todo = await _todoService.GetTodoByIdAsync(id, userId);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        /// <summary>
        /// 新しいTodoを作成
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TodoDto>> CreateTodo(CreateTodoDto createTodoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var todo = await _todoService.CreateTodoAsync(createTodoDto);
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id, userId = todo.UserId }, todo);
        }

        /// <summary>
        /// Todoを更新
        /// </summary>
        [HttpPut("{id}/user/{userId}")]
        public async Task<ActionResult<TodoDto>> UpdateTodo(int id, int userId, UpdateTodoDto updateTodoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var todo = await _todoService.UpdateTodoAsync(id, userId, updateTodoDto);
            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        /// <summary>
        /// Todoを削除（論理削除）
        /// </summary>
        [HttpDelete("{id}/user/{userId}")]
        public async Task<IActionResult> DeleteTodo(int id, int userId)
        {
            var result = await _todoService.DeleteTodoAsync(id, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Todoのステータスを更新
        /// </summary>
        [HttpPatch("{id}/user/{userId}/status")]
        public async Task<IActionResult> UpdateTodoStatus(int id, int userId, [FromBody] byte status)
        {
            if (status < 1 || status > 5)
                return BadRequest("Status must be between 1 and 5");

            var result = await _todoService.UpdateTodoStatusAsync(id, userId, status);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
