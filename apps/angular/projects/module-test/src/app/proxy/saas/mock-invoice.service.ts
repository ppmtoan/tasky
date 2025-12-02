import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { InvoiceDto, InvoiceStatus, MarkInvoiceAsPaidDto } from './models';
import { PagedResultDto } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class MockInvoiceService {
  private mockInvoices: InvoiceDto[] = [
    {
      id: '1',
      invoiceNumber: 'INV-2025-001',
      tenantId: 'tenant-1',
      tenantName: 'Acme Corporation',
      subscriptionId: '1',
      status: InvoiceStatus.Paid,
      issueDate: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000).toISOString(),
      dueDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
      paidDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString(),
      amount: { amount: 79, currency: 'USD' },
      paymentMethod: 'Credit Card',
      paymentReference: 'ch_1234567890',
      billingPeriodStart: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
      billingPeriodEnd: new Date(Date.now()).toISOString(),
      notes: ''
    },
    {
      id: '2',
      invoiceNumber: 'INV-2025-002',
      tenantId: 'tenant-1',
      tenantName: 'Acme Corporation',
      subscriptionId: '1',
      status: InvoiceStatus.Pending,
      issueDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(),
      dueDate: new Date(Date.now() + 10 * 24 * 60 * 60 * 1000).toISOString(),
      amount: { amount: 79, currency: 'USD' },
      billingPeriodStart: new Date(Date.now()).toISOString(),
      billingPeriodEnd: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
      notes: ''
    },
    {
      id: '3',
      invoiceNumber: 'INV-2025-003',
      tenantId: 'tenant-3',
      tenantName: 'Global Enterprises',
      subscriptionId: '3',
      status: InvoiceStatus.Paid,
      issueDate: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000).toISOString(),
      dueDate: new Date(Date.now() - 50 * 24 * 60 * 60 * 1000).toISOString(),
      paidDate: new Date(Date.now() - 55 * 24 * 60 * 60 * 1000).toISOString(),
      amount: { amount: 1990, currency: 'USD' },
      paymentMethod: 'Bank Transfer',
      paymentReference: 'TXN-9876543210',
      billingPeriodStart: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000).toISOString(),
      billingPeriodEnd: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000).toISOString(),
      notes: ''
    },
    {
      id: '4',
      invoiceNumber: 'INV-2025-004',
      tenantId: 'tenant-4',
      tenantName: 'Small Business LLC',
      subscriptionId: '4',
      status: InvoiceStatus.Overdue,
      issueDate: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000).toISOString(),
      dueDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000).toISOString(),
      amount: { amount: 29, currency: 'USD' },
      billingPeriodStart: new Date(Date.now() - 75 * 24 * 60 * 60 * 1000).toISOString(),
      billingPeriodEnd: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000).toISOString(),
      notes: ''
    },
    {
      id: '5',
      invoiceNumber: 'INV-2025-005',
      tenantId: 'tenant-2',
      tenantName: 'Tech Startup Inc',
      subscriptionId: '2',
      status: InvoiceStatus.Pending,
      issueDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString(),
      dueDate: new Date(Date.now() + 13 * 24 * 60 * 60 * 1000).toISOString(),
      amount: { amount: 29, currency: 'USD' },
      billingPeriodStart: new Date(Date.now()).toISOString(),
      billingPeriodEnd: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
      notes: ''
    },
    {
      id: '6',
      invoiceNumber: 'INV-2025-006',
      tenantId: 'tenant-5',
      tenantName: 'Innovation Labs',
      subscriptionId: '5',
      status: InvoiceStatus.Cancelled,
      issueDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
      dueDate: new Date(Date.now() - 20 * 24 * 60 * 60 * 1000).toISOString(),
      amount: { amount: 79, currency: 'USD' },
      billingPeriodStart: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000).toISOString(),
      billingPeriodEnd: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(),
      notes: 'Cancelled by user'
    }
  ];

  get(id: string): Observable<InvoiceDto> {
    const invoice = this.mockInvoices.find(i => i.id === id);
    return of(invoice!).pipe(delay(300));
  }

  getList(input: any = {}): Observable<PagedResultDto<InvoiceDto>> {
    return of({
      items: this.mockInvoices,
      totalCount: this.mockInvoices.length
    }).pipe(delay(300));
  }

  getCurrentTenantInvoices(): Observable<InvoiceDto[]> {
    return of(this.mockInvoices.filter(i => i.tenantId === 'tenant-1')).pipe(delay(300));
  }

  markAsPaid(id: string, input: MarkInvoiceAsPaidDto): Observable<InvoiceDto> {
    const invoice = this.mockInvoices.find(i => i.id === id);
    if (invoice) {
      invoice.status = InvoiceStatus.Paid;
      invoice.paidDate = new Date().toISOString();
      invoice.paymentMethod = input.paymentMethod;
      invoice.paymentReference = input.paymentReference;
    }
    return of(invoice!).pipe(delay(500));
  }

  cancel(id: string): Observable<void> {
    const invoice = this.mockInvoices.find(i => i.id === id);
    if (invoice) {
      invoice.status = InvoiceStatus.Cancelled;
    }
    return of(void 0).pipe(delay(500));
  }

  downloadPdf(id: string): Observable<Blob> {
    const blob = new Blob(['Mock PDF content'], { type: 'application/pdf' });
    return of(blob).pipe(delay(500));
  }
}
