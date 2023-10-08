import { Component, Signal, signal } from '@angular/core';
import { inject } from '@angular/core';
import { TodoService } from 'src/app/services/toso.service';
import { ToDo } from 'src/app/types/todo.type';

@Component({
  selector: 'app-to-do-list',
  templateUrl: './to-do-list.component.html',
  styleUrls: ['./to-do-list.component.scss']
})
export class ToDoListComponent {
  protected service = inject(TodoService);

  get todos(): Signal<ToDo[]> {
    return this.service.state$;
  }

  statusChange(id: ToDo['id'], status: ToDo['status']) {
    this.service.setTodo(id, status);
  }
}
