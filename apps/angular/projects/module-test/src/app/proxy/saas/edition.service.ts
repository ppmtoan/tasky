import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { EditionDto, CreateEditionDto, UpdateEditionDto } from './models';
import { PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class EditionService {
  apiName = 'SaasService';
  baseUrl = '/api/saas-service/editions'; // Updated to match backend route

  constructor(private rest: RestService) {}

  get(id: string): Observable<EditionDto> {
    return this.rest.request<void, EditionDto>(
      {
        method: 'GET',
        url: `${this.baseUrl}/${id}`,
      },
      { apiName: this.apiName }
    );
  }

  getList(input: PagedAndSortedResultRequestDto = { maxResultCount: 10, skipCount: 0 } as PagedAndSortedResultRequestDto): Observable<PagedResultDto<EditionDto>> {
    return this.rest.request<void, PagedResultDto<EditionDto>>(
      {
        method: 'GET',
        url: this.baseUrl,
        params: {
          skipCount: input.skipCount,
          maxResultCount: input.maxResultCount,
          sorting: input.sorting
        },
      },
      { apiName: this.apiName }
    );
  }

  create(input: CreateEditionDto): Observable<EditionDto> {
    return this.rest.request<CreateEditionDto, EditionDto>(
      {
        method: 'POST',
        url: this.baseUrl,
        body: input,
      },
      { apiName: this.apiName }
    );
  }

  update(id: string, input: UpdateEditionDto): Observable<EditionDto> {
    return this.rest.request<UpdateEditionDto, EditionDto>(
      {
        method: 'PUT',
        url: `${this.baseUrl}/${id}`,
        body: input,
      },
      { apiName: this.apiName }
    );
  }

  delete(id: string): Observable<void> {
    return this.rest.request<void, void>(
      {
        method: 'DELETE',
        url: `${this.baseUrl}/${id}`,
      },
      { apiName: this.apiName }
    );
  }
}
