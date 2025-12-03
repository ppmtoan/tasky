# SaaS Service - Domain-Driven Design Architecture

## Overview

The SaaS Service is built following **Domain-Driven Design (DDD)** principles, focusing on rich domain models, encapsulated business logic, and clear bounded contexts. This document describes the complete DDD implementation.

---

## 🏗️ DDD Layers & Structure

### 1. Domain Layer (`ModuleTest.SaasService.Domain`)

The core business logic layer containing:
- **Aggregate Roots** - Consistency boundaries with encapsulated behavior
- **Value Objects** - Immutable domain concepts
- **Domain Events** - Notification of significant domain changes
- **Domain Services** - Complex operations spanning multiple aggregates
- **Specifications** - Business rules and validation logic
- **Repository Interfaces** - Data access abstractions

**No dependencies on infrastructure** - pure business logic only.

---

## 📦 Aggregates

### 1. Edition Aggregate

**Aggregate Root:** `Edition`

**Purpose:** Represents a SaaS plan/tier with pricing and feature limits.

**Key Characteristics:**
- Controls all feature and pricing changes
- Encapsulates pricing calculations
- Maintains edition activation state
- Raises domain events for state changes

**Business Invariants:**
- Name must be unique and 3-128 characters
- Prices cannot be negative
- Feature limits must be defined
- Display order must be non-negative

**Key Methods:**
```csharp
// Pricing Operations
public Money GetPriceForPeriod(BillingPeriod period)
public void UpdatePricing(Money monthlyPrice, Money yearlyPrice)

// Feature Management
public void UpdateFeatureLimits(FeatureLimits newLimits)

// Lifecycle Operations
public void Activate()   // Raises EditionActivatedEvent
public void Deactivate() // Raises EditionActivatedEvent
```

**Value Objects Used:**
- `Money` - Monetary amounts with currency
- `FeatureLimits` - Feature quotas and capabilities

**Domain Events:**
- `EditionActivatedEvent` - When edition is activated/deactivated

---

### 2. Subscription Aggregate

**Aggregate Root:** `Subscription`

**Purpose:** Represents a tenant's subscription to an edition with billing details.

**Key Characteristics:**
- Multi-tenant entity (implements `IMultiTenant`)
- Manages subscription lifecycle (Trial → Active → Expired/Cancelled)
- Controls renewal and auto-renewal logic
- Calculates billing dates and remaining days
- Enforces state machine transitions

**Business Invariants:**
- Must reference a valid Edition
- Price must be positive (unless trial)
- End date must be after start date
- Status transitions must follow state machine rules
- Trial days cannot be negative

**Key Methods:**
```csharp
// Lifecycle Operations
public void Renew()                              // Extends subscription period
public void Renew(Money newPrice)                // Renews with price change
public void Cancel()                             // Raises SubscriptionCancelledEvent
public void Suspend()                            // Temporarily suspends
public void Activate()                           // Reactivates suspended subscription
public void MarkAsExpired()                      // Marks as expired

// Business Queries
public bool IsActive()                           // Check if currently active
public int DaysRemaining()                       // Days until expiration

// Configuration
public void UpdateBillingPeriod(BillingPeriod newPeriod, Money newPrice)
public void EnableAutoRenew()
public void DisableAutoRenew()
```

**Value Objects Used:**
- `Money` - Subscription price
- `DateRange` - Subscription period with start/end dates
- `BillingPeriod` - Monthly or Yearly

**Domain Events:**
- `SubscriptionCancelledEvent` - When subscription is cancelled
- `SubscriptionRenewedEvent` - When subscription is renewed

**State Machine:**
```
Trial ──────→ Active ──────→ Expired
  │             │  ↕           │
  └──────────→ Suspended ←─────┘
                 │
                 ↓
              Cancelled
```

**Navigation Properties:**
- `Edition` - Reference to subscribed edition (not part of aggregate)

---

### 3. Invoice Aggregate

**Aggregate Root:** `Invoice`

**Purpose:** Represents a billing invoice for a subscription period.

**Key Characteristics:**
- Multi-tenant entity
- Immutable once created (except status changes)
- Tracks payment information
- Auto-calculates overdue status
- Enforces payment rules

