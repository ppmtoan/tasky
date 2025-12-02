import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { EditionDto, CreateEditionDto, UpdateEditionDto, BillingPeriod } from './models';
import { PagedResultDto } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class MockEditionService {
  private mockEditions: EditionDto[] = [
    {
      id: '1',
      name: 'basic',
      displayName: 'Basic Plan',
      description: 'Perfect for small teams getting started',
      isActive: true,
      monthlyPrice: { amount: 29, currency: 'USD' },
      yearlyPrice: { amount: 290, currency: 'USD' },
      trialDays: 14,
      featureLimits: {
        maxUsers: 5,
        storageQuotaGB: 10,
        apiCallLimit: 1000
      }
    },
    {
      id: '2',
      name: 'professional',
      displayName: 'Professional Plan',
      description: 'For growing businesses with advanced needs',
      isActive: true,
      monthlyPrice: { amount: 79, currency: 'USD' },
      yearlyPrice: { amount: 790, currency: 'USD' },
      trialDays: 14,
      featureLimits: {
        maxUsers: 25,
        storageQuotaGB: 100,
        apiCallLimit: 10000
      }
    },
    {
      id: '3',
      name: 'enterprise',
      displayName: 'Enterprise Plan',
      description: 'For large organizations with custom requirements',
      isActive: true,
      monthlyPrice: { amount: 199, currency: 'USD' },
      yearlyPrice: { amount: 1990, currency: 'USD' },
      trialDays: 30,
      featureLimits: {
        maxUsers: -1,
        storageQuotaGB: 1000,
        apiCallLimit: -1
      }
    }
  ];

  get(id: string): Observable<EditionDto> {
    const edition = this.mockEditions.find(e => e.id === id);
    return of(edition!).pipe(delay(300));
  }

  getList(input: any = {}): Observable<PagedResultDto<EditionDto>> {
    return of({
      items: this.mockEditions,
      totalCount: this.mockEditions.length
    }).pipe(delay(300));
  }

  create(input: CreateEditionDto): Observable<EditionDto> {
    const newEdition: EditionDto = {
      id: Date.now().toString(),
      ...input
    };
    this.mockEditions.push(newEdition);
    return of(newEdition).pipe(delay(500));
  }

  update(id: string, input: UpdateEditionDto): Observable<EditionDto> {
    const index = this.mockEditions.findIndex(e => e.id === id);
    if (index >= 0) {
      this.mockEditions[index] = { ...this.mockEditions[index], ...input };
      return of(this.mockEditions[index]).pipe(delay(500));
    }
    return of(null as any);
  }

  delete(id: string): Observable<void> {
    this.mockEditions = this.mockEditions.filter(e => e.id !== id);
    return of(void 0).pipe(delay(500));
  }
}
