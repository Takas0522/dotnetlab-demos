-- 初期データの投入（開発・テスト用）
-- 注意: プロダクション環境では使用しないこと

-- サンプルユーザーの作成
INSERT INTO [dbo].[Users] (EntraId, UserPrincipalName, DisplayName, Email)
VALUES 
    ('00000000-0000-0000-0000-000000000001', 'user1@example.com', 'テストユーザー1', 'user1@example.com'),
    ('00000000-0000-0000-0000-000000000002', 'user2@example.com', 'テストユーザー2', 'user2@example.com'),
    ('00000000-0000-0000-0000-000000000003', 'user3@example.com', 'テストユーザー3', 'user3@example.com');
GO

-- ユーザーIDを取得
DECLARE @User1Id UNIQUEIDENTIFIER = (SELECT UserId FROM [dbo].[Users] WHERE EntraId = '00000000-0000-0000-0000-000000000001');
DECLARE @User2Id UNIQUEIDENTIFIER = (SELECT UserId FROM [dbo].[Users] WHERE EntraId = '00000000-0000-0000-0000-000000000002');
DECLARE @User3Id UNIQUEIDENTIFIER = (SELECT UserId FROM [dbo].[Users] WHERE EntraId = '00000000-0000-0000-0000-000000000003');

-- サンプルタグの作成
INSERT INTO [dbo].[Tags] (UserId, TagName, ColorCode)
VALUES 
    (@User1Id, '重要', '#FF5733'),
    (@User1Id, '仕事', '#3366FF'),
    (@User1Id, '個人', '#33FF66'),
    (@User2Id, 'プロジェクトA', '#FF33FF'),
    (@User2Id, '緊急', '#FF0000');
GO

-- サンプルToDoタスクの作成
DECLARE @Task1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task4Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[TodoItems] (TodoItemId, UserId, Title, Description, Priority, DueDate)
VALUES 
    (@Task1Id, @User1Id, '月次レポート作成', '2025年8月の月次レポートを作成する', 2, DATEADD(day, 7, GETUTCDATE())),
    (@Task2Id, @User1Id, 'プレゼン資料準備', '来週の会議用プレゼン資料を準備', 3, DATEADD(day, 3, GETUTCDATE())),
    (@Task3Id, @User2Id, 'データベース設計見直し', 'ToDoアプリのDB設計を最終確認', 2, DATEADD(day, 5, GETUTCDATE())),
    (@Task4Id, @User1Id, '買い物リスト', '週末の買い物リストを作成', 1, DATEADD(day, 2, GETUTCDATE()));
GO

-- タグとタスクの関連付け
DECLARE @ImportantTagId UNIQUEIDENTIFIER = (SELECT TagId FROM [dbo].[Tags] WHERE UserId = @User1Id AND TagName = '重要');
DECLARE @WorkTagId UNIQUEIDENTIFIER = (SELECT TagId FROM [dbo].[Tags] WHERE UserId = @User1Id AND TagName = '仕事');
DECLARE @PersonalTagId UNIQUEIDENTIFIER = (SELECT TagId FROM [dbo].[Tags] WHERE UserId = @User1Id AND TagName = '個人');
DECLARE @ProjectATagId UNIQUEIDENTIFIER = (SELECT TagId FROM [dbo].[Tags] WHERE UserId = @User2Id AND TagName = 'プロジェクトA');

INSERT INTO [dbo].[TodoItemTags] (TodoItemId, TagId)
VALUES 
    (@Task1Id, @WorkTagId),
    (@Task1Id, @ImportantTagId),
    (@Task2Id, @WorkTagId),
    (@Task3Id, @ProjectATagId),
    (@Task4Id, @PersonalTagId);
GO

-- タスク共有の例
INSERT INTO [dbo].[TodoItemShares] (TodoItemId, OwnerUserId, SharedUserId, Permission)
VALUES 
    (@Task1Id, @User1Id, @User2Id, 'ReadOnly'), -- User1のタスクをUser2に読み取り専用で共有
    (@Task3Id, @User2Id, @User1Id, 'ReadWrite'); -- User2のタスクをUser1に読み書き可能で共有
GO
