import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo, CreateTodoRequest, UpdateTodoRequest, Tag, ShareTodoRequest, SharedTodo } from '../models/todo.model';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api'; // APIのベースURL

  private get headers(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json',
      // 認証ヘッダーが必要な場合はここに追加
    });
  }

  // ToDo一覧取得
  getTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(`${this.baseUrl}/todoItems`, { headers: this.headers });
  }

  // ToDo詳細取得
  getTodo(id: number): Observable<Todo> {
    return this.http.get<Todo>(`${this.baseUrl}/todoItems/${id}`, { headers: this.headers });
  }

  // ToDo作成
  createTodo(todo: CreateTodoRequest): Observable<Todo> {
    return this.http.post<Todo>(`${this.baseUrl}/todoItems`, todo, { headers: this.headers });
  }

  // ToDo更新
  updateTodo(id: number, todo: UpdateTodoRequest): Observable<Todo> {
    return this.http.put<Todo>(`${this.baseUrl}/todoItems/${id}`, todo, { headers: this.headers });
  }

  // ToDo削除
  deleteTodo(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/todoItems/${id}`, { headers: this.headers });
  }

  // 共有ToDo一覧取得
  getSharedTodos(): Observable<Todo[]> {
    return this.http.get<Todo[]>(`${this.baseUrl}/todoItems/shared`, { headers: this.headers });
  }

  // ToDoを共有
  shareTodo(todoId: number, shareRequest: ShareTodoRequest): Observable<SharedTodo> {
    return this.http.post<SharedTodo>(`${this.baseUrl}/todoItems/${todoId}/share`, shareRequest, { headers: this.headers });
  }

  // 共有解除
  unshareTodo(todoId: number, shareId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/todoItems/${todoId}/share/${shareId}`, { headers: this.headers });
  }
}
