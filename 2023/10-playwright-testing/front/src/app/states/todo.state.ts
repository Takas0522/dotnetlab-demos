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

  addTodo(description: string) {
    this._todo.update((todos) => {
      const newTodo: ToDo = {
        id: todos.length + 1,
        description: description,
        status: 'incompleted',
        addDate: new Date(),
        updateDate: new Date()
      };
      return [
        newTodo,
        ...todos
      ]
    });
  }

  changeState(id: ToDo['id'], status: ToDo['status']) {
    console.log({id, status})
    this._todo.mutate(todos => {
      const todo = todos.find(f => f.id === id);
      if (todo) {
        todo.status = status;
      }
    });
  }
}
