# デバッグガイド

## 概要

このガイドでは、ToDoアプリケーションの各コンポーネント（API、フロントエンド、データベース）のデバッグ方法と開発環境の構築手順を説明します。

## 前提条件

### 必要なソフトウェア

1. **.NET 9.0 SDK**
   ```powershell
   dotnet --version  # 9.0.x が表示されることを確認
   ```

2. **Node.js 18+**
   ```powershell
   node --version    # v18.x.x 以上が表示されることを確認
   npm --version     # 対応するnpmバージョンを確認
   ```

3. **Angular CLI 20+**
   ```powershell
   npm install -g @angular/cli@latest
   ng version       # Angular CLI: 20.x.x 以上を確認
   ```

4. **SQL Server**
   - SQL Server Express/Developer Edition
   - または SQL Server LocalDB
   ```powershell
   sqllocaldb info  # LocalDB インスタンス一覧を確認
   ```

### Azure 前提条件

1. **Azure Entra ID テナント**
2. **アプリケーション登録** (SPA + Web API)
3. **Application Insights リソース** (オプション)

## 開発環境セットアップ

### 1. データベースセットアップ

#### LocalDB を使用する場合
```powershell
# LocalDB インスタンス作成
sqllocaldb create "TodoDb" -s

# 接続文字列例
# Server=(localdb)\TodoDb;Database=TodoDatabase;Trusted_Connection=true;
```

#### SQL Server を使用する場合
```powershell
# 接続文字列例
# Server=localhost;Database=TodoDatabase;Trusted_Connection=true;
# または
# Server=localhost;Database=TodoDatabase;User Id=sa;Password=YourPassword;
```

### 2. API 設定

#### appsettings.Development.json 設定例
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\TodoDb;Database=TodoDatabase;Trusted_Connection=true;"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "your-tenant-id",
    "ClientId": "your-api-client-id",
    "Audience": "api://your-api-client-id"
  },
  "ApplicationInsights": {
    "ConnectionString": "your-connection-string"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### 3. フロントエンド設定

#### environments/environment.ts 設定例
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5115/api',
  msalConfig: {
    auth: {
      clientId: 'your-spa-client-id',
      authority: 'https://login.microsoftonline.com/your-tenant-id',
      redirectUri: 'http://localhost:4200'
    }
  },
  applicationInsights: {
    connectionString: 'your-connection-string'
  }
};
```

## デバッグ手順

### 1. データベース起動とマイグレーション

```powershell
# データベースプロジェクトのビルド
cd src\database
dotnet build

# API プロジェクトでマイグレーション実行
cd ..\api
dotnet ef database update
```

### 2. API デバッグ

#### Visual Studio Code でのデバッグ
1. VS Code で `src/api` フォルダーを開く
2. `F5` キーを押してデバッグ開始
3. ブレークポイントを設定して動作確認

#### コマンドラインでの起動
```powershell
cd src\api
dotnet run
```

起動後、以下のURLでアクセス可能：
- **API エンドポイント**: http://localhost:5115
- **Swagger UI**: http://localhost:5115/swagger

#### API ヘルスチェック
```powershell
# 基本ヘルスチェック
curl http://localhost:5115/

# レスポンス例
# {"Status":"API is running","Timestamp":"2025-09-23T..."}
```

### 3. フロントエンド デバッグ

#### Angular アプリケーション起動
```powershell
cd src\front
npm install      # 初回のみ
npm start        # ng serve --proxy-config proxy.conf.json と同等
```

アクセスURL：
- **Angular アプリケーション**: http://localhost:4200
- **API プロキシ**: http://localhost:4200/api → http://localhost:5115/api

#### Angular デバッグ設定

##### ブラウザ開発者ツール
1. Chrome/Edge で F12 を押下
2. Sources タブでTypeScriptファイルにブレークポイント設定
3. ネットワークタブでAPI通信確認

##### VS Code でのデバッグ
1. `launch.json` 設定例：
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "type": "chrome",
      "request": "launch",
      "name": "Angular Debug",
      "url": "http://localhost:4200",
      "webRoot": "${workspaceFolder}/src/front/src"
    }
  ]
}
```

### 4. 統合デバッグ（フルスタック）

#### 同時起動スクリプト
```powershell
# ターミナル1: API
cd src\api
dotnet run

# ターミナル2: Angular
cd src\front
npm start
```

