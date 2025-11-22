# ToDo サービス (ToDo Service)

ToDo サービスはユーザーのタスク管理を担当します。各ユーザーの ToDo エントリを CRUD し、検索やフィルタ、状態変更（完了、アーカイブ）を提供します。独自の SQL Database を持ち、User Management のユーザー情報を参照/キャッシュします。

## 主な責務
- ToDo の作成、更新、削除、取得
- フィルタ（期日、状態、タグ）と検索
- 所有者の参照整合性を Change Feed 経由で保証

## API 契約（代表的）
- POST /api/todos
  - 入力: { title, description, dueDate, priority, tags[], ownerId }
  - 出力: { todoId }
- GET /api/todos/{id}
  - 出力: { todoId, title, description, dueDate, status, ownerId, createdAt }
- GET /api/todos?ownerId=...&status=...&dueBefore=...
  - 出力: ページネーションされた ToDo のリスト
- PUT /api/todos/{id}
  - 入力: 更新フィールド（status の変更等）
- DELETE /api/todos/{id}
  - 出力: 204

## DB スキーマ（概略）
- Todos
  - id (PK), ownerId, title, description, dueDate, status (open/completed/archived), priority, tags (json), createdAt, updatedAt

## Change Feed イベント
- todo:created / todo:updated / todo:deleted
  - payload: { todoId, ownerId, summaryFields..., timestamp, version }

## ユーザー参照と同期
- ownerId をキーに User Management の user:updated イベントを受け取り、表示名やメール等のローカルコピーを更新する。
- ユーザーが削除された場合は ownerId を null 化するか、アーカイブポリシーを適用する。

## 検索・インデックス戦略
- SQL のインデックスを ownerId, status, dueDate に張る。
- タグ検索は JSON 列または外部の全文検索（ElasticSearch）にオフロードを検討。

## セキュリティ
- API は認証・認可を必須とし、ユーザーは自分の ToDo に対する操作のみ許可する（管理者は例外）。

---

作成者: GitHub Copilot デモ用アウトライン
