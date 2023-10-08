import { Injectable, OnInit } from '@angular/core';
import { TodoState } from '../states/todo.state';
import { ToDo } from '../types/todo.type';

@Injectable({
  providedIn: 'root'
})
export class TodoService {

  private _state = new TodoState();

  state$ = this._state.inCompletedTodos;

  addTodo(description: string) {
    this._state.addTodo(description);
    // TODO: Add API call to add todo
  }

  setTodo(id: ToDo['id'], status: ToDo['status']) {
    this._state.changeState(id, status);
    // TODO: Update API call to update todo
  }

  showTodosChange(isShowAll: boolean) {
    if (isShowAll) {
      this.state$ = this._state.allTodos;
    } else {
      this.state$ = this._state.inCompletedTodos;
    }
  }

}