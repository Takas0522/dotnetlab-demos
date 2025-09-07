import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TodoService } from '../../shared/services/todo.service';
import { Todo } from '../../shared/models/todo.model';

@Component({
  selector: 'app-shared-todos',
  imports: [CommonModule],
  templateUrl: './shared-todos.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SharedTodosComponent {
  private readonly todoService = inject(TodoService);
  private readonly router = inject(Router);

  sharedTodos = signal<Todo[]>([]);
  isLoading = signal(false);

  ngOnInit() {
    this.loadSharedTodos();
  }

  async loadSharedTodos() {
    this.isLoading.set(true);
    try {
      this.todoService.getSharedTodos().subscribe({
        next: (todos) => {
          this.sharedTodos.set(todos);
          this.isLoading.set(false);
        },
        error: (error) => {
          console.error('Error loading shared todos:', error);
          this.isLoading.set(false);
        }
      });
    } catch (error) {
      console.error('Error loading shared todos:', error);
      this.isLoading.set(false);
    }
  }

  navigateToDetail(todoId: string) {
    this.router.navigate(['/todos', todoId]);
  }

  goBack() {
    this.router.navigate(['/todos']);
  }
}
