import { Provider } from '@angular/core';
import { EditionService } from './saas/edition.service';
import { SubscriptionService } from './saas/subscription.service';
import { InvoiceService } from './saas/invoice.service';
import { TenantProvisioningService } from './saas/tenant-provisioning.service';
import { TaskService } from './tasks/task.service';
import { UserService } from './tasks/user.service';
import { MockEditionService } from './saas/mock-edition.service';
import { MockSubscriptionService } from './saas/mock-subscription.service';
import { MockInvoiceService } from './saas/mock-invoice.service';
import { MockTenantProvisioningService } from './saas/mock-tenant-provisioning.service';
import { MockTaskService } from './tasks/mock-task.service';
import { MockUserService } from './tasks/mock-user.service';

/**
 * Use this to enable mock services for testing UI without backend
 * Add to your app.config.ts providers array
 */
export const MOCK_SERVICE_PROVIDERS: Provider[] = [
  { provide: EditionService, useClass: MockEditionService },
  { provide: SubscriptionService, useClass: MockSubscriptionService },
  { provide: InvoiceService, useClass: MockInvoiceService },
  { provide: TenantProvisioningService, useClass: MockTenantProvisioningService },
  { provide: TaskService, useClass: MockTaskService },
  { provide: UserService, useClass: MockUserService }
];