**Business Invariants:**
- Must reference a valid Subscription
- Invoice number must be unique
- Due date must be after invoice date
- Amount must be positive
- Paid invoices cannot be modified
- Billing period must be defined

**Key Methods:**
```csharp
// Payment Operations
public void MarkAsPaid(string paymentMethod, string paymentReference)  // Raises InvoicePaidEvent
public void MarkAsOverdue()                      // Auto-transition to overdue
public void Cancel()                             // Cancel pending invoice

// Business Queries
public bool CanBePaid()                          // Check if payable
public bool CanBeCancelled()                     // Check if cancellable
public bool IsOverdue()                          // Check overdue status
public int DaysOverdue()                         // Days past due date
public int DaysUntilDue()                        // Days remaining until due

// Additional Information
public void AddNotes(string notes)               // Add payment notes
```

**Value Objects Used:**
- `Money` - Invoice amount
- `InvoiceNumber` - Unique invoice identifier
- `DateRange` - Billing period range

**Domain Events:**
- `InvoicePaidEvent` - When invoice is paid

**Navigation Properties:**
- `Subscription` - Reference to billed subscription (not part of aggregate)

---

## 💎 Value Objects

### 1. Money

**Purpose:** Represents monetary value with currency safety.

**Immutability:** ✅ Cannot be modified after creation

**Business Rules:**
- Amount cannot be negative
- Currency must be specified (default: USD)
- Cannot mix different currencies in operations

**Operations:**
```csharp
Money Add(Money other)           // Add same currency
Money Subtract(Money other)      // Subtract same currency
Money Multiply(decimal multiplier)
bool IsZero()
string ToString()                // "49.99 USD"
```

**Factory Methods:**
```csharp
Money.Zero(string currency = "USD")
```

**Example:**
```csharp
var price = new Money(49.99m, "USD");
var doubled = price.Multiply(2);  // 99.98 USD
```

---

### 2. FeatureLimits

**Purpose:** Encapsulates feature quotas and capabilities for an edition.

**Immutability:** ✅ Cannot be modified after creation

**Properties:**
- `MaxUsers` - Maximum number of users allowed
- `MaxProjects` - Maximum number of projects
- `StorageQuotaGB` - Storage limit in gigabytes
- `APICallsPerMonth` - API rate limit
- `EnableAdvancedReports` - Advanced reporting feature flag
- `EnablePrioritySupport` - Priority support access
- `EnableCustomBranding` - Custom branding capability

**Factory Methods:**
```csharp
FeatureLimits.Free()         // Basic free tier
FeatureLimits.Basic()        // Entry-level paid tier
FeatureLimits.Professional() // Mid-tier with advanced features
FeatureLimits.Enterprise()   // Unlimited tier
```

**Business Queries:**
```csharp
bool CanAddUser(int currentUsers)
bool CanAddProject(int currentProjects)
bool HasStorageAvailable(int currentStorageGB)
bool CanMakeAPICall(int currentCalls)
int RemainingUsers(int currentUsers)
int RemainingProjects(int currentProjects)
```

**Example:**
```csharp
var limits = FeatureLimits.Professional();
if (limits.CanAddUser(currentUserCount))
{
    // Allow user creation
}
```

---

### 3. DateRange

**Purpose:** Represents a time period with start and end dates.

**Immutability:** ✅ Cannot be modified after creation

**Business Rules:**
- End date must be after or equal to start date
- Validates date range on construction

**Business Queries:**
```csharp
bool IsActive()              // Current date within range
bool HasExpired()            // Current date past end date
int DaysRemaining()          // Days until end date
int TotalDays()              // Total days in range
bool Contains(DateTime date) // Check if date in range
```

**Extension Methods:**
```csharp
DateRange ExtendByMonths(int months)  // Create new extended range
DateRange ExtendByYears(int years)    // Create new extended range
```

**Example:**
```csharp
var period = new DateRange(DateTime.Today, DateTime.Today.AddMonths(1));
if (period.IsActive() && period.DaysRemaining() < 7)
{
    // Send renewal reminder
}
```

---

