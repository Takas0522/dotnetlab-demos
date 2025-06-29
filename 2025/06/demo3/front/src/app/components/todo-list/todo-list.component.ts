import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Todo, CreateTodo, TodoStatus, TodoPriority } from '../../models/todo.model';
import { Category } from '../../models/category.model';
import { TodoService } from '../../services/todo.service';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.scss']
})
export class TodoListComponent implements OnInit {
  todos: Todo[] = [];
  categories: Category[] = [];
  filteredTodos: Todo[] = [];
  filterForm: FormGroup;
  todoForm: FormGroup;
  categoryForm: FormGroup;
  currentUserId = 1; // ハードコードされたユーザーID
  showTodoForm = false;
  showCategoryForm = false;
  editingTodo: Todo | null = null;
  editingCategory: Category | null = null;
  
  TodoStatus = TodoStatus;
  TodoPriority = TodoPriority;

  constructor(
    private todoService: TodoService,
    private categoryService: CategoryService,
    private fb: FormBuilder
  ) {
    this.filterForm = this.fb.group({
      categoryId: [''],
      status: [''],
      searchText: ['']
    });

    this.todoForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      categoryId: [''],
      priority: [TodoPriority.Normal],
      dueDate: ['']
    });

    this.categoryForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      color: ['#2196F3']
    });
  }

  ngOnInit(): void {
    this.loadTodos();
    this.loadCategories();
    this.setupFilters();
  }

  loadTodos(): void {
    this.todoService.getTodosByUserId(this.currentUserId).subscribe({
      next: (todos: Todo[]) => {
        this.todos = todos;
        this.applyFilters();
      },
      error: (error: any) => {
        console.error('Error loading todos:', error);
        alert('TODOの読み込みに失敗しました');
      }
    });
  }

  loadCategories(): void {
    this.categoryService.getCategoriesByUserId(this.currentUserId).subscribe({
      next: (categories: Category[]) => {
        this.categories = categories;
      },
      error: (error: any) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  setupFilters(): void {
    this.filterForm.valueChanges.subscribe(() => {
      this.applyFilters();
    });
  }

  applyFilters(): void {
    const { categoryId, status, searchText } = this.filterForm.value;
    
    this.filteredTodos = this.todos.filter(todo => {
      const matchesCategory = !categoryId || todo.categoryId === +categoryId;
      const matchesStatus = !status || todo.status === +status;
      const matchesSearch = !searchText || 
        todo.title.toLowerCase().includes(searchText.toLowerCase()) ||
        (todo.description && todo.description.toLowerCase().includes(searchText.toLowerCase()));
      
      return matchesCategory && matchesStatus && matchesSearch;
    });
  }

  openTodoForm(todo?: Todo): void {
    this.editingTodo = todo || null;
    if (todo) {
      this.todoForm.patchValue({
        title: todo.title,
        description: todo.description,
        categoryId: todo.categoryId,
        priority: todo.priority,
        dueDate: todo.dueDate ? todo.dueDate.split('T')[0] : ''
      });
    } else {
      this.todoForm.reset({
        title: '',
        description: '',
        categoryId: '',
        priority: TodoPriority.Normal,
        dueDate: ''
      });
    }
    this.showTodoForm = true;
  }

  closeTodoForm(): void {
    this.showTodoForm = false;
    this.editingTodo = null;
    this.todoForm.reset();
  }

  submitTodo(): void {
    if (this.todoForm.valid) {
      const formValue = this.todoForm.value;
      
      if (this.editingTodo) {
        // Update existing todo
        const updateData: any = {
          title: formValue.title,
          description: formValue.description,
          categoryId: formValue.categoryId || null,
          priority: formValue.priority,
          dueDate: formValue.dueDate || null
        };

        this.todoService.updateTodo(this.editingTodo.id, this.currentUserId, updateData).subscribe({
          next: () => {
            this.loadTodos();
            this.closeTodoForm();
            alert('TODOを更新しました');
          },
          error: (error: any) => {
            console.error('Error updating todo:', error);
            alert('TODOの更新に失敗しました');
          }
        });
      } else {
        // Create new todo
        const createData: CreateTodo = {
          userId: this.currentUserId,
          title: formValue.title,
          description: formValue.description,
          categoryId: formValue.categoryId || undefined,
          priority: formValue.priority,
          dueDate: formValue.dueDate || undefined
        };

        this.todoService.createTodo(createData).subscribe({
          next: () => {
            this.loadTodos();
            this.closeTodoForm();
            alert('TODOを作成しました');
          },
          error: (error: any) => {
            console.error('Error creating todo:', error);
            alert('TODOの作成に失敗しました');
          }
        });
      }
    }
  }

  openCategoryForm(category?: Category): void {
    this.editingCategory = category || null;
    if (category) {
      this.categoryForm.patchValue({
        name: category.name,
        description: category.description,
        color: category.color || '#2196F3'
      });
    } else {
      this.categoryForm.reset({
        name: '',
        description: '',
        color: '#2196F3'
      });
    }
    this.showCategoryForm = true;
  }

  closeCategoryForm(): void {
    this.showCategoryForm = false;
    this.editingCategory = null;
    this.categoryForm.reset();
  }

  submitCategory(): void {
    if (this.categoryForm.valid) {
      const formValue = this.categoryForm.value;
      
      if (this.editingCategory) {
        // Update existing category
        this.categoryService.updateCategory(this.editingCategory.id, this.currentUserId, formValue).subscribe({
          next: () => {
            this.loadCategories();
            this.closeCategoryForm();
            alert('カテゴリを更新しました');
          },
          error: (error: any) => {
            console.error('Error updating category:', error);
            alert('カテゴリの更新に失敗しました');
          }
        });
      } else {
        // Create new category
        const createData = {
          userId: this.currentUserId,
          ...formValue
        };

        this.categoryService.createCategory(createData).subscribe({
          next: () => {
            this.loadCategories();
            this.closeCategoryForm();
            alert('カテゴリを作成しました');
          },
          error: (error: any) => {
            console.error('Error creating category:', error);
            alert('カテゴリの作成に失敗しました');
          }
        });
      }
    }
  }

  toggleTodoStatus(todo: Todo): void {
    const newStatus = todo.status === TodoStatus.Completed 
      ? TodoStatus.NotStarted 
      : TodoStatus.Completed;
    
    this.todoService.updateTodoStatus(todo.id, this.currentUserId, newStatus).subscribe({
      next: () => {
        todo.status = newStatus;
        if (newStatus === TodoStatus.Completed) {
          todo.completedAt = new Date().toISOString();
        } else {
          todo.completedAt = undefined;
        }
        this.applyFilters();
        alert('TODOのステータスを更新しました');
      },
      error: (error: any) => {
        console.error('Error updating todo status:', error);
        alert('ステータスの更新に失敗しました');
      }
    });
  }

  deleteTodo(todo: Todo): void {
    if (confirm('このTODOを削除しますか？')) {
      this.todoService.deleteTodo(todo.id, this.currentUserId).subscribe({
        next: () => {
          this.loadTodos();
          alert('TODOを削除しました');
        },
        error: (error: any) => {
          console.error('Error deleting todo:', error);
          alert('TODOの削除に失敗しました');
        }
      });
    }
  }

  deleteCategory(category: Category): void {
    if (confirm('このカテゴリを削除しますか？')) {
      this.categoryService.deleteCategory(category.id, this.currentUserId).subscribe({
        next: () => {
          this.loadCategories();
          alert('カテゴリを削除しました');
        },
        error: (error: any) => {
          console.error('Error deleting category:', error);
          alert('カテゴリの削除に失敗しました');
        }
      });
    }
  }

  getStatusText(status: number): string {
    switch (status) {
      case TodoStatus.NotStarted: return '未開始';
      case TodoStatus.InProgress: return '進行中';
      case TodoStatus.OnHold: return '保留';
      case TodoStatus.Completed: return '完了';
      case TodoStatus.Cancelled: return 'キャンセル';
      default: return '不明';
    }
  }

  getPriorityText(priority: number): string {
    switch (priority) {
      case TodoPriority.Low: return '低';
      case TodoPriority.Normal: return '通常';
      case TodoPriority.High: return '高';
      case TodoPriority.Urgent: return '緊急';
      case TodoPriority.Critical: return '最重要';
      default: return '通常';
    }
  }

  getPriorityColor(priority: number): string {
    switch (priority) {
      case TodoPriority.Low: return '#4CAF50';
      case TodoPriority.Normal: return '#2196F3';
      case TodoPriority.High: return '#FF9800';
      case TodoPriority.Urgent: return '#F44336';
      case TodoPriority.Critical: return '#9C27B0';
      default: return '#2196F3';
    }
  }

  getCompletedTodosCount(): number {
    return this.filteredTodos.filter(todo => todo.status === TodoStatus.Completed).length;
  }

  getTotalTodosCount(): number {
    return this.filteredTodos.length;
  }
}
