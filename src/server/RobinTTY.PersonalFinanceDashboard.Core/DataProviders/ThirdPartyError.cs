namespace RobinTTY.PersonalFinanceDashboard.Core.DataProviders;

/// <summary>
/// An error returned by a third party data provider.
/// </summary>
/// <param name="Summary">A short description of the error.</param>
/// <param name="Detail">Additional details about the error, if the provider supplied any.</param>
/// <param name="RetryAfter">How long to wait before retrying, if the provider rate limited the request.</param>
public sealed record ThirdPartyError(string Summary, string? Detail = null, TimeSpan? RetryAfter = null)
{
    /// <summary>
    /// The summary and details combined into a single message.
    /// </summary>
    public string Message => Detail is null ? Summary : $"{Summary} {Detail}";
}