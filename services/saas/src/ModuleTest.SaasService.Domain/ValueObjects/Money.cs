using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Values;

namespace ModuleTest.SaasService.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    
    public string Currency { get; private set; }

    private Money()
    {
        // For EF Core
    }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency cannot be empty", nameof(currency));
        }

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency = "USD") => new Money(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException($"Cannot add money with different currencies: {Currency} and {other.Currency}");
        }

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException($"Cannot subtract money with different currencies: {Currency} and {other.Currency}");
        }

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }

    public bool IsZero() => Amount == 0;

    public override string ToString() => $"{Amount:N2} {Currency}";

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}
