CREATE PROCEDURE [dbo].[AddTodo]
    @description nvarchar(255)
AS
    INSERT INTO [dbo].[ToDo]
    (
        [Description]
    ) VALUES (
        @description
    );

    SELECT
        T.[Id],
        T.[Description],
        T.[Status],
        T.[AddDate],
        T.[UpdateDate]
    FROM
        [dbo].[ToDo] T
    WHERE
        T.[Id] = @@IDENTITY;