-- ToDoタスクにタグ情報を含めたビュー
CREATE VIEW [dbo].[vw_TodoItemsWithTags]
AS
SELECT 
    t.TodoItemId,
    t.UserId,
    u.DisplayName as UserDisplayName,
    t.Title,
    t.Description,
    t.IsCompleted,
    t.Priority,
    t.DueDate,
    t.CompletedAt,
    t.CreatedAt,
    t.UpdatedAt,
    -- タグ情報をJSON形式で集約
    (
        SELECT 
            tag.TagId,
            tag.TagName,
            tag.ColorCode
        FROM [dbo].[TodoItemTags] tt
        INNER JOIN [dbo].[Tags] tag ON tt.TagId = tag.TagId
        WHERE tt.TodoItemId = t.TodoItemId 
            AND tag.IsDeleted = 0
        FOR JSON PATH
    ) as TagsJson,
    -- タグ名をカンマ区切りで集約
    STUFF((
        SELECT ', ' + tag.TagName
        FROM [dbo].[TodoItemTags] tt
        INNER JOIN [dbo].[Tags] tag ON tt.TagId = tag.TagId
        WHERE tt.TodoItemId = t.TodoItemId 
            AND tag.IsDeleted = 0
        ORDER BY tag.TagName
        FOR XML PATH('')
    ), 1, 2, '') as TagNames
FROM [dbo].[TodoItems] t
INNER JOIN [dbo].[Users] u ON t.UserId = u.UserId
WHERE t.IsDeleted = 0;
GO
