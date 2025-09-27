import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  async ngOnInit(): Promise<void> {
    // MSAL初期化の完了を待つ
    try {
      // 認証サービスの初期化が完了するまで待機
      while (!this.authService.isInitialized()) {
        await new Promise(resolve => setTimeout(resolve, 100));
      }
      
      // 認証状態に基づいて初期ナビゲーションを設定
      this.handleInitialNavigation();
    } catch (error) {
      console.error('Error during app initialization:', error);
      this.router.navigate(['/login']);
    }
  }

  private handleInitialNavigation(): void {
    const currentUrl = this.router.url;
    
    if (this.authService.isAuthenticated()) {
      // 既にログイン済みの場合
      if (currentUrl === '/login' || currentUrl === '/' || currentUrl === '') {
        console.log('User already authenticated, redirecting to dashboard');
        this.router.navigate(['/dashboard']);
      }
    } else {
      // 未認証の場合
      if (currentUrl !== '/login') {
        console.log('User not authenticated, redirecting to login');
        this.router.navigate(['/login']);
      }
    }
  }
}
