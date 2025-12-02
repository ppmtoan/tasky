import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { InvoiceDto, MarkInvoiceAsPaidDto } from './models';
import { PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class InvoiceService {
  apiName = 'SaasService';
  baseUrl = '/api/saas/invoices';

  constructor(private rest: RestService) {}

  get(id: string): Observable<InvoiceDto> {
    return this.rest.request<void, InvoiceDto>(
      {
        method: 'GET',
        url: `${this.baseUrl}/${id}`,
      },
      { apiName: this.apiName }
    );
  }

  getList(input: PagedAndSortedResultRequestDto = { maxResultCount: 10, skipCount: 0 } as PagedAndSortedResultRequestDto): Observable<PagedResultDto<InvoiceDto>> {
    return this.rest.request<void, PagedResultDto<InvoiceDto>>(
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

  getCurrentTenantInvoices(): Observable<InvoiceDto[]> {
    return this.rest.request<void, InvoiceDto[]>(
      {
        method: 'GET',
        url: `${this.baseUrl}/current-tenant`,
      },
      { apiName: this.apiName }
    );
  }

  markAsPaid(id: string, input: MarkInvoiceAsPaidDto): Observable<InvoiceDto> {
    return this.rest.request<MarkInvoiceAsPaidDto, InvoiceDto>(
      {
        method: 'POST',
        url: `${this.baseUrl}/${id}/mark-as-paid`,
        body: input,
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

  downloadPdf(id: string): Observable<Blob> {
    return this.rest.request<void, Blob>(
      {
        method: 'GET',
        url: `${this.baseUrl}/${id}/download-pdf`,
        responseType: 'blob' as 'json',
      },
      { apiName: this.apiName }
    );
  }
}
