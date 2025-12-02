import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SubscriptionService, SubscriptionDto, SubscriptionStatus } from '../../../proxy/saas';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-subscriptions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './subscriptions.component.html',
  styleUrls: ['./subscriptions.component.scss']
})
export class SubscriptionsComponent implements OnInit {
  subscriptions: SubscriptionDto[] = [];
  filteredSubscriptions: SubscriptionDto[] = [];
  loading = false;
  selectedStatus: string = 'all';
  
  statusFilters = [
    { value: 'all', label: 'All' },
    { value: SubscriptionStatus.Active, label: 'Active' },
    { value: SubscriptionStatus.Trial, label: 'Trial' },
    { value: SubscriptionStatus.Expired, label: 'Expired' },
    { value: SubscriptionStatus.Cancelled, label: 'Cancelled' },
    { value: SubscriptionStatus.Suspended, label: 'Suspended' }
  ];

  constructor(private subscriptionService: SubscriptionService) {}

  ngOnInit(): void {
    this.loadSubscriptions();
  }

  loadSubscriptions(): void {
    this.loading = true;
    this.subscriptionService.getList({})
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (response) => {
          this.subscriptions = response.items || [];
          this.applyFilter();
        },
        error: (error) => {
          console.error('Error loading subscriptions:', error);
        }
      });
  }

  applyFilter(): void {
    if (this.selectedStatus === 'all') {
      this.filteredSubscriptions = this.subscriptions;
    } else {
      this.filteredSubscriptions = this.subscriptions.filter(
        s => s.status === this.selectedStatus
      );
    }
  }

  onFilterChange(status: string): void {
    this.selectedStatus = status;
    this.applyFilter();
  }

  cancelSubscription(id: string): void {
    if (confirm('Are you sure you want to cancel this subscription?')) {
      this.subscriptionService.cancel(id).subscribe({
        next: () => {
          this.loadSubscriptions();
        },
        error: (error) => {
          console.error('Error canceling subscription:', error);
        }
      });
    }
  }

  renewSubscription(id: string): void {
    if (confirm('Are you sure you want to renew this subscription?')) {
      this.subscriptionService.renew(id).subscribe({
        next: () => {
          this.loadSubscriptions();
        },
        error: (error) => {
          console.error('Error renewing subscription:', error);
        }
      });
    }
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case SubscriptionStatus.Active:
        return 'bg-success';
      case SubscriptionStatus.Trial:
        return 'bg-info';
      case SubscriptionStatus.Expired:
        return 'bg-danger';
      case SubscriptionStatus.Cancelled:
        return 'bg-secondary';
      case SubscriptionStatus.Suspended:
        return 'bg-warning';
      default:
        return 'bg-secondary';
    }
  }

  getDaysUntilEnd(endDate: string): number {
    const end = new Date(endDate);
    const now = new Date();
    const diffTime = end.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  }

  isExpiringSoon(subscription: SubscriptionDto): boolean {
    const days = this.getDaysUntilEnd(subscription.endDate);
    return days > 0 && days <= 7 && subscription.status === SubscriptionStatus.Active;
  }

  isExpired(subscription: SubscriptionDto): boolean {
    return this.getDaysUntilEnd(subscription.endDate) < 0;
  }
}
