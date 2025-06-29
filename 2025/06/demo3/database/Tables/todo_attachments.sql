CREATE TABLE [dbo].[todo_attachments] (
    [id] INT IDENTITY(1,1) NOT NULL,
    [todo_id] INT NOT NULL,
    [file_name] NVARCHAR(255) NOT NULL,
    [file_path] NVARCHAR(500) NOT NULL,
    [file_size] BIGINT NOT NULL,
    [content_type] NVARCHAR(100) NOT NULL,
    [created_at] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_todo_attachments] PRIMARY KEY CLUSTERED ([id] ASC),
);
