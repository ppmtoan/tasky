import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { TaskDto, CreateTaskDto, UpdateTaskDto, TaskStatus, TaskPriority } from './models';
import { PagedResultDto } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class MockTaskService {
  private mockTasks: TaskDto[] = [
    {
      id: '1',
      title: 'Implement user authentication',
      description: 'Add JWT-based authentication to the API',
      status: TaskStatus.InProgress,
      priority: TaskPriority.High,
      assigneeId: 'user-1',
      assigneeName: 'John Doe',
      creatorId: 'user-2',
      creatorName: 'Jane Smith',
      dueDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000).toISOString(),
      estimatedHours: 8,
      actualHours: 4,
      tags: ['backend', 'security'],
      createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000).toISOString(),
      updatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '2',
      title: 'Design landing page',
      description: 'Create mockups for the new landing page',
      status: TaskStatus.Review,
      priority: TaskPriority.Medium,
      assigneeId: 'user-3',
      assigneeName: 'Bob Wilson',
      creatorId: 'user-2',
      creatorName: 'Jane Smith',
      dueDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000).toISOString(),
      estimatedHours: 6,
      actualHours: 6,
      tags: ['design', 'frontend'],
      createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
      updatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '3',
      title: 'Fix payment gateway bug',
      description: 'Users reporting failed transactions',
      status: TaskStatus.Todo,
      priority: TaskPriority.Critical,
      assigneeId: 'user-1',
      assigneeName: 'John Doe',
      creatorId: 'user-4',
      creatorName: 'Admin User',
      dueDate: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000).toISOString(),
      estimatedHours: 4,
      tags: ['bug', 'payment', 'urgent'],
      createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000).toISOString(),
      updatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '4',
      title: 'Write API documentation',
      description: 'Document all REST API endpoints',
      status: TaskStatus.Done,
      priority: TaskPriority.Low,
      assigneeId: 'user-2',
      assigneeName: 'Jane Smith',
      creatorId: 'user-2',
      creatorName: 'Jane Smith',
      dueDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString(),
      estimatedHours: 12,
      actualHours: 10,
      tags: ['documentation'],
      createdAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString(),
      updatedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString()
    },
    {
      id: '5',
      title: 'Optimize database queries',
      description: 'Improve performance of slow queries',
      status: TaskStatus.InProgress,
      priority: TaskPriority.Medium,
      assigneeId: 'user-1',
      assigneeName: 'John Doe',
      creatorId: 'user-4',
      creatorName: 'Admin User',
      dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
      estimatedHours: 16,
      actualHours: 8,
      tags: ['database', 'performance'],
      createdAt: new Date(Date.now() - 4 * 24 * 60 * 60 * 1000).toISOString(),
      updatedAt: new Date().toISOString()
    }
  ];

  create(input: CreateTaskDto): Observable<TaskDto> {
    const newTask: TaskDto = {
      id: Date.now().toString(),
      ...input,
      assigneeName: 'Unassigned',
      creatorId: 'current-user',
      creatorName: 'Current User',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };
    this.mockTasks.push(newTask);
    return of(newTask).pipe(delay(500));
  }

  delete(id: string): Observable<void> {
    this.mockTasks = this.mockTasks.filter(t => t.id !== id);
    return of(void 0).pipe(delay(500));
  }

  get(id: string): Observable<TaskDto> {
    const task = this.mockTasks.find(t => t.id === id);
    return of(task!).pipe(delay(300));
  }

  getList(input: any): Observable<PagedResultDto<TaskDto>> {
    return of({
      items: this.mockTasks,
      totalCount: this.mockTasks.length
    }).pipe(delay(300));
  }

  update(id: string, input: UpdateTaskDto): Observable<TaskDto> {
    const task = this.mockTasks.find(t => t.id === id);
    if (task) {
      Object.assign(task, input);
      task.updatedAt = new Date().toISOString();
    }
    return of(task!).pipe(delay(500));
  }

  getMyTasks(): Observable<TaskDto[]> {
    return of(this.mockTasks.filter(t => t.assigneeId === 'user-1')).pipe(delay(300));
  }

  getCreatedByMe(): Observable<TaskDto[]> {
    return of(this.mockTasks.filter(t => t.creatorId === 'user-2')).pipe(delay(300));
  }

  updateStatus(id: string, status: string): Observable<TaskDto> {
    const task = this.mockTasks.find(t => t.id === id);
    if (task) {
      task.status = status as TaskStatus;
      task.updatedAt = new Date().toISOString();
    }
    return of(task!).pipe(delay(500));
  }

  assignTask(id: string, assigneeId: string): Observable<TaskDto> {
    const task = this.mockTasks.find(t => t.id === id);
    if (task) {
      task.assigneeId = assigneeId;
      task.assigneeName = 'Assigned User';
      task.updatedAt = new Date().toISOString();
    }
    return of(task!).pipe(delay(500));
  }
}
