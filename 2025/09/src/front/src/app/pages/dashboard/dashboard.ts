import { ChangeDetectionStrategy, Component, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly account = this.authService.account;
  protected readonly isLoggingOut = signal(false);
  
  protected readonly displayName = computed(() => {
    const account = this.account();
    return account?.name || account?.username || 'ユーザー';
  });

  async logout(): Promise<void> {
    try {
      this.isLoggingOut.set(true);
      await this.authService.logout();
      this.router.navigate(['/login']);
    } catch (error) {
      console.error('Logout failed:', error);
    } finally {
      this.isLoggingOut.set(false);
    }
  }

  navigateToTodos(): void {
    this.router.navigate(['/todos']);
  }

  navigateToProfile(): void {
    // プロフィール機能は今後実装予定
    console.log('プロフィール機能は準備中です');
  }
}