### 4. InvoiceNumber

**Purpose:** Unique invoice identifier with format validation.

**Immutability:** ✅ Cannot be modified after creation

**Format:** Typically `INV-{YEAR}{MONTH}-{SEQUENCE}`

**Example:** `INV-202512-00042`

---

## 🎯 Domain Services

### 1. SubscriptionManager

**Purpose:** Orchestrates complex subscription operations involving multiple aggregates.

**Key Responsibilities:**
- Create subscriptions with business rule validation
- Handle subscription renewals
- Manage trial period transitions
- Validate edition availability
- Enforce "one active subscription per tenant" rule
- Calculate pricing based on billing period

**Key Methods:**
```csharp
Task<Subscription> CreateSubscriptionAsync(
    Guid tenantId,
    Guid editionId,
    BillingPeriod billingPeriod,
    DateTime? startDate = null,
    Money customPrice = null,
    bool autoRenew = true,
    int? trialDays = null)

Task RenewSubscriptionAsync(Subscription subscription)
Task UpgradeSubscriptionAsync(Subscription subscription, Edition newEdition)
Task DowngradeSubscriptionAsync(Subscription subscription, Edition newEdition)
```

**Business Rules Enforced:**
- Edition must exist and be active
- Tenant can only have one active subscription
- Price must be positive (unless trial period)
- Only active/expired subscriptions can be renewed
- Upgrade/downgrade validation with proration

**Example Usage:**
```csharp
var subscription = await _subscriptionManager.CreateSubscriptionAsync(
    tenantId: currentTenant.Id,
    editionId: professionalEdition.Id,
    billingPeriod: BillingPeriod.Yearly,
    trialDays: 14
);

await _subscriptionRepository.InsertAsync(subscription);
```

---

### 2. InvoiceManager

**Purpose:** Manages invoice generation and payment processing.

**Key Responsibilities:**
- Generate invoices for subscriptions
- Calculate invoice amounts
- Handle payment recording
- Track overdue invoices
- Generate unique invoice numbers

**Key Methods:**
```csharp
Task<Invoice> GenerateInvoiceAsync(
    Subscription subscription,
    DateTime? invoiceDate = null,
    DateTime? dueDate = null)

Task<InvoiceNumber> GenerateInvoiceNumberAsync()
Task ProcessOverdueInvoicesAsync()
```

**Business Rules Enforced:**
- Invoice number uniqueness
- Due date validation
- Payment method validation
- Overdue detection and marking

---

### 3. TenantProvisioningManager

**Purpose:** Orchestrates complete tenant provisioning including subscription setup.

**Key Responsibilities:**
- Create tenant with ABP Saas module
- Set up initial subscription
- Generate first invoice
- Configure tenant settings
- Provision database and resources

**Key Methods:**
```csharp
Task<TenantProvisioningResult> ProvisionTenantAsync(
    string tenantName,
    string adminEmail,
    string adminPassword,
    Guid editionId,
    BillingPeriod billingPeriod)
```

**Business Rules Enforced:**
- Tenant name uniqueness
- Admin email validation
- Edition availability
- Database provisioning

---

## 📢 Domain Events

Domain events represent significant state changes in the domain and enable loose coupling between aggregates.

### 1. EditionActivatedEvent

**Triggered When:** Edition is activated or deactivated

**Properties:**
```csharp
Guid EditionId
string EditionName
bool IsActivated
```

**Use Cases:**
- Notify subscribers about edition availability changes
- Update cached edition lists
- Send notifications to admins

---

### 2. SubscriptionCancelledEvent

**Triggered When:** Subscription is cancelled by tenant or admin

**Properties:**
```csharp
Guid SubscriptionId
Guid? TenantId
Guid EditionId
DateTime SubscriptionEndDate
```

**Use Cases:**
- Trigger refund processing
- Schedule tenant deactivation
- Send cancellation confirmation email
- Update analytics

---

### 3. SubscriptionRenewedEvent

**Triggered When:** Subscription is renewed (manual or auto-renewal)

**Properties:**
```csharp
Guid SubscriptionId
Guid? TenantId
DateTime NewEndDate
Money RenewalPrice
```

