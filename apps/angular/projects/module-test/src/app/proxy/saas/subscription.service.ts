import { Injectable } from '@angular/core';
import { RestService, PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { SubscriptionDto } from './models';

@Injectable({
  providedIn: 'root',
})
export class SubscriptionService {
  apiName = 'SaasService';
  baseUrl = '/api/saas/subscriptions';

  constructor(private rest: RestService) {}

  get(id: string): Observable<SubscriptionDto> {
    return this.rest.request<void, SubscriptionDto>(
      {
        method: 'GET',
        url: `${this.baseUrl}/${id}`,
      },
      { apiName: this.apiName }
    );
  }

  getList(input: PagedAndSortedResultRequestDto = { maxResultCount: 10, skipCount: 0 } as PagedAndSortedResultRequestDto): Observable<PagedResultDto<SubscriptionDto>> {
    return this.rest.request<void, PagedResultDto<SubscriptionDto>>(
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

  getCurrentTenantSubscription(): Observable<SubscriptionDto> {
    return this.rest.request<void, SubscriptionDto>(
      {
        method: 'GET',
        url: `${this.baseUrl}/current`,
      },
      { apiName: this.apiName }
    );
  }

  cancel(id: string): Observable<void> {
    return this.rest.request<void, void>(
      {
        method: 'POST',
        url: `${this.baseUrl}/${id}/cancel`,
      },
      { apiName: this.apiName }
    );
  }

  renew(id: string): Observable<void> {
    return this.rest.request<void, void>(
      {
        method: 'POST',
        url: `${this.baseUrl}/${id}/renew`,
      },
      { apiName: this.apiName }
    );
  }
}
