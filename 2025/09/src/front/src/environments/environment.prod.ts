export const environment = {
  production: true,
  msalConfig: {
    clientId: '', // Microsoft Entra IDアプリのクライアントID
    authority: '', // または 'common'
    redirectUri: '',
    postLogoutRedirectUri: ''
  },
  apiConfig: {
    uri: '', // バックエンドAPIのURL
    scopes: [''] // カスタムAPIスコープ
  },
  applicationInsights: {
    connectionString: '' // Application Insightsの接続文字列をここに設定
  }
};
