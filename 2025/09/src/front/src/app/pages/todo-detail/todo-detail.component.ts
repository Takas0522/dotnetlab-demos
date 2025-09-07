import { ChangeDetectionStrategy, Component, inject, signal, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService } from '../../shared/services/todo.service';
import { TagService } from '../../shared/services/tag.service';
import { Todo, Tag, UpdateTodoRequest, ShareTodoRequest } from '../../shared/models/todo.model';

@Component({
  selector: 'app-todo-detail',
  imports: [CommonModule, FormsModule],
  templateUrl: './todo-detail.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoDetailComponent {
  private readonly todoService = inject(TodoService);
  private readonly tagService = inject(TagService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  todo = signal<Todo | null>(null);
  tags = signal<Tag[]>([]);
  isLoading = signal(false);
  isEditing = signal(false);
  showShareForm = signal(false);

  // 編集フォーム
  editForm = signal<UpdateTodoRequest>({
    title: '',
    description: '',
    isCompleted: false,
    tagIds: []
  });

  // 共有フォーム
  shareForm = signal<ShareTodoRequest>({
    userEmail: '',
    permissions: 'READ'
  });

  ngOnInit() {
    const todoId = this.route.snapshot.paramMap.get('id');
    if (todoId) {
      this.loadTodo(todoId);
      this.loadTags();
    }
  }

  async loadTodo(id: string) {
    this.isLoading.set(true);
    try {
      this.todoService.getTodo(id).subscribe({
        next: (todo) => {
          this.todo.set(todo);
          this.editForm.set({
            title: todo.title,
            description: todo.description || '',
            isCompleted: todo.isCompleted,
            tagIds: todo.tags.map(tag => tag.tagId)
          });
          this.isLoading.set(false);
        },
        error: (error) => {
          console.error('Error loading todo:', error);
          this.isLoading.set(false);
        }
      });
    } catch (error) {
      console.error('Error loading todo:', error);
      this.isLoading.set(false);
    }
  }

  async loadTags() {
    try {
      this.tagService.getTags().subscribe({
        next: (tags) => this.tags.set(tags),
        error: (error) => console.error('Error loading tags:', error)
      });
    } catch (error) {
      console.error('Error loading tags:', error);
    }
  }

  async updateTodo() {
    const todo = this.todo();
    if (!todo) return;

    try {
      this.todoService.updateTodo(todo.todoItemId, this.editForm()).subscribe({
        next: (updatedTodo) => {
          this.todo.set(updatedTodo);
          this.isEditing.set(false);
        },
        error: (error) => console.error('Error updating todo:', error)
      });
    } catch (error) {
      console.error('Error updating todo:', error);
    }
  }

  async shareTodo() {
    const todo = this.todo();
    if (!todo || !this.shareForm().userEmail.trim()) return;

    try {
      this.todoService.shareTodo(todo.todoItemId, this.shareForm()).subscribe({
        next: (sharedTodo) => {
          // ToDoを再読み込みして共有情報を更新
          this.loadTodo(todo.todoItemId);
          this.shareForm.set({ userEmail: '', permissions: 'READ' });
          this.showShareForm.set(false);
        },
        error: (error) => console.error('Error sharing todo:', error)
      });
    } catch (error) {
      console.error('Error sharing todo:', error);
    }
  }

  async unshareTodo(shareId: string) {
    const todo = this.todo();
    if (!todo) return;

    try {
      this.todoService.unshareTodo(todo.todoItemId, shareId).subscribe({
        next: () => {
          this.loadTodo(todo.todoItemId);
        },
        error: (error) => console.error('Error unsharing todo:', error)
      });
    } catch (error) {
      console.error('Error unsharing todo:', error);
    }
  }

  async deleteTodo() {
    const todo = this.todo();
    if (!todo || !confirm('このToDoを削除しますか？')) return;

    try {
      this.todoService.deleteTodo(todo.todoItemId).subscribe({
        next: () => {
          this.router.navigate(['/todos']);
        },
        error: (error) => console.error('Error deleting todo:', error)
      });
    } catch (error) {
      console.error('Error deleting todo:', error);
    }
  }

  toggleTagSelection(tagId: string) {
    this.editForm.update(form => ({
      ...form,
      tagIds: form.tagIds?.includes(tagId)
        ? form.tagIds.filter(id => id !== tagId)
        : [...(form.tagIds || []), tagId]
    }));
  }

  cancelEdit() {
    const todo = this.todo();
    if (todo) {
      this.editForm.set({
        title: todo.title,
        description: todo.description || '',
        isCompleted: todo.isCompleted,
        tagIds: todo.tags.map(tag => tag.tagId)
      });
    }
    this.isEditing.set(false);
  }

  goBack() {
    this.router.navigate(['/todos']);
  }

  // テンプレート用のヘルパーメソッド
  updateTitle(value: string) {
    this.editForm.update(form => ({ ...form, title: value }));
  }

  updateDescription(value: string) {
    this.editForm.update(form => ({ ...form, description: value }));
  }

  updateCompleted(value: boolean) {
    this.editForm.update(form => ({ ...form, isCompleted: value }));
  }

  updateShareEmail(value: string) {
    this.shareForm.update(form => ({ ...form, userEmail: value }));
  }

  updateSharePermissions(value: string) {
    this.shareForm.update(form => ({ ...form, permissions: value as 'READ' | 'WRITE' }));
  }
}
