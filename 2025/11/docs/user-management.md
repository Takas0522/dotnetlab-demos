# ユーザー管理サービス (User Management Service)

ユーザー管理サービスはユーザーの CRUD、プロファイル情報、権限・ロール管理を担当します。独立した ASP.NET Core WebAPI と SQL Database を持ち、他サービスは Change Feed を通じて必要に応じて参照・同期します。

## 主な責務
- ユーザー作成・更新・削除
- ユーザープロファイル（表示名、メール、アバター URL など）の管理
- 権限（ロール）管理: admin, user など
- イベント発行（Change Feed を介した user:created, user:updated など）

## API 契約（代表的）
- POST /api/users
  - 入力: { username, email, password, roles[] }
  - 出力: { userId }
- GET /api/users/{id}
  - 出力: { userId, username, email, profile... }
- PUT /api/users/{id}
  - 入力: 更新フィールド
- DELETE /api/users/{id}
  - 出力: 204

## DB スキーマ（概略）
- Users
  - id (PK), username, email, displayName, avatarUrl, createdAt, updatedAt, isActive
- Roles
  - id (PK), name, description
- UserRoles
  - userId, roleId

## Change Feed / イベント設計
- user:created
  - { userId, username, email, createdAt, version }
- user:updated
  - { userId, changedFields, timestamp, version }
- user:deleted
  - { userId, deletedAt, version }

イベント流通:
- Azure Functions が SQL の変更を検出して各ターゲットサービスへ配信する。
- ペイロードは最小限にして、受信サービスが必要に応じて User Management に問い合わせて補完できるようにする。

## キャッシュ戦略
- 他サービスはユーザーの表示名やメールなどの参照コピーを保持可。変更は Change Feed で補正。
- 一貫性要件が強い場合はリアルタイムで User Management に問い合わせる。

## セキュリティ
- ユーザー CRUD エンドポイントは認証済みおよび適切なロールによる制限を行う。
- 個人情報の取り扱いに注意し、ログに PII を残さない。

## 運用
- データ移行・マイグレーション時はユーザ ID の不整合に注意（UUID を採用することを推奨）。
- 大量更新に対してはバッチ処理と Change Feed のスロットリングを考慮。

---

作成者: GitHub Copilot デモ用アウトライン
