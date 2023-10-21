import { Injectable, OnInit, inject, signal } from '@angular/core';
import { TodoState } from '../states/todo.state';
import { ToDo } from '../types/todo.type';
import { HttpClient } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class TodoService {

  private _state = new TodoState();
  private client = inject(HttpClient);
  private sb = inject(MatSnackBar);

  state$ = this._state.inCompletedTodos;
  canNotEdit$ = signal<boolean>(true);

  constructor() {
    this.getAllData();
  }

  getAllData() {
    this.canNotEdit$.update(_ => true);
    this.client.get<ToDo[]>('/api/todo').subscribe(data => {
      this._state.addTodos(data);
      this.canNotEdit$.update(_ => false);
    });
  }

  addTodo(description: string) {
    this.canNotEdit$.update(_ => true);
    this.client.post<ToDo>('/api/todo', { description }).subscribe(data => {
      this._state.addTodo(data);
      this.sb.open('ToDoが登録されました', 'close', { duration: 2000 })
      this.canNotEdit$.update(_ => false);
    });
  }

  setTodo(id: ToDo['id'], status: ToDo['status']) {
    this.canNotEdit$.update(_ => true);
    this.client.put('/api/todo', { id }).subscribe(data => {
      this._state.changeState(id, status);
      this.sb.open('ToDoが登録されました', 'close', { duration: 2000 })
      this.canNotEdit$.update(_ => false);
    });
  }

  showTodosChange(isShowAll: boolean) {
    if (isShowAll) {
      this.state$ = this._state.allTodos;
    } else {
      this.state$ = this._state.inCompletedTodos;
    }
  }

}