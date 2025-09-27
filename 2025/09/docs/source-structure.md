# ソースコード構造

## プロジェクト全体構造

```
dotnetlab-202509-temp/
├── 09.sln                                      # Visual Studio ソリューションファイル
├── APPLICATION_INSIGHTS_GUIDE.md               # Application Insights 設定ガイド
├── dotnetlab-demos.code-workspace              # VS Code ワークスペース設定
├── docs/                                       # プロジェクトドキュメント
└── src/                                        # ソースコード
    ├── api/                                    # ASP.NET Core Web API
    ├── database/                               # SQL Database Project
    └── front/                                  # Angular Frontend
```

## API (ASP.NET Core Web API)

### ディレクトリ構造
```
src/api/
├── api.csproj                          # プロジェクトファイル
├── api.http                            # HTTP リクエストテスト用ファイル
├── Program.cs                          # アプリケーションエントリーポイント
├── README.md                           # API ドキュメント
├── appsettings.json                    # 本番環境設定
├── appsettings.Development.json        # 開発環境設定
├── Properties/
│   └── launchSettings.json             # 起動設定
├── Controllers/                        # API コントローラー
│   ├── SampleController.cs             # サンプルエンドポイント
│   ├── SharesController.cs             # 共有機能 API
│   ├── TagsController.cs               # タグ管理 API
│   ├── TodoItemsController.cs          # ToDo アイテム API
│   └── UsersController.cs              # ユーザー管理 API
├── Data/
│   └── TodoDbContext.cs                # Entity Framework DbContext
├── DTOs/                               # データ転送オブジェクト
│   ├── RequestDtos.cs                  # リクエスト用 DTO
│   ├── TagDto.cs                       # タグ用 DTO
│   ├── TodoItemDto.cs                  # ToDoアイテム用 DTO
│   ├── TodoItemShareDto.cs             # 共有用 DTO
│   └── UserDto.cs                      # ユーザー用 DTO
├── Middleware/                         # カスタムミドルウェア
│   ├── ExceptionHandlingMiddleware.cs  # 例外処理ミドルウェア
│   └── PerformanceMiddleware.cs        # パフォーマンス監視ミドルウェア
├── Models/                             # エンティティモデル
│   ├── Tag.cs                          # タグエンティティ
│   ├── TodoItem.cs                     # ToDoアイテムエンティティ
│   ├── TodoItemShare.cs                # 共有エンティティ
│   ├── TodoItemTag.cs                  # ToDo-タグ関連エンティティ
│   └── User.cs                         # ユーザーエンティティ
└── Services/                           # ビジネスロジック
    ├── CurrentUserService.cs           # 現在ユーザー情報サービス
    ├── TagService.cs                   # タグ管理サービス
    ├── TodoItemService.cs              # ToDoアイテム管理サービス
    ├── TodoItemShareService.cs         # 共有機能サービス
    └── UserService.cs                  # ユーザー管理サービス
```

### 主要コンポーネント詳細

#### 1. Controllers (API エンドポイント)
- **TodoItemsController**: ToDoアイテムの CRUD、検索、フィルタリング
- **TagsController**: タグの管理、ToDoアイテムとの関連付け
- **SharesController**: アイテム共有機能、権限管理
- **UsersController**: ユーザー情報管理、プロファイル
- **SampleController**: ヘルスチェック、サンプルエンドポイント

#### 2. Services (ビジネスロジック層)
各サービスは対応するコントローラーからの要求を処理し、データアクセス層（DbContext）とやり取りします。

#### 3. Models (エンティティ)
Entity Framework Code Firstアプローチで定義されたデータモデル：
- **User**: Azure Entra ID 連携ユーザー情報
- **TodoItem**: ToDoアイテム本体
- **Tag**: 分類用タグ
- **TodoItemTag**: Many-to-Many 関係の中間テーブル
- **TodoItemShare**: 共有機能の権限管理

## Database (SQL Database Project)

### ディレクトリ構造
```
src/database/
├── ToDo.sqlproj                        # SQL Database Project ファイル
├── README.md                           # データベース仕様書
├── Tables/                             # テーブル定義
│   ├── Users.sql                       # ユーザーテーブル
│   ├── TodoItems.sql                   # ToDoアイテムテーブル
│   ├── Tags.sql                        # タグテーブル
│   ├── TodoItemTags.sql                # ToDo-タグ関連テーブル
│   └── TodoItemShares.sql              # 共有権限テーブル
└── Views/                              # ビュー定義
    ├── vw_TodoItemsWithTags.sql        # タグ付きToDoアイテムビュー
    └── vw_UserTodoItems.sql            # ユーザー別ToDoアイテムビュー
```

### データベース設計

