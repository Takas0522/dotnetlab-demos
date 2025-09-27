import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  
  protected readonly isLoading = signal(false);
  protected readonly error = signal<string | null>(null);

  async loginWithPopup(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);
      
      await this.authService.loginWithPopup();
      
      // AuthServiceが自動的にダッシュボードにリダイレクトします
    } catch (error) {
      console.error('Login failed:', error);
      this.error.set('ログインに失敗しました。もう一度お試しください。');
    } finally {
      this.isLoading.set(false);
    }
  }

  async loginWithRedirect(): Promise<void> {
    try {
      this.isLoading.set(true);
      this.error.set(null);
      
      await this.authService.loginWithRedirect();
    } catch (error) {
      console.error('Login failed:', error);
      this.error.set('ログインに失敗しました。もう一度お試しください。');
      this.isLoading.set(false);
    }
  }
}
