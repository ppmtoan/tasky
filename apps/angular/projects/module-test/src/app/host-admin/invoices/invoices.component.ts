import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InvoiceService } from '../../../proxy/saas/invoice.service';
import { InvoiceDto, InvoiceStatus } from '../../../proxy/saas/models';

interface InvoiceFilter {
  value: string;
  label: string;
}

interface PaymentDetails {
  paymentMethod: string;
  transactionId: string;
  paymentDate: Date;
  notes: string;
}

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.scss']
})
export class InvoicesComponent implements OnInit {
  invoices = signal<InvoiceDto[]>([]);
  filteredInvoices = signal<InvoiceDto[]>([]);
  loading = signal<boolean>(false);
  selectedStatus = signal<string>('all');
  searchTerm = signal<string>('');

  // Modal state
  showPaymentModal = signal<boolean>(false);
  selectedInvoice = signal<InvoiceDto | null>(null);
  paymentDetails = signal<PaymentDetails>({
    paymentMethod: '',
    transactionId: '',
    paymentDate: new Date(),
    notes: ''
  });

  statusFilters: InvoiceFilter[] = [
    { value: 'all', label: 'All' },
    { value: 'Pending', label: 'Pending' },
    { value: 'Paid', label: 'Paid' },
    { value: 'Overdue', label: 'Overdue' },
    { value: 'Cancelled', label: 'Cancelled' }
  ];

  constructor(private invoiceService: InvoiceService) {}

  ngOnInit(): void {
    this.loadInvoices();
  }

  loadInvoices(): void {
    this.loading.set(true);
    this.invoiceService.getList({}).subscribe({
      next: (response) => {
        this.invoices.set(response.items || []);
        this.applyFilters();
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load invoices:', error);
        this.loading.set(false);
      }
    });
  }

  onFilterChange(status: string): void {
    this.selectedStatus.set(status);
    this.applyFilters();
  }

  onSearchChange(term: string): void {
    this.searchTerm.set(term.toLowerCase());
    this.applyFilters();
  }

  applyFilters(): void {
    let filtered = this.invoices();

    // Filter by status
    if (this.selectedStatus() !== 'all') {
      filtered = filtered.filter(invoice => invoice.status === this.selectedStatus());
    }

    // Filter by search term (tenant name or invoice number)
    const search = this.searchTerm();
    if (search) {
      filtered = filtered.filter(invoice => 
        invoice.tenantName?.toLowerCase().includes(search) ||
        invoice.invoiceNumber?.toLowerCase().includes(search)
      );
    }

    this.filteredInvoices.set(filtered);
  }

  getStatusBadgeClass(status: InvoiceStatus): string {
    const statusMap: Record<InvoiceStatus, string> = {
      'Pending': 'bg-warning',
      'Paid': 'bg-success',
      'Overdue': 'bg-danger',
      'Cancelled': 'bg-secondary'
    };
    return statusMap[status] || 'bg-secondary';
  }

  isOverdue(invoice: InvoiceDto): boolean {
    if (invoice.status === 'Paid' || invoice.status === 'Cancelled') {
      return false;
    }
    const dueDate = new Date(invoice.dueDate);
    const today = new Date();
    return dueDate < today;
  }

  getDaysOverdue(dueDate: string): number {
    const due = new Date(dueDate);
    const today = new Date();
    const diffTime = today.getTime() - due.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays > 0 ? diffDays : 0;
  }

  openPaymentModal(invoice: InvoiceDto): void {
    this.selectedInvoice.set(invoice);
    this.paymentDetails.set({
      paymentMethod: '',
      transactionId: '',
      paymentDate: new Date(),
      notes: ''
    });
    this.showPaymentModal.set(true);
  }

  closePaymentModal(): void {
    this.showPaymentModal.set(false);
    this.selectedInvoice.set(null);
  }

  markAsPaid(): void {
    const invoice = this.selectedInvoice();
    if (!invoice) return;

    const details = this.paymentDetails();
    if (!details.paymentMethod || !details.transactionId) {
      alert('Please fill in payment method and transaction ID');
      return;
    }

    if (confirm(`Mark invoice ${invoice.invoiceNumber} as paid?`)) {
      this.invoiceService.markAsPaid(invoice.id, {
        paymentMethod: details.paymentMethod,
        transactionId: details.transactionId,
        paidDate: details.paymentDate.toISOString(),
        notes: details.notes
      }).subscribe({
        next: () => {
          alert('Invoice marked as paid successfully');
          this.closePaymentModal();
          this.loadInvoices();
        },
        error: (error) => {
          console.error('Failed to mark invoice as paid:', error);
          alert('Failed to mark invoice as paid. Please try again.');
        }
      });
    }
  }

  downloadInvoice(invoiceId: string): void {
    this.invoiceService.downloadPdf(invoiceId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `invoice-${invoiceId}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Failed to download invoice:', error);
        alert('Failed to download invoice. Please try again.');
      }
    });
  }

  getTotalRevenue(): number {
    return this.invoices()
      .filter(invoice => invoice.status === 'Paid')
      .reduce((sum, invoice) => sum + invoice.totalAmount.amount, 0);
  }

  getTotalPending(): number {
    return this.invoices()
      .filter(invoice => invoice.status === 'Pending')
      .reduce((sum, invoice) => sum + invoice.totalAmount.amount, 0);
  }

  getTotalOverdue(): number {
    return this.invoices()
      .filter(invoice => invoice.status === 'Overdue' || this.isOverdue(invoice))
      .reduce((sum, invoice) => sum + invoice.totalAmount.amount, 0);
  }
}