**Use Cases:**
- Generate renewal invoice
- Send renewal confirmation
- Update billing records
- Track renewal metrics

---

### 4. InvoicePaidEvent

**Triggered When:** Invoice payment is successfully recorded

**Properties:**
```csharp
Guid InvoiceId
Guid SubscriptionId
Guid? TenantId
Money Amount
DateTime PaidDate
string PaymentMethod
string PaymentReference
InvoiceNumber InvoiceNumber
```

**Use Cases:**
- Activate/extend subscription
- Send payment receipt
- Update financial reports
- Trigger fulfillment processes

---

### 5. TenantProvisionedEvent

**Triggered When:** New tenant is fully provisioned

**Properties:**
```csharp
Guid TenantId
string TenantName
Guid SubscriptionId
Guid EditionId
```

**Use Cases:**
- Send welcome email
- Initialize tenant data
- Configure tenant settings
- Track onboarding metrics

---

## 🔍 Specifications & Business Rules

### SubscriptionStateMachine

**Purpose:** Enforces valid state transitions for subscriptions.

**Valid Transitions:**
```
Trial → Active       ✅
Trial → Cancelled    ✅
Active → Suspended   ✅
Active → Expired     ✅
Active → Cancelled   ✅
Suspended → Active   ✅
Suspended → Expired  ✅
Expired → Active     ✅ (renewal)
Cancelled → *        ❌ (terminal state)
```

**Usage:**
```csharp
// Called internally by Subscription.ChangeStatus()
SubscriptionStateMachine.ValidateTransition(currentStatus, newStatus);
```

---

## 📂 Repository Interfaces

Defined in Domain layer, implemented in Infrastructure layer.

### IEditionRepository

```csharp
Task<Edition> GetAsync(Guid id);
Task<Edition> FindByNameAsync(string name);
Task<List<Edition>> GetActiveEditionsAsync();
```

### ISubscriptionRepository

```csharp
Task<Subscription> FindActiveByTenantIdAsync(Guid tenantId);
Task<List<Subscription>> GetExpiringSubscriptionsAsync(int daysThreshold);
Task<List<Subscription>> GetSubscriptionsByEditionAsync(Guid editionId);
```

### IInvoiceRepository

```csharp
Task<List<Invoice>> GetOverdueInvoicesAsync();
Task<List<Invoice>> GetInvoicesBySubscriptionAsync(Guid subscriptionId);
Task<Invoice> FindByInvoiceNumberAsync(InvoiceNumber invoiceNumber);
```

---

## 🎨 DDD Patterns Applied

### 1. Aggregate Pattern
- **Edition**, **Subscription**, **Invoice** are aggregate roots
- Each controls its consistency boundary
- External access only through aggregate root
- No direct modification of child entities

### 2. Value Object Pattern
- **Money**, **FeatureLimits**, **DateRange**, **InvoiceNumber**
- Immutable once created
- Equality based on values, not identity
- No side effects in operations

### 3. Domain Service Pattern
- **SubscriptionManager**, **InvoiceManager**, **TenantProvisioningManager**
- Encapsulate business logic that doesn't fit in a single aggregate
- Coordinate operations across multiple aggregates
- Stateless services

### 4. Domain Event Pattern
- Aggregates raise events for significant state changes
- Events are distributed to event handlers
- Enables loose coupling between bounded contexts
- Supports eventual consistency

### 5. Repository Pattern
- Abstracts data access in domain layer
- Provides collection-like interface for aggregates
- Implemented in infrastructure layer
- Supports unit testing with in-memory implementations

### 6. Specification Pattern
- **SubscriptionStateMachine** enforces state transition rules
- Encapsulates business rules
- Reusable across different contexts
- Improves testability

---

## 🏛️ Business Rules Summary

### Edition Rules
1. ✅ Name must be unique and 3-128 characters
2. ✅ Prices cannot be negative
3. ✅ Feature limits required
4. ✅ Only active editions can be subscribed to

### Subscription Rules
1. ✅ One active subscription per tenant
2. ✅ Price must be positive (unless trial)
3. ✅ State transitions must follow state machine
4. ✅ Trial period must be non-negative
5. ✅ End date must be after start date
6. ✅ Cannot cancel already cancelled subscriptions
7. ✅ Only active/expired subscriptions can be renewed

