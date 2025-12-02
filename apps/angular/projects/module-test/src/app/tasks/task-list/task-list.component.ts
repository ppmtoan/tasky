import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../../proxy/tasks/task.service';
import { UserService } from '../../proxy/tasks/user.service';
import { TaskDto, CreateTaskDto, UpdateTaskDto, TaskStatus, TaskPriority, UserDto } from '../../proxy/tasks/models';

interface TaskFilter {
  value: string;
  label: string;
}

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit {
  tasks = signal<TaskDto[]>([]);
  filteredTasks = signal<TaskDto[]>([]);
  users = signal<UserDto[]>([]);
  loading = signal<boolean>(false);
  selectedStatus = signal<string>('all');
  selectedPriority = signal<string>('all');
  searchTerm = signal<string>('');
  viewMode = signal<'my-tasks' | 'created-by-me' | 'all'>('my-tasks');

  // Modal state
  showTaskModal = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  selectedTask = signal<TaskDto | null>(null);
  taskForm!: FormGroup;

  statusFilters: TaskFilter[] = [
    { value: 'all', label: 'All' },
    { value: 'Todo', label: 'To Do' },
    { value: 'InProgress', label: 'In Progress' },
    { value: 'Review', label: 'Review' },
    { value: 'Done', label: 'Done' },
    { value: 'Cancelled', label: 'Cancelled' }
  ];

  priorityFilters: TaskFilter[] = [
    { value: 'all', label: 'All Priorities' },
    { value: 'Low', label: 'Low' },
    { value: 'Medium', label: 'Medium' },
    { value: 'High', label: 'High' },
    { value: 'Critical', label: 'Critical' }
  ];

  TaskStatus = TaskStatus;
  TaskPriority = TaskPriority;

  constructor(
    private taskService: TaskService,
    private userService: UserService,
    private fb: FormBuilder
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.loadTasks();
    this.loadUsers();
  }

  initializeForm(): void {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', Validators.maxLength(2000)],
      status: [TaskStatus.Todo, Validators.required],
      priority: [TaskPriority.Medium, Validators.required],
      assigneeId: [''],
      dueDate: [''],
      estimatedHours: [null],
      actualHours: [null],
      tags: [[]]
    });
  }

  loadTasks(): void {
    this.loading.set(true);
    
    let observable;
    switch (this.viewMode()) {
      case 'my-tasks':
        observable = this.taskService.getMyTasks();
        break;
      case 'created-by-me':
        observable = this.taskService.getCreatedByMe();
        break;
      default:
        observable = this.taskService.getList({ maxResultCount: 10, skipCount: 0 });
    }

    observable.subscribe({
      next: (response: any) => {
        const tasks = Array.isArray(response) ? response : response.items || [];
        this.tasks.set(tasks);
        this.applyFilters();
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load tasks:', error);
        this.loading.set(false);
      }
    });
  }

  loadUsers(): void {
    this.userService.getTenantUsers().subscribe({
      next: (users) => {
        this.users.set(users);
      },
      error: (error) => {
        console.error('Failed to load users:', error);
      }
    });
  }

  onViewModeChange(mode: 'my-tasks' | 'created-by-me' | 'all'): void {
    this.viewMode.set(mode);
    this.loadTasks();
  }

  onStatusFilterChange(status: string): void {
    this.selectedStatus.set(status);
    this.applyFilters();
  }

  onPriorityFilterChange(priority: string): void {
    this.selectedPriority.set(priority);
    this.applyFilters();
  }

  onSearchChange(term: string): void {
    this.searchTerm.set(term.toLowerCase());
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = this.tasks();

    // Filter by status
    if (this.selectedStatus() !== 'all') {
      filtered = filtered.filter(task => task.status === this.selectedStatus());
    }

    // Filter by priority
    if (this.selectedPriority() !== 'all') {
      filtered = filtered.filter(task => task.priority === this.selectedPriority());
    }

    // Filter by search term
    const search = this.searchTerm();
    if (search) {
      filtered = filtered.filter(task =>
        task.title.toLowerCase().includes(search) ||
        task.description?.toLowerCase().includes(search) ||
        task.assigneeName?.toLowerCase().includes(search)
      );
    }

    this.filteredTasks.set(filtered);
  }

  openCreateModal(): void {
    this.isEditMode.set(false);
    this.selectedTask.set(null);
    this.taskForm.reset({
      status: TaskStatus.Todo,
      priority: TaskPriority.Medium,
      tags: []
    });
    this.showTaskModal.set(true);
  }

  openEditModal(task: TaskDto): void {
    this.isEditMode.set(true);
    this.selectedTask.set(task);
    this.taskForm.patchValue({
      title: task.title,
      description: task.description,
      status: task.status,
      priority: task.priority,
      assigneeId: task.assigneeId || '',
      dueDate: task.dueDate ? new Date(task.dueDate).toISOString().split('T')[0] : '',
      estimatedHours: task.estimatedHours,
      actualHours: task.actualHours,
      tags: task.tags || []
    });
    this.showTaskModal.set(true);
  }

  closeTaskModal(): void {
    this.showTaskModal.set(false);
    this.selectedTask.set(null);
    this.taskForm.reset();
  }

  saveTask(): void {
    if (this.taskForm.invalid) {
      Object.keys(this.taskForm.controls).forEach(key => {
        this.taskForm.get(key)?.markAsTouched();
      });
      return;
    }

    const formValue = this.taskForm.value;
    const taskData = {
      ...formValue,
      dueDate: formValue.dueDate ? new Date(formValue.dueDate).toISOString() : undefined,
      assigneeId: formValue.assigneeId || undefined
    };

    if (this.isEditMode()) {
      const task = this.selectedTask();
      if (!task) return;

      this.taskService.update(task.id, taskData as UpdateTaskDto).subscribe({
        next: () => {
          alert('Task updated successfully');
          this.closeTaskModal();
          this.loadTasks();
        },
        error: (error) => {
          console.error('Failed to update task:', error);
          alert('Failed to update task. Please try again.');
        }
      });
    } else {
      this.taskService.create(taskData as CreateTaskDto).subscribe({
        next: () => {
          alert('Task created successfully');
          this.closeTaskModal();
          this.loadTasks();
        },
        error: (error) => {
          console.error('Failed to create task:', error);
          alert('Failed to create task. Please try again.');
        }
      });
    }
  }

  deleteTask(task: TaskDto): void {
    if (confirm(`Are you sure you want to delete task "${task.title}"?`)) {
      this.taskService.delete(task.id).subscribe({
        next: () => {
          alert('Task deleted successfully');
          this.loadTasks();
        },
        error: (error) => {
          console.error('Failed to delete task:', error);
          alert('Failed to delete task. Please try again.');
        }
      });
    }
  }

  changeStatus(task: TaskDto, newStatus: TaskStatus): void {
    this.taskService.updateStatus(task.id, newStatus).subscribe({
      next: () => {
        this.loadTasks();
      },
      error: (error) => {
        console.error('Failed to update status:', error);
        alert('Failed to update status. Please try again.');
      }
    });
  }

  reassignTask(task: TaskDto, userId: string): void {
    this.taskService.assignTask(task.id, userId).subscribe({
      next: () => {
        alert('Task reassigned successfully');
        this.loadTasks();
      },
      error: (error) => {
        console.error('Failed to reassign task:', error);
        alert('Failed to reassign task. Please try again.');
      }
    });
  }

  getStatusBadgeClass(status: TaskStatus): string {
    const statusMap: Record<TaskStatus, string> = {
      'Todo': 'bg-secondary',
      'InProgress': 'bg-primary',
      'Review': 'bg-warning',
      'Done': 'bg-success',
      'Cancelled': 'bg-danger'
    };
    return statusMap[status] || 'bg-secondary';
  }

  getPriorityBadgeClass(priority: TaskPriority): string {
    const priorityMap: Record<TaskPriority, string> = {
      'Low': 'bg-info',
      'Medium': 'bg-primary',
      'High': 'bg-warning',
      'Critical': 'bg-danger'
    };
    return priorityMap[priority] || 'bg-secondary';
  }

  isOverdue(task: TaskDto): boolean {
    if (!task.dueDate || task.status === 'Done' || task.status === 'Cancelled') {
      return false;
    }
    const dueDate = new Date(task.dueDate);
    const today = new Date();
    return dueDate < today;
  }

  getDaysUntilDue(dueDate: string): number {
    const due = new Date(dueDate);
    const today = new Date();
    const diffTime = due.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  }

  getTaskStats() {
    const tasks = this.tasks();
    return {
      total: tasks.length,
      todo: tasks.filter(t => t.status === 'Todo').length,
      inProgress: tasks.filter(t => t.status === 'InProgress').length,
      done: tasks.filter(t => t.status === 'Done').length,
      overdue: tasks.filter(t => this.isOverdue(t)).length
    };
  }
}
