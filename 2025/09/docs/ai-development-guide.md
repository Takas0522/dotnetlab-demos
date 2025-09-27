# AI開発支援ガイド

## 概要

このガイドは、AI Agent（GitHub Copilot、Claude、ChatGPTなど）がこのToDoアプリケーションプロジェクトで効果的な開発支援を提供するためのカスタムインストラクションとプロジェクト固有のガイドラインです。

## プロジェクト基本情報

### アプリケーション概要
- **アプリケーション種別**: Azure Entra ID認証付きToDo管理Webアプリケーション
- **アーキテクチャ**: 三層アーキテクチャ（Angular SPA + ASP.NET Core Web API + SQL Server）
- **主要機能**: ToDoアイテム管理、タグ分類、ユーザー間共有、Azure Entra ID認証

### 技術スタック
```
Frontend:  Angular 20.2 + TypeScript + Tailwind CSS + Angular Material + MSAL
Backend:   ASP.NET Core 9.0 + C# + Entity Framework Core + Microsoft.Identity.Web
Database:  SQL Server + Database Project
Monitoring: Application Insights
```

### プロジェクト構造
```
src/
├── api/           # ASP.NET Core Web API
├── database/      # SQL Database Project  
└── front/         # Angular SPA
docs/              # プロジェクトドキュメント
```

## AI Agent向けコーディング規約

### 全体的な開発方針

#### 1. モダンフレームワーク機能の優先
- **Angular**: Signals、Standalone Components、新制御フロー (@if, @for, @switch) を使用
- **ASP.NET Core**: 最新のDI、ミニマルAPI、新しいMiddleware パターンを採用
- **Entity Framework**: Code First、LINQ クエリ、非同期処理を活用

#### 2. 型安全性の重視
- **TypeScript**: strict モード、明示的型定義、Generic の活用
- **C#**: nullable reference types、record types、pattern matching の活用

#### 3. セキュリティファースト
- 認証認可の適切な実装
- SQL インジェクション対策
- XSS/CSRF 対策
- 機密情報の環境変数管理

### バックエンド（ASP.NET Core）コーディング規約

#### ファイル命名規則
```csharp
// Controllers: [Entity]Controller.cs
TodoItemsController.cs
UsersController.cs

// Services: I[Service]Service.cs, [Service]Service.cs  
ITodoItemService.cs
TodoItemService.cs

// Models: [Entity].cs
TodoItem.cs
User.cs

// DTOs: [Entity]Dto.cs, [Purpose]Dto.cs
TodoItemDto.cs
CreateTodoItemDto.cs
```

#### コーディングパターン

##### 1. Controller パターン
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoItemService _todoItemService;
    private readonly ICurrentUserService _currentUserService;

    public TodoItemsController(
        ITodoItemService todoItemService, 
        ICurrentUserService currentUserService)
    {
        _todoItemService = todoItemService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// XMLドキュメント形式でAPI仕様を記述
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TodoItemDto>>> GetTodoItems(
        [FromQuery] TodoItemFilterDto filter)
    {
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var items = await _todoItemService.GetUserTodoItemsAsync(userId.Value, filter);
        return Ok(items);
    }
}
```

##### 2. Service パターン
```csharp
public interface ITodoItemService
{
    Task<List<TodoItemDto>> GetUserTodoItemsAsync(Guid userId, TodoItemFilterDto filter);
    Task<TodoItemDto?> CreateTodoItemAsync(Guid userId, CreateTodoItemDto dto);
    Task<bool> DeleteTodoItemAsync(Guid userId, Guid todoItemId);
}

public class TodoItemService : ITodoItemService
{
    private readonly TodoDbContext _context;
    private readonly ILogger<TodoItemService> _logger;

    public TodoItemService(TodoDbContext context, ILogger<TodoItemService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TodoItemDto>> GetUserTodoItemsAsync(
        Guid userId, 
        TodoItemFilterDto filter)
    {
        try
        {
            var query = _context.TodoItems
                .Where(t => t.UserId == userId)
                .Include(t => t.Tags);

            // フィルター適用
            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query = query.Where(t => t.Title.Contains(filter.SearchText));
            }

            return await query
                .Select(t => new TodoItemDto
                {
                    Id = t.TodoItemId,
                    Title = t.Title,
                    // ... マッピング
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get todo items for user {UserId}", userId);
            throw;
        }
    }
}
```

##### 3. Entity & DTO パターン
```csharp
// Entity (Models/)
public class TodoItem
{
    public Guid TodoItemId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<TodoItemTag> Tags { get; set; } = new List<TodoItemTag>();
}

// DTO (DTOs/)
public record TodoItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? DueDate { get; init; }
    public Priority Priority { get; init; }
    public bool IsCompleted { get; init; }
    public List<TagDto> Tags { get; init; } = new();
}

public record CreateTodoItemDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; init; }
    
    public DateTime? DueDate { get; init; }
    public Priority Priority { get; init; } = Priority.Medium;
    public List<Guid> TagIds { get; init; } = new();
}
```

### フロントエンド（Angular）コーディング規約

#### ファイル命名規則
```
// Components: [feature]-[type].component.ts
todo-list.component.ts
todo-detail.component.ts

