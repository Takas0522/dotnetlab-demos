

-- ToDoアプリ用の初期データ
-- デモユーザーの作成
IF NOT EXISTS (SELECT 1 FROM dbo.users WHERE username = 'admin')
BEGIN
    INSERT INTO dbo.users (username, email, password_hash, first_name, last_name)
    VALUES ('admin', 'admin@example.com', 'hashed_password_here', N'管理', N'太郎');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.users WHERE username = 'demo')
BEGIN
    INSERT INTO dbo.users (username, email, password_hash, first_name, last_name)
    VALUES ('demo', 'demo@example.com', 'hashed_password_here', N'デモ', N'花子');
END;

-- デフォルトカテゴリの作成
DECLARE @AdminUserId INT = (SELECT id FROM dbo.users WHERE username = 'admin');
DECLARE @DemoUserId INT = (SELECT id FROM dbo.users WHERE username = 'demo');

IF @AdminUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.categories WHERE user_id = @AdminUserId AND name = N'仕事')
BEGIN
    INSERT INTO dbo.categories (user_id, name, description, color)
    VALUES (@AdminUserId, N'仕事', N'業務関連のタスク', '#FF5733');
END;

IF @AdminUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.categories WHERE user_id = @AdminUserId AND name = N'個人')
BEGIN
    INSERT INTO dbo.categories (user_id, name, description, color)
    VALUES (@AdminUserId, N'個人', N'個人的なタスク', '#33C4FF');
END;

IF @DemoUserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.categories WHERE user_id = @DemoUserId AND name = N'買い物')
BEGIN
    INSERT INTO dbo.categories (user_id, name, description, color)
    VALUES (@DemoUserId, N'買い物', N'購入する物のリスト', '#4CAF50');
END;

-- サンプルToDoの作成
DECLARE @WorkCategoryId INT = (SELECT id FROM dbo.categories WHERE user_id = @AdminUserId AND name = N'仕事');
DECLARE @PersonalCategoryId INT = (SELECT id FROM dbo.categories WHERE user_id = @AdminUserId AND name = N'個人');
DECLARE @ShoppingCategoryId INT = (SELECT id FROM dbo.categories WHERE user_id = @DemoUserId AND name = N'買い物');

IF @WorkCategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.todos WHERE user_id = @AdminUserId AND title = N'プロジェクト企画書作成')
BEGIN
    INSERT INTO dbo.todos (user_id, category_id, title, description, priority, due_date)
    VALUES (@AdminUserId, @WorkCategoryId, N'プロジェクト企画書作成', N'来週の会議用の企画書を作成する', 3, DATEADD(day, 3, GETUTCDATE()));
END;

IF @PersonalCategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.todos WHERE user_id = @AdminUserId AND title = N'健康診断の予約')
BEGIN
    INSERT INTO dbo.todos (user_id, category_id, title, description, priority, due_date)
    VALUES (@AdminUserId, @PersonalCategoryId, N'健康診断の予約', N'年次健康診断の予約を取る', 2, DATEADD(day, 7, GETUTCDATE()));
END;

IF @ShoppingCategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.todos WHERE user_id = @DemoUserId AND title = N'牛乳を買う')
BEGIN
    INSERT INTO dbo.todos (user_id, category_id, title, description, priority)
    VALUES (@DemoUserId, @ShoppingCategoryId, N'牛乳を買う', N'低脂肪乳を1リットル', 1);
END;
GO
