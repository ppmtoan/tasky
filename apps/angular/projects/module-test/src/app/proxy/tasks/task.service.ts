import { Injectable } from '@angular/core';
import { RestService, Rest, PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { TaskDto, CreateTaskDto, UpdateTaskDto } from './models';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  create(input: CreateTaskDto): Observable<TaskDto> {
    return this.restService.request<CreateTaskDto, TaskDto>({
      method: 'POST',
      url: '/api/app/task',
      body: input,
    },
    { apiName: this.apiName });
  }

  delete(id: string): Observable<void> {
    return this.restService.request<void, void>({
      method: 'DELETE',
      url: `/api/app/task/${id}`,
    },
    { apiName: this.apiName });
  }

  get(id: string): Observable<TaskDto> {
    return this.restService.request<void, TaskDto>({
      method: 'GET',
      url: `/api/app/task/${id}`,
    },
    { apiName: this.apiName });
  }

  getList(input: PagedAndSortedResultRequestDto): Observable<PagedResultDto<TaskDto>> {
    return this.restService.request<void, PagedResultDto<TaskDto>>({
      method: 'GET',
      url: '/api/app/task',
      params: {
        skipCount: input.skipCount,
        maxResultCount: input.maxResultCount,
        sorting: input.sorting
      },
    },
    { apiName: this.apiName });
  }

  update(id: string, input: UpdateTaskDto): Observable<TaskDto> {
    return this.restService.request<UpdateTaskDto, TaskDto>({
      method: 'PUT',
      url: `/api/app/task/${id}`,
      body: input,
    },
    { apiName: this.apiName });
  }

  getMyTasks(): Observable<TaskDto[]> {
    return this.restService.request<void, TaskDto[]>({
      method: 'GET',
      url: '/api/app/task/my-tasks',
    },
    { apiName: this.apiName });
  }

  getCreatedByMe(): Observable<TaskDto[]> {
    return this.restService.request<void, TaskDto[]>({
      method: 'GET',
      url: '/api/app/task/created-by-me',
    },
    { apiName: this.apiName });
  }

  updateStatus(id: string, status: string): Observable<TaskDto> {
    return this.restService.request<void, TaskDto>({
      method: 'PUT',
      url: `/api/app/task/${id}/status`,
      params: { status },
    },
    { apiName: this.apiName });
  }

  assignTask(id: string, assigneeId: string): Observable<TaskDto> {
    return this.restService.request<void, TaskDto>({
      method: 'PUT',
      url: `/api/app/task/${id}/assign`,
      params: { assigneeId },
    },
    { apiName: this.apiName });
  }
}
