CREATE TABLE [dbo].[TodoItemTags]
(
    [TodoItemTagId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [TodoItemId] UNIQUEIDENTIFIER NOT NULL,
    [TagId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [FK_TodoItemTags_TodoItems] FOREIGN KEY ([TodoItemId]) REFERENCES [dbo].[TodoItems]([TodoItemId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TodoItemTags_Tags] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tags]([TagId]) ON DELETE CASCADE,
    CONSTRAINT [UQ_TodoItemTags_TodoItemId_TagId] UNIQUE ([TodoItemId], [TagId]) -- 同一タスクに同じタグの重複付与防止
);
GO

-- インデックス
CREATE INDEX [IX_TodoItemTags_TodoItemId] ON [dbo].[TodoItemTags] ([TodoItemId]);
GO
CREATE INDEX [IX_TodoItemTags_TagId] ON [dbo].[TodoItemTags] ([TagId]);
GO
