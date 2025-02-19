using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;

/// <summary>
/// TODO
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="context"></param>
    /// <param name="externalAuthRequest"></param>
    /// <returns></returns>
    public static async Task<OperationType> AddOrUpdateAuthenticationRequest(this ApplicationDbContext context,
        AuthenticationRequest externalAuthRequest)
    {
        var existingAuthRequest =
            context.AuthenticationRequests
                .SingleOrDefault(e => e.ThirdPartyId == externalAuthRequest.ThirdPartyId);

        if (existingAuthRequest != null)
        {
            existingAuthRequest.Status = externalAuthRequest.Status;
            existingAuthRequest.ThirdPartyId = externalAuthRequest.ThirdPartyId;
            existingAuthRequest.AuthenticationLink = externalAuthRequest.AuthenticationLink;
            return OperationType.Update;
        }

        await context.AuthenticationRequests.AddAsync(externalAuthRequest);
        return OperationType.Insert;
    }

    public enum OperationType
    {
        Undefined,
        Insert,
        Update
    }
}
