CREATE TABLE [dbo].[TodoItemShares]
(
    [TodoItemShareId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [TodoItemId] UNIQUEIDENTIFIER NOT NULL,
    [OwnerUserId] UNIQUEIDENTIFIER NOT NULL, -- 共有元ユーザー（タスクの所有者）
    [SharedUserId] UNIQUEIDENTIFIER NOT NULL, -- 共有先ユーザー
    [Permission] NVARCHAR(20) NOT NULL DEFAULT 'ReadOnly', -- 権限 (ReadOnly, ReadWrite)
    [SharedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1, -- 共有の有効/無効フラグ
    
    CONSTRAINT [FK_TodoItemShares_TodoItems] FOREIGN KEY ([TodoItemId]) REFERENCES [dbo].[TodoItems]([TodoItemId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TodoItemShares_OwnerUsers] FOREIGN KEY ([OwnerUserId]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [FK_TodoItemShares_SharedUsers] FOREIGN KEY ([SharedUserId]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [UQ_TodoItemShares_TodoItemId_SharedUserId] UNIQUE ([TodoItemId], [SharedUserId]), -- 同一タスクの同一ユーザーへの重複共有防止
    CONSTRAINT [CK_TodoItemShares_Permission] CHECK ([Permission] IN ('ReadOnly', 'ReadWrite')),
    CONSTRAINT [CK_TodoItemShares_DifferentUsers] CHECK ([OwnerUserId] != [SharedUserId]) -- 自分自身への共有を防止
);
GO

-- インデックス
CREATE INDEX [IX_TodoItemShares_TodoItemId] ON [dbo].[TodoItemShares] ([TodoItemId]);
GO
CREATE INDEX [IX_TodoItemShares_OwnerUserId] ON [dbo].[TodoItemShares] ([OwnerUserId]);
GO
CREATE INDEX [IX_TodoItemShares_SharedUserId] ON [dbo].[TodoItemShares] ([SharedUserId]);
GO
CREATE INDEX [IX_TodoItemShares_IsActive] ON [dbo].[TodoItemShares] ([IsActive]);
GO
CREATE INDEX [IX_TodoItemShares_SharedUserId_IsActive] ON [dbo].[TodoItemShares] ([SharedUserId], [IsActive]);
GO
