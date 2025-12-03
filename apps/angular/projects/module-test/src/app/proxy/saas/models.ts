export interface EditionDto {
  id: string;
  name: string;
  displayName: string;
  description: string;
  isActive: boolean;
  monthlyPrice: MoneyDto;
  yearlyPrice: MoneyDto;
  trialDays: number;
  featureLimits: Record<string, number>;
}

export interface CreateEditionDto {
  name: string;
  displayName: string;
  description: string;
  isActive: boolean;
  monthlyPrice: MoneyDto;
  yearlyPrice: MoneyDto;
  trialDays: number;
  featureLimits: Record<string, number>;
}

export interface UpdateEditionDto {
  name: string;
  displayName: string;
  description: string;
  isActive: boolean;
  monthlyPrice: MoneyDto;
  yearlyPrice: MoneyDto;
  trialDays: number;
  featureLimits: Record<string, number>;
}

export interface MoneyDto {
  amount: number;
  currency: string;
}

export interface SubscriptionDto {
  id: string;
  tenantId: string;
  tenantName: string;
  editionId: string;
  editionName: string;
  billingPeriod: BillingPeriod;
  startDate: string;
  endDate: string;
  price: MoneyDto;
  status: SubscriptionStatus;
  autoRenew: boolean;
}

export interface InvoiceDto {
  id: string;
  tenantId: string;
  tenantName: string;
  subscriptionId: string;
  invoiceNumber: string;
  issueDate: string;
  dueDate: string;
  amount: MoneyDto;
  status: InvoiceStatus;
  paidDate?: string;
  paymentMethod?: string;
  paymentReference?: string;
  billingPeriodStart: string;
  billingPeriodEnd: string;
  notes: string;
}

export interface MarkInvoiceAsPaidDto {
  paymentMethod: string;
  paymentReference: string;
}

export interface TenantProvisioningRequestDto {
  tenantName: string;
  adminEmail: string;
  adminPassword: string;
  adminUserName: string;
  editionId: string;
  billingPeriod: BillingPeriod;
}

export interface TenantProvisioningResultDto {
  success: boolean;
  tenantId?: string;
  tenantName: string;
  adminEmail: string;
  message: string;
}

export interface TenantDetailDto {
  id: string;
  name: string;
  editionId: string;
  editionName: string;
  isActive: boolean;
  subscriptionStatus: string;
  subscriptionEndDate?: string;
  connectionStrings: Record<string, string>;
}

export interface TenantDashboardDto {
  tenantName: string;
  editionName: string;
  featureLimits: Record<string, number>;
  subscriptionStatus: string;
  subscriptionStartDate: string;
  subscriptionEndDate: string;
  nextBillingDate?: string;
  billingPeriod: string;
  currentPrice: MoneyDto;
  autoRenew: boolean;
}

export enum BillingPeriod {
  Monthly = 'Monthly',
  Yearly = 'Yearly'
}

export enum SubscriptionStatus {
  Active = 'Active',
  Trial = 'Trial',
  Expired = 'Expired',
  Cancelled = 'Cancelled',
  Suspended = 'Suspended'
}

export enum InvoiceStatus {
  Pending = 'Pending',
  Paid = 'Paid',
  Overdue = 'Overdue',
  Cancelled = 'Cancelled'
}
