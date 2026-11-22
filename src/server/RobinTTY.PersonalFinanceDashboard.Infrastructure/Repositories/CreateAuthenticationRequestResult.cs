using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// The outcome of creating an <see cref="AuthenticationRequest"/>.
/// </summary>
public abstract record CreateAuthenticationRequestResult
{
    /// <summary>
    /// The authentication request was created and saved.
    /// </summary>
    /// <param name="Request">The created authentication request.</param>
    public sealed record Created(AuthenticationRequest Request) : CreateAuthenticationRequestResult;

    /// <summary>
    /// The institution's data source doesn't support authentication requests, e.g. because it was created manually.
    /// </summary>
    public sealed record NotConnectable : CreateAuthenticationRequestResult;

    /// <summary>
    /// The data provider couldn't create the authentication request.
    /// </summary>
    /// <param name="Error">The error returned by the data provider.</param>
    public sealed record ProviderFailed(ThirdPartyError Error) : CreateAuthenticationRequestResult;
}
