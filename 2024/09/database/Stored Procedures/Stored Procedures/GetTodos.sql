CREATE PROCEDURE [dbo].[GetTodos]
AS
    SELECT
        [Id],
        [Description],
        [Status],
        [AddDate],
        [UpdateDate]
    FROM
        [dbo].[ToDo]