// Services: [feature].service.ts
todo.service.ts
auth.service.ts

// Pages: [page]/[page].component.ts
pages/dashboard/dashboard.component.ts
pages/todo-list/todo-list.component.ts

// Shared: shared/[type]/[name].[type].ts
shared/components/loading-spinner.component.ts
shared/services/http-error-handler.service.ts
```

#### Angular 20 モダンパターン

##### 1. Standalone Component パターン
```typescript
import { ChangeDetectionStrategy, Component, signal, computed, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-todo-item',
  templateUrl: './todo-item.component.html',
  styleUrls: ['./todo-item.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule] // standalone: true は不要
})
export class TodoItemComponent {
  // Signals for reactive state
  protected readonly item = input.required<TodoItemDto>();
  protected readonly isEditing = signal(false);
  
  // Computed values
  protected readonly daysUntilDue = computed(() => {
    const dueDate = this.item().dueDate;
    if (!dueDate) return null;
    
    const today = new Date();
    const due = new Date(dueDate);
    const diffTime = due.getTime() - today.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  });

  // Output events
  protected readonly itemUpdated = output<TodoItemDto>();
  protected readonly itemDeleted = output<string>();

  protected toggleEdit(): void {
    this.isEditing.update(editing => !editing);
  }

  protected onSave(updatedItem: TodoItemDto): void {
    this.isEditing.set(false);
    this.itemUpdated.emit(updatedItem);
  }
}
```

##### 2. Service パターン（Signal-based）
```typescript
import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/todoitems`;

  // Signals for state management
  private readonly _todos = signal<TodoItemDto[]>([]);
  private readonly _loading = signal(false);
  private readonly _error = signal<string | null>(null);

  // Public readonly signals
  public readonly todos = this._todos.asReadonly();
  public readonly loading = this._loading.asReadonly();
  public readonly error = this._error.asReadonly();

  // Computed values
  public readonly completedTodos = computed(() => 
    this._todos().filter(todo => todo.isCompleted)
  );

  public readonly pendingTodos = computed(() => 
    this._todos().filter(todo => !todo.isCompleted)
  );

  async loadTodos(filter?: TodoFilterDto): Promise<void> {
    try {
      this._loading.set(true);
      this._error.set(null);

      const params = filter ? { ...filter } : {};
      const todos = await this.http.get<TodoItemDto[]>(this.apiUrl, { params }).toPromise();
      
      this._todos.set(todos || []);
    } catch (error) {
      this._error.set('Failed to load todos');
      console.error('Error loading todos:', error);
    } finally {
      this._loading.set(false);
    }
  }

  async createTodo(dto: CreateTodoItemDto): Promise<TodoItemDto | null> {
    try {
      this._loading.set(true);
      
      const newTodo = await this.http.post<TodoItemDto>(this.apiUrl, dto).toPromise();
      if (newTodo) {
        this._todos.update(todos => [...todos, newTodo]);
      }
      
      return newTodo || null;
    } catch (error) {
      this._error.set('Failed to create todo');
      throw error;
    } finally {
      this._loading.set(false);
    }
  }
}
```

##### 3. Template パターン（新制御フロー）
```html
<!-- todo-list.component.html -->
<div class="todo-list-container">
  <h2>{{ title() }}</h2>

  <!-- Loading state -->
  @if (todoService.loading()) {
    <div class="loading-spinner">
      <app-loading-spinner />
    </div>
  }

  <!-- Error state -->
  @if (todoService.error(); as error) {
    <div class="alert alert-error">
      {{ error }}
      <button (click)="retry()" class="btn btn-sm">Retry</button>
    </div>
  }

  <!-- Todo list -->
  @if (todoService.todos().length > 0) {
    <div class="todo-items">
      @for (todo of filteredTodos(); track todo.id) {
        <app-todo-item 
          [item]="todo"
          (itemUpdated)="onTodoUpdated($event)"
          (itemDeleted)="onTodoDeleted($event)"
        />
      } @empty {
        <div class="empty-state">
          No todos match your filters.
        </div>
      }
    </div>
  } @else {
    <div class="empty-state">
      <p>No todos yet. Create your first todo!</p>
      <button (click)="createTodo()" class="btn btn-primary">
        Create Todo
      </button>
    </div>
  }

  <!-- Priority indicator -->
  @switch (selectedPriority()) {
    @case ('high') {
      <div class="priority-badge priority-high">High Priority</div>
    }
    @case ('medium') {
      <div class="priority-badge priority-medium">Medium Priority</div>
    }
    @case ('low') {
      <div class="priority-badge priority-low">Low Priority</div>
    }
    @default {
      <div class="priority-badge">All Priorities</div>
    }
  }
</div>
```

