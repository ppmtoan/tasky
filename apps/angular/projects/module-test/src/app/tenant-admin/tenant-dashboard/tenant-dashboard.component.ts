import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TenantProvisioningService } from '../../../proxy/saas/tenant-provisioning.service';
import { InvoiceService } from '../../../proxy/saas/invoice.service';
import { InvoiceDto, TenantDashboardDto } from '../../../proxy/saas/models';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-tenant-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './tenant-dashboard.component.html',
  styleUrls: ['./tenant-dashboard.component.scss']
})
export class TenantDashboardComponent implements OnInit {
  dashboard: TenantDashboardDto | null = null;
  invoices: InvoiceDto[] = [];
  loading = false;
  error = '';

  constructor(
    private tenantProvisioningService: TenantProvisioningService,
    private invoiceService: InvoiceService
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
    this.loadInvoices();
  }

  loadDashboard(): void {
    this.loading = true;
    this.tenantProvisioningService.getTenantDashboard()
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (data) => {
          this.dashboard = data;
        },
        error: (error) => {
          this.error = 'Failed to load dashboard data';
          console.error('Error loading dashboard:', error);
        }
      });
  }

  loadInvoices(): void {
    // Using getCurrentTenantInvoices which automatically filters by current tenant
    this.invoiceService.getCurrentTenantInvoices()
      .subscribe({
        next: (data) => {
          this.invoices = data.slice(0, 5); // Show last 5 invoices
        },
        error: (error) => {
          console.error('Error loading invoices:', error);
        }
      });
  }

  getStatusBadgeClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'active':
      case 'paid':
        return 'bg-success';
      case 'trial':
        return 'bg-info';
      case 'expired':
      case 'overdue':
        return 'bg-danger';
      case 'pending':
        return 'bg-warning';
      case 'cancelled':
      case 'suspended':
        return 'bg-secondary';
      default:
        return 'bg-secondary';
    }
  }

  getFeatureLimitKeys(): string[] {
    return this.dashboard?.featureLimits ? Object.keys(this.dashboard.featureLimits) : [];
  }
}
