using System.Diagnostics.CodeAnalysis;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
public class ThirdPartyResponse<TResponse, TError>
{
    /// <summary>
    /// Whether or not the request was successful and the requested data was returned.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccessful { get; set; }
    /// <summary>
    /// The result returned from the third party service. Null if the request was not successful.
    /// </summary>
    public TResponse? Result { get; set; }
    /// <summary>
    /// The error returned from the third party service. Null if the request was successful.
    /// </summary>
    public TError? Error { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="ThirdPartyResponse{TResponse,TError}"/>.
    /// </summary>
    /// <param name="isSuccessful">Whether or not the request was successful and the requested data was returned.</param>
    /// <param name="result">The result returned from the third party service. Null if the request was not successful.</param>
    /// <param name="error">The error returned from the third party service. Null if the request was successful.</param>
    public ThirdPartyResponse(bool isSuccessful, TResponse? result, TError? error)
    {
        IsSuccessful = isSuccessful;
        Result = result;
        Error = error;
    }
}
