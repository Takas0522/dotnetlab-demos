import { ChangeDetectionStrategy, Component, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService } from '../../shared/services/todo.service';
import { TagService } from '../../shared/services/tag.service';
import { Todo, Tag, CreateTodoRequest } from '../../shared/models/todo.model';

@Component({
  selector: 'app-todo-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './todo-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TodoListComponent {
  private readonly todoService = inject(TodoService);
  private readonly tagService = inject(TagService);
  private readonly router = inject(Router);

  protected readonly todos = signal<Todo[]>([]);
  protected readonly tags = signal<Tag[]>([]);
  protected readonly selectedTags = signal<string[]>([]);
  protected readonly searchQuery = signal('');
  protected readonly showCompleted = signal(true);
  protected readonly isLoading = signal(false);
  protected readonly showCreateForm = signal(false);
  protected readonly currentFilter = signal<'all' | 'pending' | 'completed'>('all');

  // 新しいToDo作成フォーム
  protected readonly newTodo = signal<CreateTodoRequest>({
    title: '',
    description: '',
    tagIds: []
  });

  // フィルターされたToDo一覧
  protected readonly filteredTodos = computed(() => {
    let filtered = this.todos();
    
    // ステータスフィルタ
    if (this.currentFilter() === 'pending') {
      filtered = filtered.filter(todo => !todo.isCompleted);
    } else if (this.currentFilter() === 'completed') {
      filtered = filtered.filter(todo => todo.isCompleted);
    }
    
    // 検索クエリでフィルタ
    if (this.searchQuery()) {
      const query = this.searchQuery().toLowerCase();
      filtered = filtered.filter(todo => 
        todo.title.toLowerCase().includes(query) || 
        todo.description?.toLowerCase().includes(query)
      );
    }

    // タグでフィルタ
    if (this.selectedTags().length > 0) {
      filtered = filtered.filter(todo =>
        this.selectedTags().some(tagId => 
          todo.tags.some(tag => tag.tagId === tagId)
        )
      );
    }

    return filtered;
  });

  // 全てのToDo一覧（フィルタなし）
  protected readonly allTodos = computed(() => this.todos());

  ngOnInit() {
    this.loadTodos();
    this.loadTags();
  }

  async loadTodos() {
    this.isLoading.set(true);
    try {
      this.todoService.getTodos().subscribe({
        next: (todos) => {
          this.todos.set(todos);
          this.isLoading.set(false);
        },
        error: (error) => {
          console.error('Error loading todos:', error);
          this.isLoading.set(false);
        }
      });
    } catch (error) {
      console.error('Error loading todos:', error);
      this.isLoading.set(false);
    }
  }

  async loadTags() {
    try {
      console.log('Loading tags...');
      this.tagService.getTags().subscribe({
        next: (tags) => {
          console.log('Tags loaded successfully:', tags);
          this.tags.set(tags);
        },
        error: (error) => {
          console.error('Error loading tags:', error);
          console.error('Error status:', error.status);
          console.error('Error message:', error.message);
        }
      });
    } catch (error) {
      console.error('Error in loadTags:', error);
    }
  }

  async createTodo() {
    if (!this.newTodo().title.trim()) return;

    try {
      this.todoService.createTodo(this.newTodo()).subscribe({
        next: (todo) => {
          this.todos.update(todos => [...todos, todo]);
          this.newTodo.set({ title: '', description: '', tagIds: [] });
          this.showCreateForm.set(false);
        },
        error: (error) => console.error('Error creating todo:', error)
      });
    } catch (error) {
      console.error('Error creating todo:', error);
    }
  }

  async toggleTodoCompletion(todoId: string, event: Event) {
    const target = event.target as HTMLInputElement;
    const isCompleted = target.checked;
    try {
      this.todoService.updateTodo(todoId, { isCompleted }).subscribe({
        next: (updatedTodo) => {
          this.todos.update(todos => 
            todos.map(t => t.todoItemId === todoId ? updatedTodo : t)
          );
        },
        error: (error) => console.error('Error updating todo:', error)
      });
    } catch (error) {
      console.error('Error updating todo:', error);
    }
  }

  async deleteTodo(todoId: string) {
    if (!confirm('このToDoを削除しますか？')) return;

    try {
      this.todoService.deleteTodo(todoId).subscribe({
        next: () => {
          this.todos.update(todos => todos.filter(t => t.todoItemId !== todoId));
        },
        error: (error) => console.error('Error deleting todo:', error)
      });
    } catch (error) {
      console.error('Error deleting todo:', error);
    }
  }

  navigateToDetail(todoId: string) {
    this.router.navigate(['/todos', todoId]);
  }

  toggleTagFilter(tagId: string) {
    this.selectedTags.update(tags => 
      tags.includes(tagId) 
        ? tags.filter(id => id !== tagId)
        : [...tags, tagId]
    );
  }

  clearFilters() {
    this.selectedTags.set([]);
    this.searchQuery.set('');
  }

  // テンプレート用のヘルパーメソッド
  updateNewTodoTitle(event: Event) {
    const target = event.target as HTMLInputElement;
    this.newTodo.update(todo => ({ ...todo, title: target.value }));
  }

  updateNewTodoDescription(event: Event) {
    const target = event.target as HTMLTextAreaElement;
    this.newTodo.update(todo => ({ ...todo, description: target.value }));
  }

  setFilter(filter: 'all' | 'pending' | 'completed') {
    this.currentFilter.set(filter);
  }

  setSearchQuery(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchQuery.set(target.value);
  }

  selectedTagId() {
    return this.selectedTags().length === 1 ? this.selectedTags()[0] : null;
  }

  setTagFilter(tagId: string) {
    this.selectedTags.set([tagId]);
  }

  clearTagFilter() {
    this.selectedTags.set([]);
  }

  clearAllFilters() {
    this.selectedTags.set([]);
    this.searchQuery.set('');
    this.currentFilter.set('all');
  }

  viewTodoDetail(todoId: string) {
    this.router.navigate(['/todos', todoId]);
  }
}
