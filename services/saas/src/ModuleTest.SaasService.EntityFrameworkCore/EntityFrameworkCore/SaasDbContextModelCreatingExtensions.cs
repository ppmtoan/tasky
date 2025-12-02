using Microsoft.EntityFrameworkCore;
using ModuleTest.SaasService.Aggregates.EditionAggregate;
using ModuleTest.SaasService.Aggregates.SubscriptionAggregate;
using ModuleTest.SaasService.Aggregates.BillingAggregate;
using ModuleTest.SaasService.ValueObjects;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace ModuleTest.SaasService.EntityFrameworkCore;

public static class SaasDbContextModelCreatingExtensions
{
    public static void ConfigureSaaS(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        // Ignore value objects - they should only be configured as owned types
        builder.Ignore<DateRange>();
        builder.Ignore<Money>();
        builder.Ignore<InvoiceNumber>();
        builder.Ignore<FeatureLimits>();

        builder.Entity<Edition>(b =>
        {
            b.ToTable(SaasServiceDbProperties.DbTablePrefix + "Editions", SaasServiceDbProperties.DbSchema);
            
            b.ConfigureByConvention();
            
            b.Property(e => e.Name).IsRequired().HasMaxLength(128);
            b.Property(e => e.DisplayName).IsRequired().HasMaxLength(256);
            b.Property(e => e.Description).HasMaxLength(1024);
            
            // Configure Money Value Objects
            b.OwnsOne(e => e.MonthlyPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("MonthlyPrice").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("MonthlyCurrency").HasMaxLength(3).HasDefaultValue("USD").IsRequired();
            }).Navigation(e => e.MonthlyPrice).IsRequired();
            
            b.OwnsOne(e => e.YearlyPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("YearlyPrice").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("YearlyCurrency").HasMaxLength(3).HasDefaultValue("USD").IsRequired();
            }).Navigation(e => e.YearlyPrice).IsRequired();
            
            // Configure FeatureLimits Value Object
            b.OwnsOne(e => e.FeatureLimits, limits =>
            {
                limits.Property(l => l.MaxUsers).HasColumnName("MaxUsers").IsRequired();
                limits.Property(l => l.MaxProjects).HasColumnName("MaxProjects").IsRequired();
                limits.Property(l => l.StorageQuotaGB).HasColumnName("StorageQuotaGB").IsRequired();
                limits.Property(l => l.APICallsPerMonth).HasColumnName("APICallsPerMonth").IsRequired();
                limits.Property(l => l.EnableAdvancedReports).HasColumnName("EnableAdvancedReports").IsRequired();
                limits.Property(l => l.EnablePrioritySupport).HasColumnName("EnablePrioritySupport").IsRequired();
                limits.Property(l => l.EnableCustomBranding).HasColumnName("EnableCustomBranding").IsRequired();
            }).Navigation(e => e.FeatureLimits).IsRequired();
            
            b.HasIndex(e => e.Name);
            b.HasIndex(e => e.IsActive);
        });

        builder.Entity<Subscription>(b =>
        {
            b.ToTable(SaasServiceDbProperties.DbTablePrefix + "Subscriptions", SaasServiceDbProperties.DbSchema);
            
            b.ConfigureByConvention();
            
            // Configure Money Value Object
            b.OwnsOne(s => s.Price, money =>
            {
                money.Property(m => m.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).HasDefaultValue("USD").IsRequired();
            }).Navigation(s => s.Price).IsRequired();
            
            // Configure DateRange Value Object
            b.OwnsOne(s => s.SubscriptionPeriod, period =>
            {
                period.Property(p => p.StartDate).HasColumnName("StartDate").IsRequired();
                period.Property(p => p.EndDate).HasColumnName("EndDate").IsRequired();
            }).Navigation(s => s.SubscriptionPeriod).IsRequired();
            
            b.HasOne(s => s.Edition)
                .WithMany(e => e.Subscriptions)
                .HasForeignKey(s => s.EditionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasIndex(s => s.TenantId);
            b.HasIndex(s => s.EditionId);
            b.HasIndex(s => s.Status);
        });

        builder.Entity<Invoice>(b =>
        {
            b.ToTable(SaasServiceDbProperties.DbTablePrefix + "Invoices", SaasServiceDbProperties.DbSchema);
            
            b.ConfigureByConvention();
            
            // Configure InvoiceNumber Value Object
            b.OwnsOne(i => i.InvoiceNumber, number =>
            {
                number.Property(n => n.Value).HasColumnName("InvoiceNumber").IsRequired().HasMaxLength(64);
            }).Navigation(i => i.InvoiceNumber).IsRequired();
            
            // Configure Money Value Object
            b.OwnsOne(i => i.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).HasDefaultValue("USD").IsRequired();
            }).Navigation(i => i.Amount).IsRequired();
            
            // Configure DateRange Value Object for Billing Period
            b.OwnsOne(i => i.BillingPeriodRange, period =>
            {
                period.Property(p => p.StartDate).HasColumnName("PeriodStart").IsRequired();
                period.Property(p => p.EndDate).HasColumnName("PeriodEnd").IsRequired();
            }).Navigation(i => i.BillingPeriodRange).IsRequired();
            
            b.Property(i => i.PaymentMethod).HasMaxLength(128);
            b.Property(i => i.PaymentReference).HasMaxLength(256);
            b.Property(i => i.Notes).HasMaxLength(1024);
            
            b.HasOne(i => i.Subscription)
                .WithMany()
                .HasForeignKey(i => i.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            b.HasIndex(i => i.TenantId);
            b.HasIndex(i => i.SubscriptionId);
            b.HasIndex(i => i.Status);
            b.HasIndex(i => i.DueDate);
        });
    }
}
