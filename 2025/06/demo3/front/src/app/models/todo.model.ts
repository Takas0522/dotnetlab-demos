export interface Todo {
  id: number;
  userId: number;
  categoryId?: number;
  title: string;
  description?: string;
  priority: number;
  status: number;
  dueDate?: string;
  completedAt?: string;
  createdAt: string;
  updatedAt: string;
  categoryName?: string;
  categoryColor?: string;
}

export interface CreateTodo {
  userId: number;
  categoryId?: number;
  title: string;
  description?: string;
  priority: number;
  dueDate?: string;
}

export interface UpdateTodo {
  categoryId?: number;
  title?: string;
  description?: string;
  priority?: number;
  status?: number;
  dueDate?: string;
}

export enum TodoStatus {
  NotStarted = 1,
  InProgress = 2,
  OnHold = 3,
  Completed = 4,
  Cancelled = 5
}

export enum TodoPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Urgent = 4,
  Critical = 5
}
