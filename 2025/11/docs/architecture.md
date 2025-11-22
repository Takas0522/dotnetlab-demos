## システム概要

このドキュメントは、GitHub Copilot デモ用に設計した仮想サービス群のアーキテクチャアウトラインを示します。

目的: 小規模なマイクロサービス群（認証、フロントエンド、ユーザー管理、BFF、ToDo、ファイル共有）を共通技術スタックで構築し、開発者間のやり取りと運用のしやすさを担保すること。

基本設計の要点:
- 各サービスは独立した ASP.NET Core WebAPI（C#）で実装され、個別の SQL Database を持ちます（データベースはサービス単位の所有権を持つ）。
- フロントエンドは Angular（最新）で実装され、ユーザー操作は BFF（Backend For Frontend）を経由して各サービスと連携します。
- サービス間の即時同期は行わず、Azure Functions による Change Feed（SQL Database の変更をトリガー）を用いてイベントベースでデータを同期します。これにより疎結合な非同期連携を実現します。
- 認証は専用の認証サービスが担当し、トークンベースの認証（OAuth 2.0 / JWT）を採用します。各サービスは BFF または直接トークンを検証して認可を行います。

技術スタック（共通）:
- FrontEnd: Angular (latest)
- WebAPI: ASP.NET Core WebAPI - C# (latest)
- Database: SQL Database (各サービスごとに分離)
- Azure Functions: C# (Change Feed トリガー用)

配置・運用の前提:
- 各サービスはコンテナ化（Docker）してデプロイすることを想定します。
- Azure SQL Database を使用し、変更検出（Change Feed）は Azure Functions の定期実行もしくは DB の変更通知機構を経由して実装します。
- ロギング/トレーシングは共通で OpenTelemetry を採用し、集中ログと分散トレースへ送信します。

詳細な各サービス情報は個別ファイルに記載しています（/docs 以下）。

## サービス一覧（役割の概要）

- 認証サービス (Authentication)
	- ユーザー認証、発行 JWT、OAuth2 フローの管理
- フロントエンド (Frontend)
	- Angular アプリケーション。BFF を通じてバックエンドとやり取り。
- ユーザー管理サービス (User Management)
	- ユーザーの CRUD、プロファイル、権限管理
- BFF (Backend For Frontend)
	- フロントエンド固有の API 集約、認可ヘッダの注入、複数サービス呼び出しの調整
- ToDo サービス (ToDo Service)
	- ユーザーの ToDo 管理。検索/フィルタ、完了/期日管理
- ファイル共有サービス (File Sharing)
	- ファイルのアップロード、メタデータ管理、ストレージ連携（Blob Storage 等）

## サービス間の依存関係

通信方式:
- 同期: WebAPI（HTTP/HTTPS, JSON） — BFF とフロントエンド、サービス間の直接リクエスト（必要最小限）
- 非同期同期: Azure Functions を使った Change Feed ベースのデータ同期（SQL Database の変更検出 → Functions が変更イベントを発行 → 対象サービスが処理）

依存マップ（高レベル）:

1. Frontend → BFF
	 - すべての UI リクエストは原則 BFF 経由で送られる。BFF が認証トークンの検証や API 集約を行う。
2. BFF → Authentication
	 - 認証トークンの検証やリフレッシュ、セッション照会のために呼び出す。
3. BFF → User Management / ToDo / File Sharing
	 - UI に必要なデータを集約して返す。必要に応じて複数サービスへ並列リクエストを行う。
4. User Management → Authentication
	 - ユーザーの作成やパスワードリセット時に認証サービスへ情報を同期する（Change Feed 経由を推奨）。
5. ToDo, File Sharing → User Management
	 - 所有者情報や参照整合性のためにユーザー情報を参照する。参照はキャッシュ/コピーで行い、整合性は Change Feed で補正する。

Change Feed 同期パターン:
- あるサービスの SQL Database に変更があった場合、Azure Functions が変更イベントを受け取り、必要なターゲットサービスへイベント（HTTP Webhook / Service Queue / Event Grid など）を送る。
- 例: User Management のユーザー情報が更新されたら、Functions は ToDo / File Sharing のマイクロサービスへ「ユーザ情報更新」イベントを送付し、各サービスはローカル DB のプロファイルコピーを更新する。
- イベントの冪等性を担保するため、イベントには changeId / timestamp / version 情報を含める。

エラー・リトライ方針:
- Functions は送信失敗の場合、一定回数リトライし、失敗が続く場合はデッドレターキュー（Storage queue / Service Bus DLQ）へ退避させる。
- 受信側サービスは受信イベントの重複処理（冪等設計）を必須とする。

セキュリティ:
- 内部 API は mTLS または Azure AD によるサービス間認証を利用することを推奨。
- フロントエンドは BFF 経由でトークンの扱いを簡素化し、ブラウザ側トークン露出を最小化する。

運用 / 監視:
- OpenTelemetry によるトレース、Azure Monitor / Application Insights へメトリクスとログを集約。
- Azure Functions の実行ログと Change Feed イベントメトリクスを可視化し、同期遅延やエラーをアラート化する。

## 次のステップ

- 各サービスの詳細（API 契約、DB スキーマ概要、イベント仕様、運用注意）を個別のファイルに展開します。
- 詳細ファイル:
	- `/docs/authentication.md`
	- `/docs/frontend.md`
	- `/docs/user-management.md`
	- `/docs/bff.md`
	- `/docs/todo-service.md`
	- `/docs/file-sharing.md`

---

作成者: GitHub Copilot デモ用アウトライン

