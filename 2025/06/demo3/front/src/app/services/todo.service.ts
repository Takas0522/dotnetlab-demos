import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Todo, CreateTodo, UpdateTodo } from '../models/todo.model';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private readonly apiUrl = '/api/todos';

  constructor(private http: HttpClient) {}

  getTodosByUserId(userId: number, categoryId?: number, status?: number): Observable<Todo[]> {
    let params = new HttpParams();
    if (categoryId) {
      params = params.set('categoryId', categoryId.toString());
    }
    if (status) {
      params = params.set('status', status.toString());
    }

    return this.http.get<Todo[]>(`${this.apiUrl}/user/${userId}`, { params });
  }

  getTodoById(id: number, userId: number): Observable<Todo> {
    return this.http.get<Todo>(`${this.apiUrl}/${id}/user/${userId}`);
  }

  createTodo(todo: CreateTodo): Observable<Todo> {
    return this.http.post<Todo>(this.apiUrl, todo);
  }

  updateTodo(id: number, userId: number, todo: UpdateTodo): Observable<Todo> {
    return this.http.put<Todo>(`${this.apiUrl}/${id}/user/${userId}`, todo);
  }

  deleteTodo(id: number, userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/user/${userId}`);
  }

  updateTodoStatus(id: number, userId: number, status: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/user/${userId}/status`, status);
  }
}
