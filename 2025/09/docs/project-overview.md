# プロジェクト概要

## 概要

このプロジェクトは、モダンなWebテクノロジーを使用して構築されたToDo管理アプリケーションです。Azure Entra ID（旧Azure AD）による認証機能を持ち、チーム間でのタスク共有とコラボレーションをサポートします。

## 技術スタック

### バックエンド（API）
- **フレームワーク**: ASP.NET Core 9.0 (C#)
- **認証**: Microsoft Identity Web (Azure Entra ID/JWT Bearer)
- **データベース**: SQL Server
- **ORM**: Entity Framework Core 9.0
- **API仕様**: OpenAPI/Swagger
- **監視**: Application Insights

### フロントエンド
- **フレームワーク**: Angular 20.2
- **言語**: TypeScript
- **スタイリング**: Tailwind CSS, Angular Material
- **認証**: Microsoft Authentication Library (MSAL) for Angular
- **監視**: Application Insights Web SDK

### データベース
- **DBMS**: SQL Server
- **プロジェクト**: SQL Database Project (.sqlproj)
- **スキーマ管理**: Database-first approach

## アプリケーションアーキテクチャ

### 全体構成
```
┌─────────────────┐    HTTP/HTTPS    ┌─────────────────┐    SQL    ┌─────────────────┐
│   Angular SPA   │ ◄──────────────► │  ASP.NET Core   │ ◄───────► │   SQL Server    │
│   (Frontend)    │                  │     Web API     │           │   (Database)    │
└─────────────────┘                  └─────────────────┘           └─────────────────┘
         │                                     │                             │
         ▼                                     ▼                             ▼
┌─────────────────┐                  ┌─────────────────┐           ┌─────────────────┐
│  Azure Entra ID │                  │ Application     │           │  Database       │
│  (Authentication)│                  │   Insights      │           │   Views         │
└─────────────────┘                  └─────────────────┘           └─────────────────┘
```

### 主要コンポーネント

#### 1. フロントエンド (Angular SPA)
- **場所**: `src/front/`
- **役割**: ユーザーインターフェース、認証、API通信
- **機能**:
  - Azure Entra IDによるシングルサインオン
  - ToDoアイテムの作成、編集、削除
  - タグによる分類
  - タスクの共有機能
  - レスポンシブデザイン

#### 2. バックエンド (ASP.NET Core Web API)
- **場所**: `src/api/`
- **役割**: ビジネスロジック、認証認可、データアクセス
- **機能**:
  - RESTful API の提供
  - JWT Bearer認証
  - Entity Framework によるデータアクセス
  - カスタムミドルウェア（例外処理、パフォーマンス監視）
  - Swagger による API ドキュメント

#### 3. データベース (SQL Server)
- **場所**: `src/database/`
- **役割**: データ永続化、リレーショナルデータ管理
- **機能**:
  - ユーザー管理
  - ToDoアイテム管理
  - タグ管理
  - 共有機能
  - データベースビューによる複雑なクエリのサポート

## 主要機能

### 1. 認証・認可
- Azure Entra ID による統合認証
- JWT Bearer トークンによる API アクセス制御
- ユーザー情報の自動同期

### 2. ToDo管理
- ToDoアイテムの CRUD 操作
- 優先度、期限、ステータス管理
- タグによる分類・検索機能

### 3. コラボレーション
- ToDoアイテムの他ユーザーとの共有
- 共有権限の管理
- チーム作業のサポート

### 4. 監視・ロギング
- Application Insights による アプリケーション監視
- パフォーマンス監視
- エラートラッキング

## セキュリティ

### 認証フロー
1. ユーザーが Angular アプリケーションにアクセス
2. MSAL ライブラリが Azure Entra ID にリダイレクト
3. ユーザー認証後、JWT アクセストークンを取得
4. すべての API リクエストにBearerトークンを含めて送信
5. API側でトークンの検証と認可を実行

### セキュリティ対策
- HTTPS通信の強制
- JWT トークンの有効期限管理
- ユーザー固有のデータアクセス制御
- SQL インジェクション対策（Entity Framework使用）
- XSS対策（Angular標準対策）

## 開発環境要件

### 必要なソフトウェア
- **.NET 9.0 SDK**
- **Node.js 18+** (npm含む)
- **Angular CLI 20+**
- **SQL Server** (LocalDB可)
- **Visual Studio Code** または **Visual Studio**

### Azure リソース
- **Azure Entra ID テナント**
- **Application Insights** (オプション)

## プロジェクト構成

```
dotnetlab-202509-temp/
├── docs/                          # プロジェクトドキュメント
├── src/
│   ├── api/                       # ASP.NET Core Web API
│   │   ├── Controllers/           # API コントローラー
│   │   ├── Data/                  # Entity Framework DbContext
│   │   ├── DTOs/                  # データ転送オブジェクト
│   │   ├── Models/                # エンティティモデル
│   │   ├── Services/              # ビジネスロジック
│   │   └── Middleware/            # カスタムミドルウェア
│   ├── database/                  # SQL Database Project
│   │   ├── Tables/                # テーブル定義
│   │   └── Views/                 # ビュー定義
│   └── front/                     # Angular Frontend
│       └── src/
│           ├── app/
│           │   ├── auth/          # 認証関連
│           │   ├── pages/         # ページコンポーネント
│           │   ├── services/      # Angular サービス
│           │   └── shared/        # 共有コンポーネント
│           └── environments/      # 環境設定
└── *.sln                         # Visual Studio ソリューション
```

## 次のステップ

詳細な情報については、以下のドキュメントを参照してください：

- [ソースコード構造](./source-structure.md)
- [デバッグガイド](./debugging-guide.md)
- [AI開発支援ガイド](./ai-development-guide.md)