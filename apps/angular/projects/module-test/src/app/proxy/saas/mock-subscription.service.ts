import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { SubscriptionDto, SubscriptionStatus, BillingPeriod } from './models';
import { PagedResultDto } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class MockSubscriptionService {
  private mockSubscriptions: SubscriptionDto[] = [
    {
      id: '1',
      tenantId: 'tenant-1',
      tenantName: 'Acme Corporation',
      editionId: '2',
      editionName: 'Professional Plan',
      status: SubscriptionStatus.Active,
      billingPeriod: BillingPeriod.Monthly,
      startDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
      endDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
      price: { amount: 79, currency: 'USD' },
      autoRenew: true
    },
    {
      id: '2',
      tenantId: 'tenant-2',
      tenantName: 'Tech Startup Inc',
      editionId: '1',
      editionName: 'Basic Plan',
      status: SubscriptionStatus.Trial,
      billingPeriod: BillingPeriod.Monthly,
      startDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
      endDate: new Date(Date.now() + 9 * 24 * 60 * 60 * 1000).toISOString(),
      price: { amount: 0, currency: 'USD' },
      autoRenew: true
    },
    {
      id: '3',
      tenantId: 'tenant-3',
      tenantName: 'Global Enterprises',
      editionId: '3',
      editionName: 'Enterprise Plan',
      status: SubscriptionStatus.Active,
      billingPeriod: BillingPeriod.Yearly,
      startDate: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000).toISOString(),
      endDate: new Date(Date.now() + 305 * 24 * 60 * 60 * 1000).toISOString(),
      price: { amount: 1990, currency: 'USD' },
      autoRenew: true
    },
    {
      id: '4',
      tenantId: 'tenant-4',
      tenantName: 'Small Business LLC',
      editionId: '1',
      editionName: 'Basic Plan',
      status: SubscriptionStatus.Expired,
      billingPeriod: BillingPeriod.Monthly,
      startDate: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000).toISOString(),
      endDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
      price: { amount: 29, currency: 'USD' },
      autoRenew: false
    },
    {
      id: '5',
      tenantId: 'tenant-5',
      tenantName: 'Innovation Labs',
      editionId: '2',
      editionName: 'Professional Plan',
      status: SubscriptionStatus.Suspended,
      billingPeriod: BillingPeriod.Monthly,
      startDate: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000).toISOString(),
      endDate: new Date(Date.now() + 15 * 24 * 60 * 60 * 1000).toISOString(),
      price: { amount: 79, currency: 'USD' },
      autoRenew: true
    }
  ];

  get(id: string): Observable<SubscriptionDto> {
    const subscription = this.mockSubscriptions.find(s => s.id === id);
    return of(subscription!).pipe(delay(300));
  }

  getList(input: any = {}): Observable<PagedResultDto<SubscriptionDto>> {
    return of({
      items: this.mockSubscriptions,
      totalCount: this.mockSubscriptions.length
    }).pipe(delay(300));
  }

  getCurrentTenantSubscription(): Observable<SubscriptionDto> {
    return of(this.mockSubscriptions[0]).pipe(delay(300));
  }

  cancel(id: string): Observable<void> {
    const subscription = this.mockSubscriptions.find(s => s.id === id);
    if (subscription) {
      subscription.status = SubscriptionStatus.Cancelled;
      subscription.autoRenew = false;
    }
    return of(void 0).pipe(delay(500));
  }

  renew(id: string): Observable<void> {
    const subscription = this.mockSubscriptions.find(s => s.id === id);
    if (subscription) {
      subscription.status = SubscriptionStatus.Active;
      subscription.startDate = new Date().toISOString();
      subscription.endDate = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString();
    }
    return of(void 0).pipe(delay(500));
  }
}
