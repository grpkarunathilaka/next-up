export interface Todo {
  id: string;
  title: string;
  isCompleted: boolean;
  createdAt: Date;
  priority: 'low' | 'medium' | 'high';
  category?: string;
  dueDate?: Date;
  tags: string[];
}

export interface CreateTodoDto {
  title: string;
  priority?: 'low' | 'medium' | 'high';
  category?: string;
  dueDate?: Date;
  tags?: string[];
}

export interface TodoStats {
  total: number;
  completed: number;
  pending: number;
  completionRate: number;
}