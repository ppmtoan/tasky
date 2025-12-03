import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { 
  TenantProvisioningRequestDto, 
  TenantProvisioningResultDto,
  TenantDashboardDto 
} from './models';

@Injectable({
  providedIn: 'root',
})
export class MockTenantProvisioningService {
  provision(input: TenantProvisioningRequestDto): Observable<TenantProvisioningResultDto> {
    return of({
      success: true,
      tenantId: Date.now().toString(),
      tenantName: input.tenantName,
      adminEmail: input.adminEmail,
      message: 'Tenant created successfully'
    }).pipe(delay(1000));
  }

  getTenantDashboard(): Observable<TenantDashboardDto> {
    return of({
      tenantName: 'Acme Corporation',
      editionName: 'Professional Plan',
      featureLimits: {
        maxUsers: 25,
        storageQuotaGB: 100,
        apiCallLimit: 10000
      },
      subscriptionStatus: 'Active',
      subscriptionStartDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
      subscriptionEndDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
      nextBillingDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
      billingPeriod: 'Monthly',
      currentPrice: {
        amount: 299,
        currency: 'USD'
      },
      autoRenew: true
    }).pipe(delay(300));
  }
}
