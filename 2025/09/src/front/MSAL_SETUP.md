# Microsoft Entra ID 認証設定ガイド

このプロジェクトでは、Microsoft Entra ID（旧Azure AD）を使用したOAuth 2.0認証を実装しています。

## 前提条件

1. Microsoft Entra IDテナントへのアクセス
2. アプリケーション登録の権限

## セットアップ手順

### 1. Microsoft Entra IDでのアプリケーション登録

1. [Azure Portal](https://portal.azure.com)にアクセス
2. 「Microsoft Entra ID」→「アプリの登録」→「新規登録」
3. 以下の設定で登録：
   - **名前**: お好みのアプリケーション名
   - **サポートされているアカウントの種類**: 組織のディレクトリ内のアカウントのみ
   - **リダイレクトURI**: 
     - 種類: シングルページアプリケーション (SPA)
     - URI: `http://localhost:4200` (開発環境)

### 2. アプリケーションの設定

#### APIのアクセス許可
1. 登録したアプリの「APIのアクセス許可」セクション
2. 「アクセス許可の追加」→「Microsoft Graph」
3. 以下の委任されたアクセス許可を追加：
   - `User.Read` (ユーザープロフィールの読み取り)
   - `openid` (OpenIDConnect)
   - `profile` (基本プロフィール情報)

#### 認証設定
1. 「認証」セクション
2. 「暗黙的な許可およびハイブリッド フロー」で以下をチェック：
   - ✅ アクセス トークン
   - ✅ ID トークン

### 3. 環境設定ファイルの更新

`src/environments/environment.ts` ファイルを以下の情報で更新：

```typescript
export const environment = {
  production: false,
  msalConfig: {
    clientId: 'YOUR_CLIENT_ID', // アプリケーション(クライアント)ID
    authority: 'https://login.microsoftonline.com/YOUR_TENANT_ID', // ディレクトリ(テナント)ID
    redirectUri: 'http://localhost:4200',
    postLogoutRedirectUri: 'http://localhost:4200'
  },
  apiConfig: {
    uri: 'https://localhost:7000/api', // バックエンドAPIのURL
    scopes: ['api://YOUR_API_CLIENT_ID/access_as_user'] // カスタムAPIがある場合
  }
};
```

### 4. 必要な情報の取得

Azure Portalのアプリ登録画面から以下の情報を取得：

- **アプリケーション(クライアント)ID**: `YOUR_CLIENT_ID`に設定
- **ディレクトリ(テナント)ID**: `YOUR_TENANT_ID`に設定

### 5. 本番環境の設定

`src/environments/environment.prod.ts` も同様に更新し、本番環境用のURLとIDを設定してください。

## 使用方法

### 基本的な認証フロー

1. **ログイン**: `/login` ページでMicrosoftアカウントでサインイン
2. **認証後**: 自動的に `/dashboard` にリダイレクト
3. **ログアウト**: ダッシュボードの「ログアウト」ボタン

### コンポーネントでの使用例

```typescript
import { Component, inject } from '@angular/core';
import { AuthService } from './auth/auth.service';

@Component({...})
export class MyComponent {
  private readonly authService = inject(AuthService);
  
  // 認証状態の確認
  readonly isAuthenticated = this.authService.isAuthenticated;
  
  // ユーザー情報の取得
  readonly user = this.authService.account;
  
  // アクセストークンの取得
  async callApi() {
    try {
      const token = await this.authService.getAccessToken();
      // APIコール
    } catch (error) {
      console.error('Token acquisition failed', error);
    }
  }
}
```

### HTTPリクエストでの自動認証

`AuthInterceptor`により、以下のURLに対するHTTPリクエストには自動的にBearerトークンが追加されます：

- Microsoft Graph API (`https://graph.microsoft.com`)
- アプリケーションAPI（環境設定で指定したURL）

## トラブルシューティング

### よくある問題

1. **リダイレクトURIエラー**
   - Azure Portalの認証設定でリダイレクトURIが正しく設定されているか確認

2. **アクセス許可エラー**
   - 必要なAPIアクセス許可が追加されているか確認
   - 管理者の同意が必要な場合は管理者に依頼

3. **CORS エラー**
   - SPAアプリケーションとして正しく登録されているか確認

### デバッグ

MSALのログレベルを調整するには、`auth-config.ts`の`logLevel`を変更してください：

```typescript
logLevel: LogLevel.Verbose, // より詳細なログ
```

## セキュリティ考慮事項

1. **クライアントID**: 公開情報のため、ソースコードに含まれても問題ありません
2. **トークンの保存**: セッションストレージを使用してローカル保存を最小限に
3. **HTTPS必須**: 本番環境では必ずHTTPSを使用
4. **スコープの最小化**: 必要最小限のアクセス許可のみ要求

## 参考資料

- [Microsoft Identity Platform Documentation](https://docs.microsoft.com/ja-jp/azure/active-directory/develop/)
- [MSAL.js Documentation](https://docs.microsoft.com/ja-jp/azure/active-directory/develop/msal-js-initializing-client-applications)
- [Angular MSAL Sample](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/samples/msal-angular-v3-samples)