#### VS Code タスク設定
`.vscode/tasks.json` 例：
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Start API",
      "type": "shell",
      "command": "dotnet run",
      "options": {
        "cwd": "${workspaceFolder}/src/api"
      },
      "isBackground": true,
      "group": "build"
    },
    {
      "label": "Start Frontend",
      "type": "shell",
      "command": "npm start",
      "options": {
        "cwd": "${workspaceFolder}/src/front"
      },
      "isBackground": true,
      "group": "build"
    }
  ]
}
```

## 認証デバッグ

### Azure Entra ID 設定確認

#### API アプリケーション登録
- **アプリケーション ID URI**: `api://your-api-client-id`
- **公開スコープ**: `access_as_user`
- **アプリケーションロール**: 必要に応じて設定

#### SPA アプリケーション登録
- **リダイレクトURL**: `http://localhost:4200`
- **許可されたトークンタイプ**: アクセストークン
- **API アクセス許可**: API アプリケーションの `access_as_user`

### 認証フロー デバッグ

#### 1. MSAL ログ確認
```typescript
// src/front/src/environments/environment.ts
export const environment = {
  // ...
  msalConfig: {
    auth: {
      // ...
    },
    system: {
      loggerOptions: {
        loggerCallback: (level, message, containsPii) => {
          console.log('MSAL Log:', message);
        },
        logLevel: LogLevel.Verbose
      }
    }
  }
};
```

#### 2. JWT トークン検証
- [jwt.io](https://jwt.io) でトークンの内容確認
- トークンの有効期限、スコープ、クレーム確認

#### 3. API 認証エラー確認
```csharp
// src/api/Middleware/ExceptionHandlingMiddleware.cs で詳細ログ確認
```

## トラブルシューティング

### よくある問題と解決方法

#### 1. CORS エラー
**症状**: ブラウザで CORS エラーが発生
**解決**: API の Program.cs で CORS 設定確認
```csharp
app.UseCors(builder => builder
    .WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
```

#### 2. データベース接続エラー
**症状**: Entity Framework でデータベース接続失敗
**解決手順**:
1. 接続文字列確認
2. LocalDB/SQL Server の起動確認
3. マイグレーション実行
```powershell
dotnet ef database update
```

#### 3. 認証トークンエラー
**症状**: API で 401 Unauthorized エラー
**解決手順**:
1. Azure Entra ID 設定確認
2. JWT トークンの内容確認
3. API の AzureAd 設定確認

#### 4. npm/Node.js エラー
**症状**: Angular ビルドエラー
**解決手順**:
```powershell
# キャッシュクリア
npm cache clean --force

# node_modules 再インストール
rm -rf node_modules package-lock.json
npm install
```

#### 5. ポート競合
**症状**: ポートが既に使用されているエラー
**解決手順**:
```powershell
# ポート使用状況確認
netstat -ano | findstr :5115
netstat -ano | findstr :4200

# プロセス終了（PID確認後）
taskkill /PID <PID> /F
```

## デバッグツール

### 1. API テストツール

#### REST Client (VS Code拡張)
`src/api/api.http` ファイルを使用してAPIテスト実行

#### Postman
- 認証設定: Bearer Token
- 環境変数設定でベースURL管理

### 2. ログ監視

#### Application Insights
- リアルタイムメトリクス
- 例外とトレース
- パフォーマンス監視

#### Console ログ
```csharp
// API ログ出力
_logger.LogInformation("Debug message: {Variable}", variable);
```

```typescript
// Angular ログ出力
console.log('Debug message:', variable);
```

### 3. データベースツール

#### SQL Server Management Studio (SSMS)
#### Azure Data Studio
#### VS Code SQL Server 拡張

## パフォーマンスデバッグ

### 1. API パフォーマンス
- Application Insights でレスポンス時間監視
- Entity Framework ログでSQL クエリ確認
- PerformanceMiddleware でカスタム測定

### 2. フロントエンド パフォーマンス
- Chrome DevTools の Performance タブ
- Angular DevTools 拡張
- Bundle Analyzer でバンドルサイズ確認

```powershell
# Bundle サイズ分析
npm run build -- --named-chunks
npx webpack-bundle-analyzer dist/front
```

このガイドに従って開発環境を構築し、効率的なデバッグを行ってください。