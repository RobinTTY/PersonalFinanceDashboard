using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Core.Extensions;

public static class AuthenticationRequestExtensions
{
    /// <summary>
    /// How long the access granted by an authentication request is valid.
    /// </summary>
    // TODO: EUA may have a different validity than 90 days
    public static readonly TimeSpan AccessValidity = TimeSpan.FromDays(90);

    /// <summary>
    /// Gets when the access granted by the authentication request expires.
    /// </summary>
    public static DateTime GetAccessExpiry(this AuthenticationRequest request) =>
        request.CreatedAt + AccessValidity;

    /// <summary>
    /// Whether the authentication request currently grants access to its accounts.
    /// </summary>
    public static bool IsActive(this AuthenticationRequest request, DateTime utcNow)
    {
        return request.Status == AuthenticationStatus.Active && utcNow <= request.GetAccessExpiry();
    }
}
