export interface Todo {
  id: string;
  title: string;
  isCompleted: boolean;
  position: number;
  priority: string;
}

export interface CreateTodoDto {
  title: string;
  priority: string;
  category?: string;
  dueDate?: Date;
  tags?: string[];
}

export interface TodoStats {
  total: number;
  completed: number;
  pending: number;
  completionRate: number;
  byPriority: { [key: string]: number };
  byCategory: { [key: string]: number };
  dueToday: number;
  overdue: number;
}

export interface ReorderTodosDto {
  orderedIds: string[];
}
