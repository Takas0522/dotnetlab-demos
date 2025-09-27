import { Configuration, LogLevel } from '@azure/msal-browser';
import { environment } from '../../environments/environment';

export const msalConfig: Configuration = {
  auth: {
    clientId: environment.msalConfig.clientId,
    authority: environment.msalConfig.authority,
    redirectUri: environment.msalConfig.redirectUri,
    postLogoutRedirectUri: environment.msalConfig.postLogoutRedirectUri,
  },
  cache: {
    cacheLocation: 'sessionStorage', // セッションストレージを使用
    storeAuthStateInCookie: false, // Cookieに認証状態を保存しない（IE11対応が不要な場合）
  },
  system: {
    loggerOptions: {
      loggerCallback: (level: LogLevel, message: string) => {
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
          default:
            return;
        }
      },
      logLevel: LogLevel.Info,
    },
  },
};

export const loginRequest = {
  redirectUri: environment.msalConfig.redirectUri,
  scopes: [...environment.apiConfig.scopes],
};

export const graphConfig = {
  graphMeEndpoint: 'https://graph.microsoft.com/v1.0/me',
};
