# ToDo Management Application

Azure Entra ID認証機能を持つモダンなToDo管理Webアプリケーションです。Angular 20とASP.NET Core 9.0を使用して構築されており、チーム間でのタスク共有とコラボレーションをサポートします。

## 🚀 主要機能

- **認証**: Azure Entra ID統合によるシングルサインオン
- **ToDo管理**: タスクの作成、編集、削除、完了管理
- **タグ分類**: カスタムタグによるタスク分類・検索
- **共有機能**: チームメンバー間でのタスク共有
- **リアルタイム監視**: Application Insightsによるパフォーマンス監視
- **AI支援**: GitHub Actionsによる自動Issue分析とラベル付与

## 🛠 技術スタック

### フロントエンド
- **Angular 20.2** - モダンなSPAフレームワーク
- **TypeScript** - 型安全な開発
- **Tailwind CSS** - ユーティリティファーストCSS
- **Angular Material** - マテリアルデザインコンポーネント
- **MSAL Angular** - Microsoft認証ライブラリ

### バックエンド  
- **ASP.NET Core 9.0** - 高性能WebAPIフレームワーク
- **Entity Framework Core** - オブジェクトリレーショナルマッピング
- **Microsoft.Identity.Web** - Azure Entra ID統合
- **Application Insights** - アプリケーション監視

### データベース
- **SQL Server** - リレーショナルデータベース
- **Database Project** - スキーマ管理とバージョン管理

## 📁 プロジェクト構造

```
├── docs/                    # プロジェクトドキュメント
│   ├── project-overview.md      # プロジェクト概要
│   ├── source-structure.md      # ソースコード構造
│   ├── debugging-guide.md       # デバッグガイド
│   └── ai-development-guide.md  # AI開発支援ガイド
├── src/
│   ├── api/                # ASP.NET Core Web API
│   ├── database/           # SQL Database Project
│   └── front/             # Angular Frontend
└── *.sln                  # Visual Studio ソリューション
```

## 🔧 セットアップ

### 前提条件
- .NET 9.0 SDK
- Node.js 18+
- Angular CLI 20+
- SQL Server (LocalDB可)
- Azure Entra IDテナント

### 1. リポジトリのクローン
```bash
git clone <repository-url>
cd dotnetlab-202509-temp
```

### 2. データベースセットアップ
```bash
cd src/database
dotnet build

cd ../api
dotnet ef database update
```

### 3. APIの起動
```bash
cd src/api
dotnet run
```
API: http://localhost:5115

### 4. フロントエンドの起動
```bash
cd src/front
npm install
npm start
```
Web App: http://localhost:4200

## 📖 ドキュメント

詳細な情報については、以下のドキュメントを参照してください：

### 🌟 [プロジェクト概要](./docs/project-overview.md)
アプリケーションの目的、技術スタック、アーキテクチャの詳細

### 🏗️ [ソースコード構造](./docs/source-structure.md)  
ディレクトリ構造、各コンポーネントの役割、モジュール間の関係

### 🐛 [デバッグガイド](./docs/debugging-guide.md)
各環境でのデバッグ方法、必要な設定、トラブルシューティング

### 🤖 [AI開発支援ガイド](./docs/ai-development-guide.md)
AI Agentによる開発サポートのためのカスタムインストラクション

## 🚀 開発開始

1. **環境セットアップ**: [デバッグガイド](./docs/debugging-guide.md)を参照
2. **プロジェクト理解**: [プロジェクト概要](./docs/project-overview.md)と[ソースコード構造](./docs/source-structure.md)を確認
3. **AI支援活用**: [AI開発支援ガイド](./docs/ai-development-guide.md)でコーディング規約を確認

## 🔒 セキュリティ

このアプリケーションは以下のセキュリティ機能を実装しています：

- Azure Entra ID による統合認証
- JWT Bearer トークンによるAPI認証
- HTTPS通信の強制
- SQL インジェクション対策
- XSS/CSRF保護

## 📊 監視

Application Insightsによる包括的な監視を実装：

- リアルタイムパフォーマンス監視
- エラートラッキング
- ユーザー行動分析
- カスタムメトリクス

## � GitHub Actions - AI支援機能

このプロジェクトには以下のGitHub Actionsワークフローが設定されています：

### 自動Issue分析とラベル付与
新しいIssueが作成されると、Azure OpenAI APIを使用してIssueの内容を自動分析し、適切なラベルを付与します。

**ワークフロー**: `.github/workflows/auto-label-issues.yml`

**機能**:
1. **ラベル取得**: GitHub APIからリポジトリの全ラベルを取得
2. **AI分析**: Azure OpenAIでIssueタイトルと本文を分析
3. **ラベル付与**: 最適なラベル（最大3個）を自動選択・適用
4. **通知**: 適用されたラベルについてIssueにコメント

**設定が必要なSecrets**:
- `AZURE_OPENAI_ENDPOINT`: Azure OpenAIエンドポイントURL
- `AZURE_OPENAI_API_KEY`: Azure OpenAI APIキー
- `AZURE_OPENAI_DEPLOYMENT`: デプロイメント名

### Issue回答支援
Issueが作成されると、プロジェクトのコードコンテキストを含めてAIが自動回答を提供します。

**ワークフロー**: `.github/workflows/ai-answer.yml`

### その他のワークフロー
- **APIデプロイ**: Azure App Serviceへの自動デプロイ
- **フロントエンドデプロイ**: 静的サイトの自動デプロイ
- **Discussion回答**: GitHubディスカッションへのAI回答
- **タグ管理**: リリースタグの自動管理

## �🤝 コントリビューション

1. フィーチャーブランチを作成
2. [AI開発支援ガイド](./docs/ai-development-guide.md)のコーディング規約に従う
3. 適切なテストを追加
4. プルリクエストを作成

## 📄 ライセンス

このプロジェクトは[MIT License](LICENSE)の下で公開されています。

---

## ⚡ クイックスタート

### すべてを一度に起動
```bash
# ターミナル1: API
cd src/api && dotnet run

# ターミナル2: Frontend  
cd src/front && npm start
```

### APIテスト
REST Clientを使用して`src/api/api.http`でAPIテストを実行できます。

### デバッグ
VS Codeで各プロジェクトを開き、F5でデバッグ開始できます。詳細は[デバッグガイド](./docs/debugging-guide.md)を参照してください。