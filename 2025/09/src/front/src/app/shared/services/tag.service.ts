import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Tag } from '../models/todo.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiConfig.uri; // 環境設定からAPIのベースURLを取得

  // タグ一覧取得
  getTags(): Observable<Tag[]> {
    console.log('TagService: Making request to:', `${this.baseUrl}/tags`);
    return this.http.get<Tag[]>(`${this.baseUrl}/tags`);
  }

  // タグ作成
  createTag(name: string, color?: string): Observable<Tag> {
    console.log('TagService: Creating tag with potential delay for Application Insights testing');
    const startTime = performance.now();
    
    return this.http.post<Tag>(`${this.baseUrl}/tags`, { tagName: name, colorCode: color }).pipe(
      tap({
        next: (tag: Tag) => {
          const endTime = performance.now();
          const duration = endTime - startTime;
          console.log(`TagService: Tag created successfully in ${duration.toFixed(2)}ms`, tag);
        },
        error: (error: any) => {
          const endTime = performance.now();
          const duration = endTime - startTime;
          console.error(`TagService: Tag creation failed after ${duration.toFixed(2)}ms`, error);
        }
      })
    );
  }

  // タグ削除
  deleteTag(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/tags/${id}`);
  }
}
