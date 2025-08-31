import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Tag } from '../models/todo.model';

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'https://localhost:7001/api'; // APIのベースURL

  private get headers(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
    });
  }

  // タグ一覧取得
  getTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.baseUrl}/tags`, { headers: this.headers });
  }

  // タグ作成
  createTag(name: string, color?: string): Observable<Tag> {
    return this.http.post<Tag>(`${this.baseUrl}/tags`, { name, color }, { headers: this.headers });
  }

  // タグ削除
  deleteTag(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/tags/${id}`, { headers: this.headers });
  }
}
