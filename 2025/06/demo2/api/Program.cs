var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

// ToDoタスク管理用のキャッシュ
var todoItems = new List<TodoItem>
{
    new TodoItem(1, "サンプルタスク1", false),
    new TodoItem(2, "サンプルタスク2", true),
    new TodoItem(3, "サンプルタスク3", false)
};
var nextId = todoItems.Max(t => t.Id) + 1;

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
