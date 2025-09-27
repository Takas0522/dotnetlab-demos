import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface User {
  id: string;
  entraId: string;
  userPrincipalName: string;
  displayName: string;
  email: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiConfig.uri;

  /**
   * 現在のユーザー情報を取得または作成する
   * バックエンドで自動的にユーザーが存在しない場合は作成される
   */
  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/users/me`).pipe(
      retry(2), // 失敗時に2回まで再試行
      catchError(this.handleError)
    );
  }

  /**
   * EntraIDでユーザー情報を取得する
   */
  getUserByEntraId(entraId: string): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/users/${entraId}`).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * HTTPエラーハンドラー
   */
  private handleError = (error: HttpErrorResponse): Observable<never> => {
    let errorMessage = 'ユーザー情報の処理中にエラーが発生しました';
    
    if (error.error instanceof ErrorEvent) {
      // クライアントサイドエラー
      errorMessage = `ネットワークエラー: ${error.error.message}`;
    } else {
      // サーバーサイドエラー
      switch (error.status) {
        case 401:
          errorMessage = '認証が必要です。再度ログインしてください。';
          break;
        case 403:
          errorMessage = 'アクセス権限がありません。';
          break;
        case 404:
          errorMessage = 'ユーザー情報が見つかりません。';
          break;
        case 500:
          errorMessage = 'サーバーエラーが発生しました。しばらく経ってから再試行してください。';
          break;
        default:
          errorMessage = `エラーが発生しました (${error.status}): ${error.message}`;
      }
    }
    
    console.error('UserService Error:', errorMessage, error);
    return throwError(() => new Error(errorMessage));
  };
}