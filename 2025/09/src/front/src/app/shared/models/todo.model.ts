export interface Todo {
  todoItemId: string;  // WebAPIの TodoItemId に合わせる
  title: string;
  description?: string;
  isCompleted: boolean;
  priority: number;
  dueDate?: string;
  completedAt?: string;
  createdAt: string;
  updatedAt: string;
  userId: string;
  userDisplayName: string;
  tags: Tag[];
  accessType: string;
  permission: string;
}

export interface Tag {
  tagId: string;
  userId: string;
  tagName: string;  // WebAPIの TagName に合わせる
  colorCode?: string;  // WebAPIの ColorCode に合わせる
  createdAt: string;
  updatedAt: string;
  usageCount: number;
}

export interface SharedTodo {
  id: string;
  todoId: string;
  sharedWithUserId: string;
  sharedByUserId: string;
  sharedAt: string;
  permissions: 'READ' | 'WRITE';
}

export interface CreateTodoRequest {
  title: string;
  description?: string;
  priority?: number;
  dueDate?: string;
  tagIds?: string[];  // WebAPIの TagIds に合わせる
}

export interface UpdateTodoRequest {
  title?: string;
  description?: string;
  isCompleted?: boolean;
  priority?: number;
  dueDate?: string;
  tagIds?: string[];
}

export interface ShareTodoRequest {
  userEmail: string;
  permissions: 'READ' | 'WRITE';
}

export interface User {
  id: string;
  email: string;
  name?: string;
}
