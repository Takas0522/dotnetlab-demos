CREATE TABLE [dbo].[TodoItems]
(
    [TodoItemId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL, -- 作成者のユーザーID
    [Title] NVARCHAR(500) NOT NULL, -- タスクのタイトル
    [Description] NVARCHAR(MAX) NULL, -- タスクの詳細説明
    [IsCompleted] BIT NOT NULL DEFAULT 0, -- 完了フラグ
    [Priority] INT NOT NULL DEFAULT 1, -- 優先度 (1: 低, 2: 中, 3: 高)
    [DueDate] DATETIME2(7) NULL, -- 期限日時
    [CompletedAt] DATETIME2(7) NULL, -- 完了日時
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0, -- 論理削除フラグ
    
    CONSTRAINT [FK_TodoItems_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
);
GO

-- インデックス
CREATE INDEX [IX_TodoItems_UserId] ON [dbo].[TodoItems] ([UserId]);
GO
CREATE INDEX [IX_TodoItems_IsCompleted] ON [dbo].[TodoItems] ([IsCompleted]);
GO
CREATE INDEX [IX_TodoItems_IsDeleted] ON [dbo].[TodoItems] ([IsDeleted]);
GO
CREATE INDEX [IX_TodoItems_DueDate] ON [dbo].[TodoItems] ([DueDate]);
GO
CREATE INDEX [IX_TodoItems_CreatedAt] ON [dbo].[TodoItems] ([CreatedAt]);
GO

-- 複合インデックス（よく使われるクエリパターン用）
CREATE INDEX [IX_TodoItems_UserId_IsCompleted_IsDeleted] ON [dbo].[TodoItems] ([UserId], [IsCompleted], [IsDeleted]);
GO
CREATE INDEX [IX_TodoItems_UserId_IsDeleted_CreatedAt] ON [dbo].[TodoItems] ([UserId], [IsDeleted], [CreatedAt]);
GO
