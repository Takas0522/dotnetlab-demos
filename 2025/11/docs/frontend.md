# フロントエンド (Angular)

このドキュメントはフロントエンド（Angular）アプリケーションのアーキテクチャと BFF との連携方法を示します。

## 主な責務
- UI 表示、ルーティング、クライアント側の入力バリデーション
- 認証フロー（ログイン画面、トークン管理は BFF を介して行う）
- BFF への API 呼び出しとレスポンスの集約表示

## 構成方針
- Angular (latest) を使用。プロジェクトは Nx / Angular CLI ベースで管理することを推奨。
- サービス層（Angular Services）で BFF のエンドポイントと通信する。
- 認証トークンはブラウザ側で直接保持せず、可能であればセキュアな HttpOnly Cookie を BFF がセットする方式を採用。トークンを保持する場合は短命なアクセストークンとサーバー側でのリフレッシュ制御を併用。

## 認可フロー（推奨）
1. ユーザーがログインフォームに入力し POST /bff/auth/login を呼ぶ。
2. BFF が認証サービスへ問い合わせ、成功時に HttpOnly セットされたセッション/トークン cookie をブラウザへ返す。
3. 以降の API 呼び出しは同一オリジンの BFF へ行い、BFF が Authorization ヘッダを注入してバックエンドへリクエストする。

## API 呼び出しパターン
- プレゼンテーションに近い集約は BFF に任せる。フロントエンドは画面ごとの API を呼ぶだけで良い。
- 長時間のポーリングやリアルタイム更新が必要な場合は SignalR や WebSocket の導入を検討する。

## セキュリティ上の注意
- XSS 対策を徹底し、Content Security Policy (CSP) を設定する。
- CSRF: BFF が HttpOnly Cookie を発行する場合は CSRF トークンを付与して検証する。

## ローカル開発 / CI
- ローカルでは BFF をプロキシに設定し、CORS を回避して API 呼び出しを行う。
- CI/CD では Angular のビルド成果物を静的ホスティング（Azure Static Web Apps / Blob Storage + CDN）へデプロイ。

## テスト
- E2E テスト: Cypress / Playwright を用いて主要シナリオ（ログイン、ToDo 作成、ファイルアップロード）をカバーする。
- 単体テスト: Karma/Jest + Angular Testing Library

---

作成者: GitHub Copilot デモ用アウトライン
