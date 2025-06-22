import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Todo } from '../models/todo.model';


@Injectable({
    providedIn: 'root'
})
export class TodoService {
    private apiUrl = '/api/todos';

    private readonly http = inject(HttpClient);

    async getTodos(): Promise<Todo[]> {
        return await firstValueFrom(this.http.get<Todo[]>(this.apiUrl));
    }

    async addTodo(title: string): Promise<Todo> {
        return await firstValueFrom(this.http.post<Todo>(this.apiUrl, title, {
            headers: { 'Content-Type': 'application/json' }
        }));
    }

    async updateTodo(todo: Todo): Promise<Todo> {
        return await firstValueFrom(
            this.http.put<Todo>(`${this.apiUrl}/${todo.id}`, {
                title: todo.title,
                isCompleted: todo.isCompleted
            }, {
                headers: { 'Content-Type': 'application/json' }
            })
        );
    }

    async deleteTodo(id: number): Promise<void> {
        await firstValueFrom(this.http.delete(`${this.apiUrl}/${id}`));
    }
}