import { Injectable, inject } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  canActivate(): Observable<boolean> {
    if (this.authService.isAuthenticated()) {
      return of(true);
    }

    // 認証されていない場合、ログインページにリダイレクト
    this.router.navigate(['/login']);
    return of(false);
  }
}
