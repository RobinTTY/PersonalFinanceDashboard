using System.Diagnostics.CodeAnalysis;

namespace RobinTTY.PersonalFinanceDashboard.Core.DataProviders;

/// <summary>
/// The response of a third party data provider: either the requested result or an error.
/// </summary>
/// <typeparam name="TResult">The type of the requested result.</typeparam>
public sealed record ThirdPartyResponse<TResult> where TResult : notnull
{
    /// <summary>
    /// Whether the request was successful and <see cref="Result"/> holds the requested data.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccessful => Error is null;

    /// <summary>
    /// The requested data, <see langword="null"/> if the request failed.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// The error returned by the provider, <see langword="null"/> if the request was successful.
    /// </summary>
    public ThirdPartyError? Error { get; }

    private ThirdPartyResponse(TResult? result, ThirdPartyError? error)
    {
        Result = result;
        Error = error;
    }

    /// <summary>
    /// Creates the response of a successful request.
    /// </summary>
    public static ThirdPartyResponse<TResult> Success(TResult result) => new(result, null);

    /// <summary>
    /// Creates the response of a failed request.
    /// </summary>
    public static ThirdPartyResponse<TResult> Failure(ThirdPartyError error) => new(default, error);
}
