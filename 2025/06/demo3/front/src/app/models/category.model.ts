export interface Category {
  id: number;
  userId: number;
  name: string;
  description?: string;
  color?: string;
  createdAt: string;
  updatedAt: string;
  todoCount: number;
}

export interface CreateCategory {
  userId: number;
  name: string;
  description?: string;
  color?: string;
}

export interface UpdateCategory {
  name?: string;
  description?: string;
  color?: string;
}