### Invoice Rules
1. ✅ Invoice number must be unique
2. ✅ Due date must be after invoice date
3. ✅ Amount must be positive
4. ✅ Paid invoices cannot be modified
5. ✅ Cancelled invoices cannot be paid
6. ✅ Auto-mark as overdue when past due date

### Money Rules
1. ✅ Amount cannot be negative
2. ✅ Currency required (default: USD)
3. ✅ Cannot mix currencies in operations

### DateRange Rules
1. ✅ End date must be ≥ start date
2. ✅ Validated on construction

---

## 🔄 Entity Lifecycle

### Subscription Lifecycle

```
┌──────────┐
│  Create  │ → Trial (if trialDays > 0)
└──────────┘       │
                   ↓
              ┌────────┐
    ┌────────→│ Active │←────────┐
    │         └────────┘         │
    │              │              │
    │              ↓              │
    │         ┌──────────┐        │
    │         │ Suspended│────────┘
    │         └──────────┘
    │              │
    │              ↓
    │         ┌─────────┐
    └─────────│ Expired │
              └─────────┘
                   │
                   ↓
              ┌───────────┐
              │ Cancelled │ (Terminal)
              └───────────┘
```

### Invoice Lifecycle

```
┌──────────┐
│  Create  │ → Pending
└──────────┘       │
                   ├──→ MarkAsPaid() → Paid (Terminal)
                   │
                   ├──→ IsOverdue() → Overdue
                   │         │
                   │         └──→ MarkAsPaid() → Paid
                   │
                   └──→ Cancel() → Cancelled (Terminal)
```

---

## 🧪 Testing Strategy

### Unit Tests
- Test aggregate business methods
- Test value object operations
- Test domain service logic
- Test specification rules
- Mock repository interfaces

### Integration Tests
- Test complete workflows
- Test database persistence
- Test event distribution
- Test state machine transitions

### Example Test:
```csharp
[Fact]
public async Task Should_Create_Subscription_With_Trial_Period()
{
    // Arrange
    var edition = new Edition(...);
    var tenantId = Guid.NewGuid();
    
    // Act
    var subscription = await _subscriptionManager.CreateSubscriptionAsync(
        tenantId, edition.Id, BillingPeriod.Monthly, trialDays: 14
    );
    
    // Assert
    subscription.Status.ShouldBe(SubscriptionStatus.Trial);
    subscription.TrialEndDate.ShouldNotBeNull();
    subscription.DaysRemaining().ShouldBe(14);
}
```

---

## 📊 Domain Model Relationships

```
┌─────────────┐
│   Edition   │
└─────────────┘
       │ 1
       │
       │ *
┌─────────────────┐
│  Subscription   │───────┐
└─────────────────┘       │ 1
       │ 1                │
       │                  │
       │ *                │ *
┌─────────────┐    ┌──────────┐
│   Invoice   │    │  Tenant  │
└─────────────┘    └──────────┘
                   (ABP Framework)
```

**Relationships:**
- Edition (1) ─→ (∗) Subscription
- Subscription (1) ─→ (∗) Invoice
- Tenant (1) ─→ (∗) Subscription (via TenantId)
- Tenant (1) ─→ (∗) Invoice (via TenantId)

---

## 🎯 Key Principles Applied

1. **Encapsulation** - Business logic inside aggregates, not in services
2. **Immutability** - Value objects cannot be modified
3. **Consistency Boundaries** - Aggregates enforce invariants
4. **Ubiquitous Language** - Domain terms match business terminology
5. **Persistence Ignorance** - Domain layer has no infrastructure dependencies
6. **Explicit Intent** - Method names reveal business operations
7. **Event-Driven** - State changes trigger domain events
8. **Testability** - Pure domain logic is easily testable

---

## 📚 References

- Eric Evans - "Domain-Driven Design"
- Vaughn Vernon - "Implementing Domain-Driven Design"
- ABP Framework DDD Documentation
- Martin Fowler - Enterprise Application Patterns
