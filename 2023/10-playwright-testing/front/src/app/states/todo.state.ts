import { computed, signal } from "@angular/core";
import { ToDo } from "../types/todo.type";

export class TodoState {
  private _todo = signal<ToDo[]>([]);
  todo = this._todo.asReadonly();

  completedTodos = computed(() => {
    return this._todo().filter(f => f.status === 'completed');
  });

  inCompletedTodos = computed(() => {
    return this._todo().filter(f => f.status !== 'completed');
  });

  allTodos = computed(() => {
    return this._todo();
  });

  addTodos(todos: ToDo[]) {
    this._todo.update(_ => todos);
  }

  addTodo(item: ToDo) {
    this._todo.update((todos) => {
      return [
        item,
        ...todos
      ]
    });
  }

  changeState(id: ToDo['id'], status: ToDo['status']) {
    this._todo.mutate(todos => {
      const todo = todos.find(f => f.id === id);
      if (todo) {
        todo.status = status;
      }
    });
  }
}
