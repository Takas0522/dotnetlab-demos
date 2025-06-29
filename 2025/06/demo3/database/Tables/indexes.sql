-- インデックス：ToDoテーブル用
CREATE NONCLUSTERED INDEX [IX_todos_user_id_status] ON [dbo].[todos]([user_id], [status]) 
INCLUDE ([title], [priority], [due_date], [created_at]);
GO
CREATE NONCLUSTERED INDEX [IX_todos_due_date] ON [dbo].[todos]([due_date]) 
WHERE [due_date] IS NOT NULL AND [is_deleted] = 0;
GO
CREATE NONCLUSTERED INDEX [IX_todos_category_id] ON [dbo].[todos]([category_id]) 
WHERE [category_id] IS NOT NULL;
GO
-- インデックス：カテゴリテーブル用
CREATE NONCLUSTERED INDEX [IX_categories_user_id] ON [dbo].[categories]([user_id]) 
WHERE [is_active] = 1;
GO
-- インデックス：添付ファイルテーブル用
CREATE NONCLUSTERED INDEX [IX_todo_attachments_todo_id] ON [dbo].[todo_attachments]([todo_id]);
GO