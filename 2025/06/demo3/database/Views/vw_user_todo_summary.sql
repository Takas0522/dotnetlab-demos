CREATE VIEW [dbo].[vw_user_todo_summary] AS
SELECT 
    u.id AS user_id,
    u.username,
    u.first_name,
    u.last_name,
    -- 全体統計
    COUNT(t.id) AS total_todos,
    SUM(CASE WHEN t.status = 3 THEN 1 ELSE 0 END) AS completed_todos,
    SUM(CASE WHEN t.status IN (1, 2) THEN 1 ELSE 0 END) AS active_todos,
    SUM(CASE WHEN t.status = 4 THEN 1 ELSE 0 END) AS on_hold_todos,
    -- 優先度別統計
    SUM(CASE WHEN t.priority = 4 AND t.status IN (1, 2) THEN 1 ELSE 0 END) AS urgent_todos,
    SUM(CASE WHEN t.priority = 3 AND t.status IN (1, 2) THEN 1 ELSE 0 END) AS high_priority_todos,
    -- 期限関連統計
    SUM(CASE WHEN t.due_date IS NOT NULL AND t.due_date < GETUTCDATE() AND t.status IN (1, 2) THEN 1 ELSE 0 END) AS overdue_todos,
    SUM(CASE WHEN t.due_date IS NOT NULL AND CAST(t.due_date AS DATE) = CAST(GETUTCDATE() AS DATE) AND t.status IN (1, 2) THEN 1 ELSE 0 END) AS due_today_todos,
    -- 完了率
    CASE 
        WHEN COUNT(t.id) > 0 
        THEN CAST(SUM(CASE WHEN t.status = 3 THEN 1 ELSE 0 END) AS FLOAT) / COUNT(t.id) * 100
        ELSE 0 
    END AS completion_rate
FROM dbo.users u
LEFT JOIN dbo.todos t ON u.id = t.user_id AND t.is_deleted = 0
WHERE u.is_active = 1
GROUP BY u.id, u.username, u.first_name, u.last_name;
