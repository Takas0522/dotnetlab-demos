import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
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
    return this.http.post<Tag>(`${this.baseUrl}/tags`, { name, color });
  }

  // タグ削除
  deleteTag(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/tags/${id}`);
  }
}
