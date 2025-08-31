CREATE TABLE [dbo].[Tags]
(
    [TagId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL, -- タグ作成者のユーザーID
    [TagName] NVARCHAR(100) NOT NULL, -- タグ名
    [ColorCode] NVARCHAR(7) NULL, -- タグの色コード (例: #FF5733)
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0, -- 論理削除フラグ
    
    CONSTRAINT [FK_Tags_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [UQ_Tags_UserId_TagName] UNIQUE ([UserId], [TagName]) -- 同一ユーザー内でのタグ名重複防止
);
GO

-- インデックス
CREATE INDEX [IX_Tags_UserId] ON [dbo].[Tags] ([UserId]);
GO
CREATE INDEX [IX_Tags_IsDeleted] ON [dbo].[Tags] ([IsDeleted]);
GO
CREATE INDEX [IX_Tags_UserId_IsDeleted] ON [dbo].[Tags] ([UserId], [IsDeleted]);
GO
