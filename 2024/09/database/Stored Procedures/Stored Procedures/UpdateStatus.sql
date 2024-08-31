CREATE PROCEDURE [dbo].[UpdateStatus]
    @id int
AS

DECLARE @status nvarchar(15);
DECLARE @updateStatus nvarchar(15);

SELECT
    @status = [status]
FROM
    [dbo].[ToDo] T
WHERE
    T.Id = @id;
BEGIN
    IF @status = 'completed'
        BEGIN
            SET @updateStatus = 'incompleted'
        END
    ELSE
        BEGIN
            SET @updateStatus = 'completed'
        END
END

UPDATE [dbo].[ToDo] SET [Status] = @updateStatus Where [Id] = @id;
