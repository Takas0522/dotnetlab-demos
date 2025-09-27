CREATE TABLE [dbo].[Users]
(
    [UserId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EntraId] NVARCHAR(255) NOT NULL UNIQUE, -- Azure Entra ID (旧Azure AD) の ObjectId
    [UserPrincipalName] NVARCHAR(255) NOT NULL, -- ユーザープリンシパル名 (UPN)
    [DisplayName] NVARCHAR(255) NOT NULL, -- 表示名
    [Email] NVARCHAR(255) NULL, -- メールアドレス
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1
);
GO

-- インデックス
CREATE INDEX [IX_Users_EntraId] ON [dbo].[Users] ([EntraId]);
GO
CREATE INDEX [IX_Users_UserPrincipalName] ON [dbo].[Users] ([UserPrincipalName]);
GO
CREATE INDEX [IX_Users_IsActive] ON [dbo].[Users] ([IsActive]);
GO
