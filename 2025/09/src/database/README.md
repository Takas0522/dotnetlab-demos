# ToDoアプリケーション データベース設計

## 概要
ToDoアプリケーションのデータベース設計です。Azure Entra ID認証に対応し、タスク管理、タグ付け、共有機能を提供します。

## データベース構成

### テーブル設計

#### 1. Users テーブル
Azure Entra IDと連携するユーザー情報テーブル
- `UserId`: 内部ユーザーID (UNIQUEIDENTIFIER, PK)
- `EntraId`: Azure Entra IDのObjectId (NVARCHAR(255), UNIQUE)
- `UserPrincipalName`: ユーザープリンシパル名 (NVARCHAR(255))
- `DisplayName`: 表示名 (NVARCHAR(255))
- `Email`: メールアドレス (NVARCHAR(255))
- `CreatedAt`: 作成日時 (DATETIME2(7))
- `UpdatedAt`: 更新日時 (DATETIME2(7))
- `IsActive`: アクティブフラグ (BIT)

#### 2. TodoItems テーブル
ToDoタスクの本体情報
- `TodoItemId`: タスクID (UNIQUEIDENTIFIER, PK)
- `UserId`: 作成者のユーザーID (UNIQUEIDENTIFIER, FK)
- `Title`: タスクのタイトル (NVARCHAR(500))
- `Description`: タスクの詳細説明 (NVARCHAR(MAX))
- `IsCompleted`: 完了フラグ (BIT)
- `Priority`: 優先度 1:低 2:中 3:高 (INT)
- `DueDate`: 期限日時 (DATETIME2(7))
- `CompletedAt`: 完了日時 (DATETIME2(7))
- `CreatedAt`: 作成日時 (DATETIME2(7))
- `UpdatedAt`: 更新日時 (DATETIME2(7))
- `IsDeleted`: 論理削除フラグ (BIT)

#### 3. Tags テーブル
タグマスター情報
- `TagId`: タグID (UNIQUEIDENTIFIER, PK)
- `UserId`: タグ作成者のユーザーID (UNIQUEIDENTIFIER, FK)
- `TagName`: タグ名 (NVARCHAR(100))
- `ColorCode`: タグの色コード (NVARCHAR(7))
- `CreatedAt`: 作成日時 (DATETIME2(7))
- `UpdatedAt`: 更新日時 (DATETIME2(7))
- `IsDeleted`: 論理削除フラグ (BIT)

#### 4. TodoItemTags テーブル
タスクとタグの多対多関連テーブル
- `TodoItemTagId`: 関連ID (UNIQUEIDENTIFIER, PK)
- `TodoItemId`: タスクID (UNIQUEIDENTIFIER, FK)
- `TagId`: タグID (UNIQUEIDENTIFIER, FK)
- `CreatedAt`: 作成日時 (DATETIME2(7))

#### 5. TodoItemShares テーブル
タスク共有情報
- `TodoItemShareId`: 共有ID (UNIQUEIDENTIFIER, PK)
- `TodoItemId`: タスクID (UNIQUEIDENTIFIER, FK)
- `OwnerUserId`: 共有元ユーザーID (UNIQUEIDENTIFIER, FK)
- `SharedUserId`: 共有先ユーザーID (UNIQUEIDENTIFIER, FK)
- `Permission`: 権限 ('ReadOnly', 'ReadWrite') (NVARCHAR(20))
- `SharedAt`: 共有日時 (DATETIME2(7))
- `IsActive`: 共有有効フラグ (BIT)

### ビュー

#### 1. vw_UserTodoItems
ユーザーがアクセス可能なすべてのToDoタスクを表示（自分のタスク + 共有されたタスク）

#### 2. vw_TodoItemsWithTags
ToDoタスクにタグ情報を含めた統合ビュー（JSON形式とカンマ区切り形式の両方でタグ情報を提供）

### インデックス設計

#### パフォーマンス最適化のためのインデックス
- ユーザーIDベースの検索
- 完了状態での絞り込み
- 削除フラグでの絞り込み
- 期限日での並び替え
- 共有状態での検索

## 主要機能への対応

### ✅ 実装済み機能
1. **ユーザー認証**: Azure Entra ID対応
2. **ToDoの追加・削除**: TodoItemsテーブルで管理（論理削除）
3. **複数選択削除**: バッチ更新でIsDeletedフラグを更新
4. **タグ付け**: TagsとTodoItemTagsテーブルで多対多関連
5. **ToDoの編集**: TodoItemsテーブルの更新
6. **タスク完了**: IsCompletedフラグとCompletedAt日時
7. **タスク一覧フィルタ**: IsCompletedフラグでの絞り込み
8. **タスク共有**: TodoItemSharesテーブルで権限管理

### データ整合性の保証
- 外部キー制約によるリレーションシップの保証
- ユニーク制約による重複データの防止
- チェック制約による不正データの防止
- 論理削除による履歴保持

### セキュリティ考慮事項
- Azure Entra IDとの連携による認証
- ユーザー権限に基づくデータアクセス制御
- 共有権限の細分化（ReadOnly/ReadWrite）
- 論理削除による誤削除からの復旧可能性

## デプロイメント手順

1. SQLプロジェクトのビルド:
   ```bash
   dotnet build
   ```

2. データベース作成とスキーマ適用:
   ```bash
   sqlcmd -S [ServerName] -d [DatabaseName] -i Tables\*.sql
   sqlcmd -S [ServerName] -d [DatabaseName] -i Views\*.sql
   ```

3. 初期データ投入（開発環境のみ）:
   ```bash
   sqlcmd -S [ServerName] -d [DatabaseName] -i Scripts\SeedData.sql
   ```

## 今後の拡張可能性

1. **タスクの階層化**: 親タスク・子タスクの関係
2. **添付ファイル**: ファイル情報テーブルの追加
3. **コメント機能**: タスクコメントテーブルの追加
4. **通知機能**: 通知設定テーブルの追加
5. **監査ログ**: 操作履歴テーブルの追加
