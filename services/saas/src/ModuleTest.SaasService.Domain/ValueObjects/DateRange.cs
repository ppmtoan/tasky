using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace ModuleTest.SaasService.ValueObjects;

public class DateRange : ValueObject
{
    public DateTime StartDate { get; private set; }
    
    public DateTime EndDate { get; private set; }

    private DateRange()
    {
        // For EF Core
    }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new BusinessException("Saas:InvalidDateRange")
                .WithData("StartDate", startDate)
                .WithData("EndDate", endDate);
        }

        StartDate = startDate;
        EndDate = endDate;
    }

    public bool IsActive()
    {
        var now = DateTime.UtcNow;
        return now >= StartDate && now <= EndDate;
    }

    public bool HasExpired()
    {
        return DateTime.UtcNow > EndDate;
    }

    public int DaysRemaining()
    {
        var days = (EndDate - DateTime.UtcNow).Days;
        return days > 0 ? days : 0;
    }

    public int TotalDays()
    {
        return (EndDate - StartDate).Days;
    }

    public bool Contains(DateTime date)
    {
        return date >= StartDate && date <= EndDate;
    }

    public DateRange ExtendByMonths(int months)
    {
        return new DateRange(EndDate.AddDays(1), EndDate.AddMonths(months));
    }

    public DateRange ExtendByYears(int years)
    {
        return new DateRange(EndDate.AddDays(1), EndDate.AddYears(years));
    }

    public override string ToString() => $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return StartDate;
        yield return EndDate;
    }
}
