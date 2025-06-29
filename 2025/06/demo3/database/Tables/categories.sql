CREATE TABLE [dbo].[categories] (
    [id] INT IDENTITY(1,1) NOT NULL,
    [user_id] INT NOT NULL,
    [name] NVARCHAR(100) NOT NULL,
    [description] NVARCHAR(500) NULL,
    [color] NVARCHAR(7) NULL, -- HEXカラーコード (#FF5733)
    [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [updated_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [is_active] BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_categories] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [UQ_categories_user_name] UNIQUE ([user_id], [name])
);
