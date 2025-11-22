# 認証サービス (Authentication Service)

このドキュメントは認証サービスの設計仕様をまとめたものです。サービスは ASP.NET Core WebAPI (C#) で実装され、独自の SQL Database を保持します。主に OAuth2 / JWT によるトークン発行・検証を担います。

## 主な責務
- ユーザー認証 (パスワード認証、外部 IdP のフェデレーション)
- JWT アクセストークン / リフレッシュトークンの発行と管理
- パスワードリセット、メール確認、MFA（オプション）
- サービス間トークン検証エンドポイント（introspect）

## 技術スタック
- ASP.NET Core WebAPI (C# latest)
- SQL Database (ユーザー認証データ、リフレッシュトークン保存)
- OpenIddict または IdentityServer 相当の軽量実装を想定
- Azure Key Vault（シークレット、署名鍵の保管）

## API 契約（代表的）
- POST /api/auth/login
  - 入力: { username, password }
  - 出力: { accessToken, refreshToken, expiresIn }
  - 説明: 資格情報を検証し、JWT を返す。
- POST /api/auth/refresh
  - 入力: { refreshToken }
  - 出力: { accessToken, refreshToken, expiresIn }
  - 説明: リフレッシュトークンから新しいアクセストークンを発行。
- POST /api/auth/logout
  - 入力: { refreshToken }
  - 出力: 204 No Content
  - 説明: リフレッシュトークンを無効化する。
- GET /api/auth/introspect
  - 入力: accessToken の検査用ヘッダまたはクエリ
  - 出力: { active, claims... }
  - 説明: 他サービスがトークンの状態を確認するために使用。

## DB スキーマ（概略）
- Users table
  - id (PK), username, email, passwordHash, salt, createdAt, updatedAt, isEmailConfirmed
- RefreshTokens table
  - id (PK), userId (FK), tokenHash, issuedAt, expiresAt, revokedAt, replacedBy

## イベント仕様（Change Feed）
認証サービスは通常 Change Feed のソースにはならない想定ですが、以下のイベントを出すケースを想定します。
- user:created
  - payload: { userId, username, email, createdAt, version }
- user:updated
  - payload: { userId, changedFields, timestamp, version }

イベント要件:
- Azure Functions が SQL Database の変更を検出し、Change Feed イベントを配信する。
- イベントは冪等であること（changeId / version の付与）

## セキュリティと運用注意点
- パスワードは PBKDF2 / Argon2 等の強いハッシュ関数で保存する。
- リフレッシュトークンはハッシュ化して DB 保存し、転送時は TLS を必須とする。
- 秘密鍵は Azure Key Vault に格納し、ローテーション手順を整備する。
- Introspect エンドポイントはサービス間認証を要求する（Client Credentials など）。

## テスト / 検証
- 主要フローの自動テスト: ログイン/リフレッシュ/イントロスペクト/ログアウト
- 負荷試験: トークン発行のスループットと Key Vault レイテンシの影響を評価

## 運用時のメトリクス
- 認証成功/失敗数
- リフレッシュトークン発行数
- Introspect 呼び出し率とレイテンシ
- Key Vault の呼び出しエラー率

---

作成者: GitHub Copilot デモ用アウトライン
