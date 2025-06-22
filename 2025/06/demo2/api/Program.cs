var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ToDoタスク管理用のキャッシュ
var todoItems = new List<TodoItem>();
var nextId = 1;

// 一覧取得
app.MapGet("/todos", () => todoItems);

// 追加
app.MapPost("/todos", (string title) => {
    var todo = new TodoItem(nextId++, title, false);
    todoItems.Add(todo);
    return Results.Created($"/todos/{todo.Id}", todo);
});

// 更新
app.MapPut("/todos/{id}", (int id, string title, bool isCompleted) => {
    var index = todoItems.FindIndex(t => t.Id == id);
    if (index == -1) return Results.NotFound();
    var updated = new TodoItem(id, title, isCompleted);
    todoItems[index] = updated;
    return Results.Ok(updated);
});

// 削除
app.MapDelete("/todos/{id}", (int id) => {
    var index = todoItems.FindIndex(t => t.Id == id);
    if (index == -1) return Results.NotFound();
    todoItems.RemoveAt(index);
    return Results.NoContent();
});

app.Run();

record TodoItem(int Id, string Title, bool IsCompleted);
