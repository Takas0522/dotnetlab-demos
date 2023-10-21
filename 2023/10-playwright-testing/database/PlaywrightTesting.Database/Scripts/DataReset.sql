DELETE FROM [ToDo];
GO
INSERT INTO [ToDo] ([Description],[Status])VALUES
('テストデータ未実行', 'incompleted'),
('テストデータ実行済', 'completed')