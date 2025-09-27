import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { 
  PublicClientApplication,
  InteractionStatus,
  PopupRequest,
  RedirectRequest,
  SilentRequest,
  AccountInfo,
  AuthenticationResult,
  EndSessionRequest,
} from '@azure/msal-browser';
import { Subject, filter, takeUntil } from 'rxjs';
import { msalConfig, loginRequest as defaultLoginRequest } from './auth-config';
import { ApplicationInsightsService } from '../services/application-insights.service';
import { UserService } from '../shared/services/user.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly msalInstance = new PublicClientApplication(msalConfig);
  private readonly router = inject(Router);
  private readonly appInsights = inject(ApplicationInsightsService);
  private readonly userService = inject(UserService);
  private readonly destroy$ = new Subject<void>();
  private _initializationPromise: Promise<void> | null = null;
  
  // Signals for reactive state management
  private readonly _isAuthenticated = signal<boolean>(false);
  private readonly _account = signal<AccountInfo | null>(null);
  private readonly _interactionStatus = signal<InteractionStatus>(InteractionStatus.Startup);
  private readonly _isInitialized = signal<boolean>(false);
  
  // Public readonly computed signals
  readonly isAuthenticated = computed(() => this._isAuthenticated());
  readonly account = computed(() => this._account());
  readonly interactionStatus = computed(() => this._interactionStatus());
  readonly isInitialized = computed(() => this._isInitialized());
  readonly isLoginInProgress = computed(() => 
    this._interactionStatus() === InteractionStatus.Login
  );

  constructor() {
    this.initializeMsal();
  }

  private async initializeMsal(): Promise<void> {
    try {
      await this.msalInstance.initialize();
      this._isInitialized.set(true);
      
      // リダイレクトレスポンスを処理
      const response = await this.msalInstance.handleRedirectPromise();
      if (response && response.account) {
        this.setActiveAccount(response.account);
      } else {
        // アカウント情報を設定
        this.setActiveAccount();
      }
      
      // インタラクションの状態を監視
      this.msalInstance.addEventCallback((event) => {
        if (event.eventType === 'msal:acquireTokenSuccess' && event.payload) {
          const payload = event.payload as AuthenticationResult;
          this.setActiveAccount(payload.account);
        }
        
        if (event.eventType === 'msal:acquireTokenFailure') {
          console.error('Token acquisition failed', event.error);
        }
        
        if (event.eventType === 'msal:loginSuccess' && event.payload) {
          const payload = event.payload as AuthenticationResult;
          this.setActiveAccount(payload.account);
        }
        
        if (event.eventType === 'msal:loginFailure') {
          console.error('Login failed', event.error);
        }
        
        if (event.eventType === 'msal:logoutSuccess') {
          this.setActiveAccount(null);
        }
      });
      
      this._interactionStatus.set(InteractionStatus.None);
    } catch (error) {
      console.error('MSAL initialization failed', error);
      this._interactionStatus.set(InteractionStatus.None);
    }
  }

  private async ensureInitialized(): Promise<void> {
    if (!this._isInitialized()) {
      if (!this._initializationPromise) {
        this._initializationPromise = this.initializeMsal();
      }
      await this._initializationPromise;
    }
  }

  private setActiveAccount(account?: AccountInfo | null): void {
    let activeAccount = account;
    const wasAuthenticated = this._isAuthenticated();
    
    if (!activeAccount) {
      // アクティブなアカウントがない場合、利用可能なアカウントから選択
      const accounts = this.msalInstance.getAllAccounts();
      if (accounts.length > 0) {
        activeAccount = accounts[0];
        this.msalInstance.setActiveAccount(activeAccount);
      }
    } else {
      this.msalInstance.setActiveAccount(activeAccount);
    }
    
    this._account.set(activeAccount || null);
    this._isAuthenticated.set(!!activeAccount);
    
    // 認証状態が変更された場合（未認証 → 認証済み）にダッシュボードにリダイレクト
    if (!wasAuthenticated && !!activeAccount) {
      this.handleUserLogin();
    }
  }

  private async handleUserLogin(): Promise<void> {
    try {
      // ユーザー情報を取得または作成
      await this.userService.getCurrentUser().toPromise();
      console.log('User registration/retrieval successful');
      
      // ダッシュボードにリダイレクト
      this.navigateToDefaultPage();
    } catch (error) {
      console.error('Failed to register/retrieve user:', error);
      
      // Application Insightsにエラーを記録
      this.appInsights.trackException(error instanceof Error ? error : new Error(String(error)));
      
      // エラーが発生してもダッシュボードにリダイレクト（ユーザー体験を優先）
      this.navigateToDefaultPage();
    }
  }

  private navigateToDefaultPage(): void {
    // 現在のURLを確認
    const currentUrl = this.router.url;
    
    // ログインページにいるか、ルートページにいる場合はダッシュボードにリダイレクト
    if (currentUrl === '/login' || currentUrl === '/' || currentUrl === '') {
      console.log('Login successful, redirecting to dashboard');
      
      // Application Insightsにログイン成功イベントを送信
      this.appInsights.trackEvent('User.LoginSuccess', {
        redirectFrom: currentUrl,
        redirectTo: '/dashboard',
        userId: this._account()?.username || 'Unknown'
      });
      
      // ユーザー情報をApplication Insightsに設定
      const account = this._account();
      if (account) {
        this.appInsights.setUser(
          account.homeAccountId,
          account.tenantId,
          account.username
        );
      }
      
      this.router.navigate(['/dashboard']);
    }
  }

  async loginWithPopup(): Promise<void> {
    try {
      await this.ensureInitialized();
      this._interactionStatus.set(InteractionStatus.Login);
      
      const loginRequest: PopupRequest = {
        ...defaultLoginRequest,
        prompt: 'select_account',
      };
      
      const result = await this.msalInstance.loginPopup(loginRequest);
      this.setActiveAccount(result.account);
    } catch (error) {
      console.error('Login with popup failed', error);
      throw error;
    } finally {
      this._interactionStatus.set(InteractionStatus.None);
    }
  }

  async loginWithRedirect(): Promise<void> {
    try {
      await this.ensureInitialized();
      this._interactionStatus.set(InteractionStatus.Login);
      
      const loginRequest: RedirectRequest = {
        ...defaultLoginRequest,
        prompt: 'select_account',
      };
      
      await this.msalInstance.loginRedirect(loginRequest);
    } catch (error) {
      console.error('Login with redirect failed', error);
      this._interactionStatus.set(InteractionStatus.None);
      throw error;
    }
  }

  async logout(): Promise<void> {
    try {
      await this.ensureInitialized();
      const logoutRequest: EndSessionRequest = {
        account: this.msalInstance.getActiveAccount(),
        postLogoutRedirectUri: msalConfig.auth.postLogoutRedirectUri,
      };
      
      await this.msalInstance.logoutPopup(logoutRequest);
      this.setActiveAccount(null);
    } catch (error) {
      console.error('Logout failed', error);
      throw error;
    }
  }

  async getAccessToken(scopes: string[] = defaultLoginRequest.scopes): Promise<string | null> {
    await this.ensureInitialized();
    
    const account = this.msalInstance.getActiveAccount();
    if (!account) {
      throw new Error('No active account found');
    }
    console.log(scopes)
    try {
      const silentRequest: SilentRequest = {
        scopes,
        account,
      };
      
      const result = await this.msalInstance.acquireTokenSilent(silentRequest);
      return result.accessToken;
    } catch (error) {
      console.warn('Silent token acquisition failed, trying popup', error);
      
      try {
        const popupRequest: PopupRequest = {
          scopes,
          account,
        };
        
        const result = await this.msalInstance.acquireTokenPopup(popupRequest);
        return result.accessToken;
      } catch (popupError) {
        console.error('Token acquisition failed', popupError);
        throw popupError;
      }
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
