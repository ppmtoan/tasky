using System.Collections.Generic;
using ModuleTest.SaasService.Enums;
using Volo.Abp;

namespace ModuleTest.SaasService.Aggregates.SubscriptionAggregate;

/// <summary>
/// State machine for validating subscription status transitions.
/// Ensures only valid status changes are allowed according to business rules.
/// </summary>
public static class SubscriptionStateMachine
{
    private static readonly Dictionary<SubscriptionStatus, HashSet<SubscriptionStatus>> ValidTransitions = new()
    {
        [SubscriptionStatus.Trial] = new()
        {
            SubscriptionStatus.Active,
            SubscriptionStatus.Cancelled,
            SubscriptionStatus.Expired
        },
        [SubscriptionStatus.Active] = new()
        {
            SubscriptionStatus.Suspended,
            SubscriptionStatus.Cancelled,
            SubscriptionStatus.Expired
        },
        [SubscriptionStatus.Suspended] = new()
        {
            SubscriptionStatus.Active,
            SubscriptionStatus.Cancelled,
            SubscriptionStatus.Expired
        },
        [SubscriptionStatus.Expired] = new()
        {
            SubscriptionStatus.Active,
            SubscriptionStatus.Cancelled
        },
        [SubscriptionStatus.Cancelled] = new()
        {
            // Terminal state - no transitions allowed
        }
    };

    /// <summary>
    /// Checks if a transition from one status to another is valid.
    /// </summary>
    public static bool CanTransition(SubscriptionStatus from, SubscriptionStatus to)
    {
        // Same status is always valid (idempotent operations)
        if (from == to)
        {
            return true;
        }

        return ValidTransitions.TryGetValue(from, out var allowedTransitions) 
               && allowedTransitions.Contains(to);
    }

    /// <summary>
    /// Validates a status transition and throws an exception if invalid.
    /// </summary>
    public static void ValidateTransition(SubscriptionStatus from, SubscriptionStatus to)
    {
        if (!CanTransition(from, to))
        {
            throw new BusinessException(SaasServiceErrorCodes.InvalidSubscriptionStatusTransition)
                .WithData("CurrentStatus", from.ToString())
                .WithData("RequestedStatus", to.ToString())
                .WithData("Message", $"Cannot transition from {from} to {to}");
        }
    }

    /// <summary>
    /// Gets all valid transitions from a given status.
    /// </summary>
    public static HashSet<SubscriptionStatus> GetValidTransitions(SubscriptionStatus from)
    {
        return ValidTransitions.TryGetValue(from, out var transitions) 
            ? new HashSet<SubscriptionStatus>(transitions) 
            : new HashSet<SubscriptionStatus>();
    }
}
