using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types;

/// <summary>
/// Fields of <see cref="AuthenticationRequest"/> that aren't stored on the entity.
/// </summary>
[ObjectType<AuthenticationRequest>]
public static partial class AuthenticationRequestNode
{
    static partial void Configure(IObjectTypeDescriptor<AuthenticationRequest> descriptor)
    {
        // The expiry is calculated from these, so they're loaded even if the query doesn't select them
        descriptor.Field(request => request.CreatedAt).IsProjected();
        descriptor.Field(request => request.AccessValidForDays).IsProjected();
    }

    /// <summary>
    /// When the access to the accounts granted by this request expires.
    /// </summary>
    public static DateTime GetAccessExpiresAt([Parent] AuthenticationRequest request) => request.GetAccessExpiry();
}
