using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Core.Utility;

namespace RobinTTY.PersonalFinanceDashboard.Core.Extensions;

public static class AuthenticationRequestExtensions
{
    // TODO: EUA may have a different validity than 90 days
    public static bool IsActive(this AuthenticationRequest request)
    {
        return request.Status == AuthenticationStatus.Active &&
               !DateUtility.IsOlderThan(request.CreatedAt, 90, TimeUnit.Days);
    }
}