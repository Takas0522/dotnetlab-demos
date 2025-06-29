CREATE PROCEDURE [dbo].[sp_CreateTodo]
    @UserId INT,
    @CategoryId INT = NULL,
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX) = NULL,
    @Priority TINYINT = 1,
    @DueDate DATETIME2 = NULL,
    @TodoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- バリデーション
        IF NOT EXISTS (SELECT 1 FROM dbo.users WHERE id = @UserId AND is_active = 1)
        BEGIN
            RAISERROR('指定されたユーザーが存在しないか、無効です。', 16, 1);
            RETURN;
        END;
        
        IF @CategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.categories WHERE id = @CategoryId AND user_id = @UserId AND is_active = 1)
        BEGIN
            RAISERROR('指定されたカテゴリが存在しないか、このユーザーに属していません。', 16, 1);
            RETURN;
        END;
        
        IF @Priority NOT BETWEEN 1 AND 4
        BEGIN
            RAISERROR('優先度は1から4の間で指定してください。', 16, 1);
            RETURN;
        END;
        
        -- ToDoの作成
        INSERT INTO dbo.todos (user_id, category_id, title, description, priority, due_date)
        VALUES (@UserId, @CategoryId, @Title, @Description, @Priority, @DueDate);
        
        SET @TodoId = SCOPE_IDENTITY();
        
        COMMIT TRANSACTION;
        
        -- 作成されたToDoの詳細を返す
        SELECT * FROM dbo.vw_todos_details WHERE id = @TodoId;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH;
END;
