export interface Todo {
  id: number;
  title: string;
  description?: string;
  isCompleted: boolean;
  createdAt: string;
  updatedAt: string;
  userId: string;
  tags: Tag[];
  sharedWith: SharedTodo[];
}

export interface Tag {
  id: number;
  name: string;
  color?: string;
}

export interface SharedTodo {
  id: number;
  todoId: number;
  sharedWithUserId: string;
  sharedByUserId: string;
  sharedAt: string;
  permissions: 'READ' | 'WRITE';
}

export interface CreateTodoRequest {
  title: string;
  description?: string;
  tagIds?: number[];
}

export interface UpdateTodoRequest {
  title?: string;
  description?: string;
  isCompleted?: boolean;
  tagIds?: number[];
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
