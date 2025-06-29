CREATE TABLE [dbo].[todos] (
    [id] INT IDENTITY(1,1) NOT NULL,
    [user_id] INT NOT NULL,
    [category_id] INT NULL,
    [title] NVARCHAR(200) NOT NULL,
    [description] NVARCHAR(MAX) NULL,
    [priority] TINYINT NOT NULL DEFAULT 1, -- 1:低, 2:中, 3:高, 4:緊急
    [status] TINYINT NOT NULL DEFAULT 1, -- 1:未完了, 2:進行中, 3:完了, 4:保留, 5:キャンセル
    [due_date] DATETIME2 NULL,
    [completed_at] DATETIME2 NULL,
    [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [updated_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [is_deleted] BIT NOT NULL DEFAULT 0,
    
    CONSTRAINT [PK_todos] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [CK_todos_priority] CHECK ([priority] BETWEEN 1 AND 4),
    CONSTRAINT [CK_todos_status] CHECK ([status] BETWEEN 1 AND 5)
);
