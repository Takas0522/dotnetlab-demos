CREATE PROCEDURE [dbo].[sp_UpdateTodoStatus]
    @TodoId INT,
    @UserId INT,
    @NewStatus TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- バリデーション
        IF NOT EXISTS (SELECT 1 FROM dbo.todos WHERE id = @TodoId AND user_id = @UserId AND is_deleted = 0)
        BEGIN
            RAISERROR('指定されたToDoが存在しないか、このユーザーに属していません。', 16, 1);
            RETURN;
        END;
        
        IF @NewStatus NOT BETWEEN 1 AND 5
        BEGIN
            RAISERROR('ステータスは1から5の間で指定してください。', 16, 1);
            RETURN;
        END;
        
        DECLARE @CurrentStatus TINYINT;
        SELECT @CurrentStatus = status FROM dbo.todos WHERE id = @TodoId;
        
        -- ステータスの更新
        UPDATE dbo.todos 
        SET 
            status = @NewStatus,
            completed_at = CASE WHEN @NewStatus = 3 THEN GETUTCDATE() ELSE NULL END,
            updated_at = GETUTCDATE()
        WHERE id = @TodoId;
        
        COMMIT TRANSACTION;
        
        -- 更新されたToDoの詳細を返す
        SELECT * FROM dbo.vw_todos_details WHERE id = @TodoId;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH;
END;
