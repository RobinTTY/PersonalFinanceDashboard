using System.ComponentModel.DataAnnotations;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

/// <summary>
/// How often data is retrieved again from the data providers.
/// </summary>
public class DataRetrievalConfiguration
{
    /// <summary>
    /// How often the banking institutions are retrieved again.
    /// </summary>
    [Range(typeof(TimeSpan), "00:01:00", "365.00:00:00")]
    public TimeSpan BankingInstitutionsInterval { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// How often the authentication requests are retrieved again.
    /// </summary>
    [Range(typeof(TimeSpan), "00:01:00", "365.00:00:00")]
    public TimeSpan AuthenticationRequestsInterval { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// How often the details and balance of each bank account are retrieved again.
    /// </summary>
    [Range(typeof(TimeSpan), "00:01:00", "365.00:00:00")]
    public TimeSpan BankAccountsInterval { get; set; } = TimeSpan.FromDays(1);

    /// <summary>
    /// How often the transactions of each bank account are retrieved again.
    /// </summary>
    [Range(typeof(TimeSpan), "00:01:00", "365.00:00:00")]
    public TimeSpan TransactionsInterval { get; set; } = TimeSpan.FromDays(1);

    /// <summary>
    /// Gets how often data of the given type is retrieved again.
    /// </summary>
    /// <param name="dataType">The type of the data.</param>
    public TimeSpan GetRetrievalInterval(ThirdPartyDataType dataType) => dataType switch
    {
        ThirdPartyDataType.BankingInstitutions => BankingInstitutionsInterval,
        ThirdPartyDataType.AuthenticationRequests => AuthenticationRequestsInterval,
        ThirdPartyDataType.BankAccounts => BankAccountsInterval,
        ThirdPartyDataType.Transactions => TransactionsInterval,
        _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, "Unknown data type.")
    };
}
