using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Values;

namespace ModuleTest.SaasService.ValueObjects;

public class InvoiceNumber : ValueObject
{
    public string Value { get; private set; }

    private InvoiceNumber()
    {
        // For EF Core
    }

    private InvoiceNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Invoice number cannot be empty", nameof(value));
        }

        Value = value;
    }

    public static InvoiceNumber Generate(Guid tenantId, DateTime date, int sequenceNumber)
    {
        // Format: INV-2024-11-TENANT123-001
        var tenantPrefix = tenantId.ToString().Substring(0, 8).ToUpper();
        var value = $"INV-{date:yyyy-MM}-{tenantPrefix}-{sequenceNumber:D4}";
        return new InvoiceNumber(value);
    }

    public static InvoiceNumber FromString(string value)
    {
        return new InvoiceNumber(value);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator string(InvoiceNumber invoiceNumber) => invoiceNumber.Value;
}
