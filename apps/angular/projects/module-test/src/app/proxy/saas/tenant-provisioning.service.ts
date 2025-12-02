import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { 
  TenantProvisioningRequestDto, 
  TenantProvisioningResultDto,
  TenantDetailDto,
  TenantDashboardDto 
} from './models';

@Injectable({
  providedIn: 'root',
})
export class TenantProvisioningService {
  apiName = 'SaasService';
  baseUrl = '/api/saas/tenant-provisioning';

  constructor(private rest: RestService) {}

  provision(input: TenantProvisioningRequestDto): Observable<TenantProvisioningResultDto> {
    return this.rest.request<TenantProvisioningRequestDto, TenantProvisioningResultDto>(
      {
        method: 'POST',
        url: `${this.baseUrl}/provision`,
        body: input,
      },
      { apiName: this.apiName }
    );
  }

  getTenantDetail(tenantId: string): Observable<TenantDetailDto> {
    return this.rest.request<void, TenantDetailDto>(
      {
        method: 'GET',
        url: `/api/saas/tenant-admin/details/${tenantId}`,
      },
      { apiName: this.apiName }
    );
  }

  getTenantDashboard(): Observable<TenantDashboardDto> {
    return this.rest.request<void, TenantDashboardDto>(
      {
        method: 'GET',
        url: '/api/saas/tenant-admin/dashboard',
      },
      { apiName: this.apiName }
    );
  }
}
