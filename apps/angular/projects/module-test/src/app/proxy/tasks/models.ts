export enum TaskStatus {
  Todo = 'Todo',
  InProgress = 'InProgress',
  Review = 'Review',
  Done = 'Done',
  Cancelled = 'Cancelled'
}

export enum TaskPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical'
}

export interface TaskDto {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  assigneeId?: string;
  assigneeName?: string;
  creatorId: string;
  creatorName: string;
  dueDate?: string;
  estimatedHours?: number;
  actualHours?: number;
  tags?: string[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateTaskDto {
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  assigneeId?: string;
  dueDate?: string;
  estimatedHours?: number;
  tags?: string[];
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  status?: TaskStatus;
  priority?: TaskPriority;
  assigneeId?: string;
  dueDate?: string;
  estimatedHours?: number;
  actualHours?: number;
  tags?: string[];
}

export interface UserDto {
  id: string;
  userName: string;
  name: string;
  surname: string;
  email: string;
}

export interface TaskCommentDto {
  id: string;
  taskId: string;
  content: string;
  authorId: string;
  authorName: string;
  createdAt: string;
}

export interface CreateTaskCommentDto {
  taskId: string;
  content: string;
}
