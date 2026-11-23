using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Core.Extensions;

/// <summary>
/// Determines how long an <see cref="AuthenticationRequest"/> grants access to its accounts.
/// </summary>
public static class AuthenticationRequestExtensions
{
    /// <summary>
    /// How long the access granted by an authentication request is valid if its validity is unknown.
    /// This is the validity of the GoCardless default agreement.
    /// </summary>
    public static readonly TimeSpan DefaultAccessValidity = TimeSpan.FromDays(90);

    /// <summary>
    /// Gets when the access granted by the authentication request expires.
    /// </summary>
    public static DateTime GetAccessExpiry(this AuthenticationRequest request) =>
        request.CreatedAt + (request.AccessValidForDays is { } days ? TimeSpan.FromDays(days) : DefaultAccessValidity);

    /// <summary>
    /// Whether the authentication request currently grants access to its accounts.
    /// </summary>
    public static bool IsActive(this AuthenticationRequest request, DateTime utcNow)
    {
        return request.Status == AuthenticationStatus.Active && utcNow <= request.GetAccessExpiry();
    }
}