#### テーブル構造概要
```
Users (ユーザー)
├── UserId (PK)
├── EntraId (Azure Entra ID)
├── UserPrincipalName
├── DisplayName
└── Email

TodoItems (ToDoアイテム)
├── TodoItemId (PK)
├── UserId (FK → Users)
├── Title
├── Description
├── DueDate
├── Priority
└── IsCompleted

Tags (タグ)
├── TagId (PK)
├── UserId (FK → Users)
├── Name
└── Color

TodoItemTags (多対多関係)
├── TodoItemId (FK → TodoItems)
└── TagId (FK → Tags)

TodoItemShares (共有)
├── ShareId (PK)
├── TodoItemId (FK → TodoItems)
├── SharedWithUserId (FK → Users)
└── Permission
```

## Frontend (Angular Application)

### ディレクトリ構造
```
src/front/
├── angular.json                        # Angular CLI 設定
├── package.json                        # npm 依存関係
├── tailwind.config.js                  # Tailwind CSS 設定
├── proxy.conf.json                     # 開発サーバープロキシ設定
├── tsconfig.json                       # TypeScript 設定
├── MSAL_SETUP.md                       # MSAL 認証設定ガイド
├── README.md                           # フロントエンド仕様書
├── public/
│   └── favicon.ico                     # ファビコン
└── src/
    ├── index.html                      # メインHTMLテンプレート
    ├── main.ts                         # アプリケーションブートストラップ
    ├── styles.scss                     # グローバルスタイル
    ├── app/
    │   ├── app.ts                      # ルートコンポーネント
    │   ├── app.config.ts               # アプリケーション設定
    │   ├── app.routes.ts               # ルーティング設定
    │   ├── auth/                       # 認証関連
    │   │   ├── auth.interceptor.ts     # HTTP認証インターセプター
    │   │   └── auth.guard.ts           # ルートガード
    │   ├── pages/                      # ページコンポーネント
    │   │   ├── dashboard/              # ダッシュボードページ
    │   │   ├── login/                  # ログインページ
    │   │   ├── shared-todos/           # 共有ToDoページ
    │   │   ├── todo-detail/            # ToDo詳細ページ
    │   │   └── todo-list/              # ToDo一覧ページ
    │   ├── services/                   # Angular サービス
    │   │   ├── application-insights.service.ts        #監視サービス
    │   │   ├── application-insights.interceptor.ts    # 監視インターセプター
    │   │   └── application-insights-error-handler.service.ts  # エラーハンドラー
    │   └── shared/                     # 共有コンポーネント
    │       ├── components/             # 再利用可能コンポーネント
    │       ├── directives/             # カスタムディレクティブ
    │       └── pipes/                  # カスタムパイプ
    └── environments/                   # 環境設定
        ├── environment.ts              # 開発環境設定
        └── environment.prod.ts         # 本番環境設定
```

### Angular アーキテクチャの特徴

#### 1. モダンAngular機能の活用
- **Angular 20**: 最新機能の採用
- **Standalone Components**: NgModulesを使わないモジュール構成
- **Signals**: リアクティブな状態管理
- **Control Flow**: `@if`, `@for`, `@switch` による新しいテンプレート制御

#### 2. 認証システム
- **MSAL Angular**: Microsoft Authentication Library
- **Azure Entra ID統合**: シングルサインオン対応
- **JWT Bearer Token**: API アクセス用

#### 3. UI/UX
- **Tailwind CSS**: ユーティリティファーストCSS
- **Angular Material**: Material Design コンポーネント
- **レスポンシブデザイン**: モバイルファースト

## アーキテクチャパターン

### 1. 三層アーキテクチャ
- **プレゼンテーション層**: Angular Frontend
- **ビジネスロジック層**: ASP.NET Core Web API
- **データアクセス層**: Entity Framework + SQL Server

### 2. ドメイン駆動設計 (DDD) 要素
- **エンティティ**: Models フォルダーのドメインオブジェクト
- **サービス**: Services フォルダーのビジネスロジック
- **DTO**: レイヤー間のデータ転送

### 3. 依存性注入 (DI)
- **ASP.NET Core DI**: バックエンドサービスの注入
- **Angular DI**: フロントエンドサービスの注入

### 4. 横断的関心事 (Cross-cutting Concerns)
- **認証**: 全層にわたる認証機能
- **ロギング**: Application Insights による監視
- **エラーハンドリング**: ミドルウェアとインターセプター
- **パフォーマンス**: 監視とメトリクス収集

## 開発時の重要なファイル

### 設定ファイル
- `src/api/appsettings.Development.json`: API開発環境設定
- `src/front/src/environments/environment.ts`: Angular開発環境設定
- `src/front/proxy.conf.json`: 開発時のプロキシ設定

### ビルド・デプロイ
- `src/api/Properties/launchSettings.json`: API起動設定
- `src/front/angular.json`: Angular CLI設定
- `.vscode/tasks.json`: VS Code タスク設定

### テスト用ファイル
- `src/api/api.http`: REST Client用APIテストファイル

この構造により、保守性が高く、スケーラブルなモダンWebアプリケーションが実現されています。