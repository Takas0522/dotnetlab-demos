CREATE PROCEDURE [dbo].[sp_GetUserTodos]
    @UserId INT,
    @Status TINYINT = NULL,
    @CategoryId INT = NULL,
    @Priority TINYINT = NULL,
    @DueDateFrom DATETIME2 = NULL,
    @DueDateTo DATETIME2 = NULL,
    @SearchText NVARCHAR(200) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    -- オフセット計算
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- 結果の取得
    SELECT 
        id,
        title,
        description,
        priority,
        priority_text,
        status,
        status_text,
        due_date,
        completed_at,
        created_at,
        updated_at,
        category_name,
        category_color,
        is_overdue,
        is_due_today,
        attachment_count
    FROM dbo.vw_todos_details
    WHERE username = (SELECT username FROM dbo.users WHERE id = @UserId)
        AND (@Status IS NULL OR status = @Status)
        AND (@CategoryId IS NULL OR category_id = @CategoryId)
        AND (@Priority IS NULL OR priority = @Priority)
        AND (@DueDateFrom IS NULL OR due_date >= @DueDateFrom)
        AND (@DueDateTo IS NULL OR due_date <= @DueDateTo)
        AND (@SearchText IS NULL OR title LIKE '%' + @SearchText + '%' OR description LIKE '%' + @SearchText + '%')
    ORDER BY 
        CASE WHEN status IN (1, 2) THEN 0 ELSE 1 END, -- アクティブなタスクを優先
        priority DESC, -- 優先度順
        due_date ASC, -- 期限順
        created_at ASC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- 総件数の取得
    SELECT COUNT(*) AS TotalCount
    FROM dbo.vw_todos_details
    WHERE username = (SELECT username FROM dbo.users WHERE id = @UserId)
        AND (@Status IS NULL OR status = @Status)
        AND (@CategoryId IS NULL OR category_id = @CategoryId)
        AND (@Priority IS NULL OR priority = @Priority)
        AND (@DueDateFrom IS NULL OR due_date >= @DueDateFrom)
        AND (@DueDateTo IS NULL OR due_date <= @DueDateTo)
        AND (@SearchText IS NULL OR title LIKE '%' + @SearchText + '%' OR description LIKE '%' + @SearchText + '%');
END;
