-- ユーザーのToDoタスク一覧を取得するビュー（自分のタスクと共有されたタスクを含む）
CREATE VIEW [dbo].[vw_UserTodoItems]
AS
SELECT 
    t.TodoItemId,
    t.UserId as OwnerUserId,
    u_owner.DisplayName as OwnerDisplayName,
    t.Title,
    t.Description,
    t.IsCompleted,
    t.Priority,
    t.DueDate,
    t.CompletedAt,
    t.CreatedAt,
    t.UpdatedAt,
    'Owner' as AccessType,
    'ReadWrite' as Permission,
    t.UserId as AccessUserId
FROM [dbo].[TodoItems] t
INNER JOIN [dbo].[Users] u_owner ON t.UserId = u_owner.UserId
WHERE t.IsDeleted = 0

UNION ALL

SELECT 
    t.TodoItemId,
    t.UserId as OwnerUserId,
    u_owner.DisplayName as OwnerDisplayName,
    t.Title,
    t.Description,
    t.IsCompleted,
    t.Priority,
    t.DueDate,
    t.CompletedAt,
    t.CreatedAt,
    t.UpdatedAt,
    'Shared' as AccessType,
    s.Permission,
    s.SharedUserId as AccessUserId
FROM [dbo].[TodoItems] t
INNER JOIN [dbo].[Users] u_owner ON t.UserId = u_owner.UserId
INNER JOIN [dbo].[TodoItemShares] s ON t.TodoItemId = s.TodoItemId
WHERE t.IsDeleted = 0 
    AND s.IsActive = 1;
GO
