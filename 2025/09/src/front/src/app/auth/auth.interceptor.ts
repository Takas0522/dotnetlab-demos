import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, from, switchMap } from 'rxjs';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private readonly authService = inject(AuthService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // APIリクエストのみを対象とする（Microsoft Graph APIやアプリのAPIなど）
    if (this.shouldAddToken(req.url)) {
      console.log('AuthInterceptor: Processing API request to:', req.url);
      
      // 認証されている場合のみトークンを追加
      if (this.authService.isAuthenticated()) {
        console.log('AuthInterceptor: User is authenticated, getting access token');
        return from(this.authService.getAccessToken()).pipe(
          switchMap(token => {
            if (token) {
              console.log('AuthInterceptor: Token acquired, adding to headers');
              const authReq = req.clone({
                headers: req.headers.set('Authorization', `Bearer ${token}`)
              });
              return next.handle(authReq);
            }
            console.log('AuthInterceptor: No token available');
            return next.handle(req);
          })
        );
      } else {
        console.log('AuthInterceptor: User is not authenticated');
      }
    } else {
      console.log('AuthInterceptor: Skipping token for URL:', req.url);
    }
    
    return next.handle(req);
  }

  private shouldAddToken(url: string): boolean {
    // Microsoft Graph APIやアプリのAPIなど、トークンが必要なURLを指定
    const protectedUrls = [
      'https://graph.microsoft.com',
      environment.apiConfig.uri,
      '/api/', // 相対パスのAPI
      'http://localhost:5115/api' // 直接API呼び出し
    ];
    
    console.log('AuthInterceptor: Checking URL for token:', url);
    const shouldAdd = protectedUrls.some(protectedUrl => url.includes(protectedUrl));
    console.log('AuthInterceptor: Should add token:', shouldAdd);
    return shouldAdd;
  }
}
