CREATE VIEW [dbo].[vw_todos_details] AS
SELECT 
    t.id,
    t.title,
    t.description,
    t.priority,
    CASE t.priority
        WHEN 1 THEN N'低'
        WHEN 2 THEN N'中'
        WHEN 3 THEN N'高'
        WHEN 4 THEN N'緊急'
    END AS priority_text,
    t.status,
    CASE t.status
        WHEN 1 THEN N'未完了'
        WHEN 2 THEN N'進行中'
        WHEN 3 THEN N'完了'
        WHEN 4 THEN N'保留'
        WHEN 5 THEN N'キャンセル'
    END AS status_text,
    t.due_date,
    t.completed_at,
    t.created_at,
    t.updated_at,
    t.category_id,
    u.username,
    u.first_name,
    u.last_name,
    c.name AS category_name,
    c.color AS category_color,
    -- 期限切れかどうか
    CASE 
        WHEN t.due_date IS NOT NULL AND t.due_date < GETUTCDATE() AND t.status IN (1, 2) 
        THEN 1 
        ELSE 0 
    END AS is_overdue,
    -- 今日期限かどうか
    CASE 
        WHEN t.due_date IS NOT NULL AND CAST(t.due_date AS DATE) = CAST(GETUTCDATE() AS DATE) AND t.status IN (1, 2)
        THEN 1 
        ELSE 0 
    END AS is_due_today,
    -- 添付ファイル数
    (SELECT COUNT(*) FROM dbo.todo_attachments ta WHERE ta.todo_id = t.id) AS attachment_count
FROM dbo.todos t
INNER JOIN dbo.users u ON t.user_id = u.id
LEFT JOIN dbo.categories c ON t.category_id = c.id
WHERE t.is_deleted = 0 AND u.is_active = 1;
