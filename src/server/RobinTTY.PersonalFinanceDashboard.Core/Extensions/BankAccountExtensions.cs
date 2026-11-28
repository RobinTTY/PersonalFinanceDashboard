using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Core.Extensions;

/// <summary>
/// Provides extensions for the <see cref="BankAccount"/> type.
/// </summary>
public static class BankAccountExtensions
{
    /// <summary>
    /// Whether the account is synchronized with its data source, because one of its authentication requests
    /// currently grants access. Requires the <see cref="BankAccount.AssociatedAuthenticationRequests"/> to be loaded.
    /// </summary>
    /// <param name="account">The bank account.</param>
    /// <param name="utcNow">The current time.</param>
    public static bool IsSynchronized(this BankAccount account, DateTime utcNow) =>
        !account.IsManual() && account.AssociatedAuthenticationRequests.Any(request => request.IsActive(utcNow));
}
