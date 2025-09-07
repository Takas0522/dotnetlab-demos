export const environment = {
  production: true,
  msalConfig: {
    clientId: 'YOUR_PRODUCTION_CLIENT_ID',
    authority: 'https://login.microsoftonline.com/YOUR_TENANT_ID',
    redirectUri: 'https://your-app-domain.com',
    postLogoutRedirectUri: 'https://your-app-domain.com'
  },
  apiConfig: {
    uri: 'https://your-api-domain.com/api',
    scopes: ['api://YOUR_API_CLIENT_ID/access_as_user']
  },
  applicationInsights: {
    connectionString: '' // Application Insightsの接続文字列をここに設定（本番環境用）
  }
};
