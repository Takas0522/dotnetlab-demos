import { ChangeDetectionStrategy, Component, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService } from '../../shared/services/todo.service';
import { TagService } from '../../shared/services/tag.service';
import { ApplicationInsightsService } from '../../services/application-insights.service';
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
  private readonly appInsights = inject(ApplicationInsightsService);

  protected readonly todos = signal<Todo[]>([]);
  protected readonly tags = signal<Tag[]>([]);
  protected readonly selectedTags = signal<string[]>([]);
  protected readonly searchQuery = signal('');
  protected readonly showCompleted = signal(true);
  protected readonly isLoading = signal(false);
  protected readonly isCreatingTag = signal(false);
  protected readonly showCreateForm = signal(false);
  protected readonly currentFilter = signal<'all' | 'pending' | 'completed'>('all');
  protected readonly showCreateTagForm = signal(false);

  // 新しいToDo作成フォーム
  protected readonly newTodo = signal<CreateTodoRequest>({
    title: '',
    description: '',
    tagIds: []
  });

  // 新しいタグ作成フォーム  
  protected readonly newTag = signal({
    name: '',
    color: '#3B82F6'
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
    // ページビューの追跡
    this.appInsights.trackPageView('TodoList', '/todos');
    this.appInsights.trackEvent('TodoList.PageLoad');
    
    this.loadTodos();
    this.loadTags();
  }

  async loadTodos() {
    const operationName = 'LoadTodos';
    this.isLoading.set(true);
    
    try {
      await this.appInsights.measurePerformance(operationName, async () => {
        return new Promise<void>((resolve, reject) => {
          this.todoService.getTodos().subscribe({
            next: (todos) => {
              this.todos.set(todos);
              this.isLoading.set(false);
              
              // メトリクス追跡
              this.appInsights.trackMetric('TodoList.LoadedCount', todos.length);
              this.appInsights.trackEvent('TodoList.LoadSuccess', {
                todoCount: todos.length.toString()
              });
              
              resolve();
            },
            error: (error) => {
              console.error('Error loading todos:', error);
              this.isLoading.set(false);
              
              this.appInsights.trackException(error, {
                operation: operationName,
                errorContext: 'Loading todos from service'
              });
              
              reject(error);
            }
          });
        });
      });
    } catch (error) {
      console.error('Error loading todos:', error);
      this.isLoading.set(false);
    }
  }

  async loadTags() {
    const operationName = 'LoadTags';
    
    try {
      console.log('Loading tags...');
      await this.appInsights.measurePerformance(operationName, async () => {
        return new Promise<void>((resolve, reject) => {
          this.tagService.getTags().subscribe({
            next: (tags) => {
              console.log('Tags loaded successfully:', tags);
              this.tags.set(tags);
              
              // メトリクス追跡
              this.appInsights.trackMetric('TodoList.TagsLoadedCount', tags.length);
              this.appInsights.trackEvent('TodoList.TagsLoadSuccess', {
                tagCount: tags.length.toString()
              });
              
              resolve();
            },
            error: (error) => {
              console.error('Error loading tags:', error);
              console.error('Error status:', error.status);
              console.error('Error message:', error.message);
              
              this.appInsights.trackException(error, {
                operation: operationName,
                errorContext: 'Loading tags from service',
                errorStatus: error.status?.toString() || 'Unknown'
              });
              
              reject(error);
            }
          });
        });
      });
    } catch (error) {
      console.error('Error in loadTags:', error);
    }
  }

  async createTodo() {
    if (!this.newTodo().title.trim()) return;

    const operationName = 'CreateTodo';
    
    try {
      await this.appInsights.measurePerformance(operationName, async () => {
        return new Promise<void>((resolve, reject) => {
          this.todoService.createTodo(this.newTodo()).subscribe({
            next: (todo) => {
              this.todos.update(todos => [...todos, todo]);
              this.newTodo.set({ title: '', description: '', tagIds: [] });
              this.showCreateForm.set(false);
              
              // イベント追跡
              this.appInsights.trackEvent('TodoList.TodoCreated', {
                todoTitle: todo.title,
                hasDescription: (!!todo.description).toString(),
                tagCount: todo.tags.length.toString()
              });
              
              resolve();
            },
            error: (error) => {
              console.error('Error creating todo:', error);
              
              this.appInsights.trackException(error, {
                operation: operationName,
                errorContext: 'Creating new todo',
                todoTitle: this.newTodo().title
              });
              
              reject(error);
            }
          });
        });
      });
    } catch (error) {
      console.error('Error creating todo:', error);
    }
  }

  async createTag() {
    if (!this.newTag().name.trim()) return;

    const operationName = 'CreateTag';
    this.isCreatingTag.set(true);
    
    try {
      console.log('Creating tag with delay for Application Insights testing...');
      await this.appInsights.measurePerformance(operationName, async () => {
        return new Promise<void>((resolve, reject) => {
          this.tagService.createTag(this.newTag().name, this.newTag().color).subscribe({
            next: (tag) => {
              this.tags.update(tags => [...tags, tag]);
              this.newTag.set({ name: '', color: '#3B82F6' });
              this.showCreateTagForm.set(false);
              
              // イベント追跡
              this.appInsights.trackEvent('TodoList.TagCreated', {
                tagName: tag.tagName,
                tagColor: tag.colorCode || 'default'
              });
              
              console.log('Tag created successfully with potential delay:', tag);
              resolve();
            },
            error: (error) => {
              console.error('Error creating tag:', error);
              
              this.appInsights.trackException(error, {
                operation: operationName,
                errorContext: 'Creating new tag',
                tagName: this.newTag().name
              });
              
              reject(error);
            }
          });
        });
      });
    } catch (error) {
      console.error('Error creating tag:', error);
    } finally {
      this.isCreatingTag.set(false);
    }
  }

  async toggleTodoCompletion(todoId: string, event: Event) {
    const target = event.target as HTMLInputElement;
    const isCompleted = target.checked;
    const operationName = 'ToggleTodoCompletion';
    
    try {
      await this.appInsights.measurePerformance(operationName, async () => {
        return new Promise<void>((resolve, reject) => {
          this.todoService.updateTodo(todoId, { isCompleted }).subscribe({
            next: (updatedTodo) => {
              this.todos.update(todos => 
                todos.map(t => t.todoItemId === todoId ? updatedTodo : t)
              );
              
              // イベント追跡
              this.appInsights.trackEvent('TodoList.TodoToggled', {
                todoId,
                newStatus: isCompleted ? 'completed' : 'pending',
                todoTitle: updatedTodo.title
              });
              
              resolve();
            },
            error: (error) => {
              console.error('Error updating todo:', error);
              
              this.appInsights.trackException(error, {
                operation: operationName,
                errorContext: 'Toggling todo completion status',
                todoId,
                targetStatus: isCompleted ? 'completed' : 'pending'
              });
              
              reject(error);
            }
          });
        });
      });
    } catch (error) {
      console.error('Error updating todo:', error);
    }
  }

  async deleteTodo(todoId: string) {
    if (!confirm('このToDoを削除しますか？')) {
      // ユーザーがキャンセルしたことを追跡
      this.appInsights.trackEvent('TodoList.DeleteCancelled', { todoId });
      return;
    }

    const operationName = 'DeleteTodo';
    
    try {
      await this.appInsights.measurePerformance(operationName, async () => {
        return new Promise<void>((resolve, reject) => {
          this.todoService.deleteTodo(todoId).subscribe({
            next: () => {
              this.todos.update(todos => todos.filter(t => t.todoItemId !== todoId));
              
              // イベント追跡
              this.appInsights.trackEvent('TodoList.TodoDeleted', {
                todoId
              });
              
              resolve();
            },
            error: (error) => {
              console.error('Error deleting todo:', error);
              
              this.appInsights.trackException(error, {
                operation: operationName,
                errorContext: 'Deleting todo',
                todoId
              });
              
              reject(error);
            }
          });
        });
      });
    } catch (error) {
      console.error('Error deleting todo:', error);
    }
  }

  navigateToDetail(todoId: string) {
    this.router.navigate(['/todos', todoId]);
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

  // タグ作成フォーム用のヘルパーメソッド
  updateNewTagName(event: Event) {
    const target = event.target as HTMLInputElement;
    this.newTag.update(tag => ({ ...tag, name: target.value }));
  }

  updateNewTagColor(event: Event) {
    const target = event.target as HTMLInputElement;
    this.newTag.update(tag => ({ ...tag, color: target.value }));
  }

  setFilter(filter: 'all' | 'pending' | 'completed') {
    const previousFilter = this.currentFilter();
    this.currentFilter.set(filter);
    
    // フィルター変更の追跡
    this.appInsights.trackEvent('TodoList.FilterChanged', {
      previousFilter,
      newFilter: filter,
      filteredCount: this.filteredTodos().length.toString()
    });
  }

  setSearchQuery(event: Event) {
    const target = event.target as HTMLInputElement;
    const query = target.value;
    this.searchQuery.set(query);
    
    // 検索の追跡（デバウンスなしで即座に）
    if (query.length > 2) {
      this.appInsights.trackEvent('TodoList.SearchPerformed', {
        searchQuery: query,
        resultCount: this.filteredTodos().length.toString()
      });
    }
  }

  toggleTagFilter(tagId: string) {
    const wasSelected = this.selectedTags().includes(tagId);
    this.selectedTags.update(tags => 
      tags.includes(tagId) 
        ? tags.filter(id => id !== tagId)
        : [...tags, tagId]
    );
    
    // タグフィルター変更の追跡
    this.appInsights.trackEvent('TodoList.TagFilterToggled', {
      tagId,
      action: wasSelected ? 'removed' : 'added',
      totalSelectedTags: this.selectedTags().length.toString(),
      filteredCount: this.filteredTodos().length.toString()
    });
  }

  clearAllFilters() {
    const hadFilters = this.selectedTags().length > 0 || 
                      this.searchQuery() !== '' || 
                      this.currentFilter() !== 'all';
    
    this.selectedTags.set([]);
    this.searchQuery.set('');
    this.currentFilter.set('all');
    
    if (hadFilters) {
      this.appInsights.trackEvent('TodoList.FiltersCleared', {
        totalTodos: this.todos().length.toString()
      });
    }
  }

  viewTodoDetail(todoId: string) {
    // 詳細表示の追跡
    this.appInsights.trackEvent('TodoList.TodoDetailViewed', {
      todoId
    });
    
    this.router.navigate(['/todos', todoId]);
  }
}
