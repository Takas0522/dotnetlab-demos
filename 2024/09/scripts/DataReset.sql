DELETE FROM [dbo].[ToDo];
GO
INSERT INTO [dbo].[ToDo] ([Description],[Status])VALUES
('テストデータ未実行', 'incompleted'),
('テストデータ実行済', 'completed')