##### 4. 型定義パターン
```typescript
// models/todo.models.ts
export interface TodoItemDto {
  readonly id: string;
  readonly title: string;
  readonly description?: string;
  readonly dueDate?: Date;
  readonly priority: Priority;
  readonly isCompleted: boolean;
  readonly tags: TagDto[];
  readonly createdAt: Date;
  readonly updatedAt: Date;
}

export interface CreateTodoItemDto {
  readonly title: string;
  readonly description?: string;
  readonly dueDate?: Date;
  readonly priority: Priority;
  readonly tagIds: string[];
}

export enum Priority {
  Low = 'low',
  Medium = 'medium',
  High = 'high'
}

export interface TodoFilterDto {
  readonly searchText?: string;
  readonly priority?: Priority;
  readonly isCompleted?: boolean;
  readonly tagIds?: string[];
  readonly dueDateFrom?: Date;
  readonly dueDateTo?: Date;
}
```

### データベース設計パターン

#### テーブル設計原則
```sql
-- 1. 主キーは UNIQUEIDENTIFIER (GUID) を使用
-- 2. 作成日時・更新日時を必須で含める
-- 3. Azure Entra ID との連携を考慮
-- 4. インデックスを適切に設定

CREATE TABLE [dbo].[TodoItems]
(
    [TodoItemId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [DueDate] DATETIME2(7) NULL,
    [Priority] NVARCHAR(20) NOT NULL DEFAULT 'Medium',
    [IsCompleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_TodoItems_Users FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
);

-- インデックス設計
CREATE INDEX IX_TodoItems_UserId ON [dbo].[TodoItems]([UserId]);
CREATE INDEX IX_TodoItems_DueDate ON [dbo].[TodoItems]([DueDate]);
CREATE INDEX IX_TodoItems_Priority ON [dbo].[TodoItems]([Priority]);
CREATE INDEX IX_TodoItems_IsCompleted ON [dbo].[TodoItems]([IsCompleted]);
```

## AI Agent 支援のための重要な考慮事項

### 1. 認証コンテキストの理解
```typescript
// フロントエンド: 常にMSALトークンが必要
const accessToken = await this.msalService.acquireTokenSilent({
  scopes: ['api://your-api-client-id/access_as_user']
}).toPromise();
```

```csharp
// バックエンド: 現在のユーザー取得パターン
var userId = User.FindFirstValue("oid"); // Azure Entra ID ObjectId
```

### 2. エラーハンドリングパターン
```csharp
// API: 統一されたエラーレスポンス
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}
```

```typescript
// Frontend: エラー状態管理
private readonly _error = signal<ApiError | null>(null);

interface ApiError {
  message: string;
  details?: string;
  validationErrors?: string[];
}
```

### 3. パフォーマンス考慮
- Entity Framework: `Include()` で必要なナビゲーションプロパティのみ取得
- Angular: `OnPush` 変更検知戦略を必須使用
- SQL: 適切なインデックス設計とクエリ最適化

### 4. セキュリティ考慮
- すべてのAPIエンドポイントで `[Authorize]` 属性を使用
- ユーザー固有データのアクセス制御を実装
- SQL インジェクション対策のためパラメータ化クエリを使用

### 5. ログ・監視
```csharp
// 構造化ログ使用
_logger.LogInformation("User {UserId} created todo item {TodoItemId}", 
    userId, todoItem.TodoItemId);
```

```typescript
// Application Insights統合
this.applicationInsights.trackEvent('TodoCreated', {
  userId: currentUser.id,
  priority: dto.priority
});
```

## 開発時のチェックリスト

### 新機能追加時
- [ ] API: DTOクラス作成
- [ ] API: Entity Framework モデル更新
- [ ] API: サービス層実装
- [ ] API: コントローラー実装
- [ ] DB: マイグレーション作成
- [ ] Frontend: TypeScript型定義
- [ ] Frontend: Signal-based サービス実装
- [ ] Frontend: Standalone コンポーネント実装
- [ ] Frontend: 新制御フロー使用
- [ ] Test: APIテスト（api.http）追加
- [ ] Doc: API仕様更新

### コードレビュー観点
- [ ] 型安全性の確保
- [ ] 認証認可の適切な実装
- [ ] エラーハンドリングの網羅性
- [ ] パフォーマンス影響の確認
- [ ] セキュリティ要件の満足
- [ ] ログ出力の適切性
- [ ] テスタビリティの確保

## AI Agent への指示例

### 効果的な指示方法
```
✅ 良い例:
「TodoItemsController に優先度でフィルタリング機能を追加してください。
Priority enum を使用し、Service 層経由でデータアクセスし、
DTOパターンに従ってレスポンスを返すようにしてください。」

❌ 悪い例:
「ToDoにフィルター機能を追加して」
```

### プロジェクト固有の専門用語
- **Todo**: ToDoアイテム（タスク）
- **Tag**: 分類タグ
- **Share**: ユーザー間共有機能
- **Priority**: 優先度（High/Medium/Low）
- **Entra ID**: Azure Active Directory の新名称
- **MSAL**: Microsoft Authentication Library
- **DbContext**: Entity Framework のデータベースコンテキスト

このガイドラインに従って、一貫性のある高品質なコードの生成をお願いします。