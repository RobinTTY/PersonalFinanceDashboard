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
        // The expiry is calculated from the creation time, so it's loaded even if the query doesn't select it
        descriptor.Field(request => request.CreatedAt).IsProjected();
    }

    /// <summary>
    /// When the access to the accounts granted by this request expires.
    /// </summary>
    public static DateTime GetAccessExpiresAt([Parent] AuthenticationRequest request) => request.GetAccessExpiry();
}
