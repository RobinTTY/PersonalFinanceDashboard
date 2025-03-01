using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// An account is an entity that represents for instance a checking account at a bank or an investment account.
/// </summary>
public class Account : ThirdPartyEntity
{
    /// <summary>
    /// The name of the account.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// A description of this account.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// The current balance of the account.
    /// </summary>
    public decimal? Balance { get; set; }
    /// <summary>
    /// The currency this account is denominated in.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Transactions that are associated with this account.
    /// </summary>
    public List<Transaction> Transactions { get; set; } = [];

    /// <summary>
    /// Creates a new instance of <see cref="Account"/>.
    /// </summary>
    public Account()
    {
        ThirdPartyId = Guid.Empty;
        Name = string.Empty;
        Description = string.Empty;
        Balance = null;
        Currency = string.Empty;
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="Account"/>.
    /// </summary>
    /// <param name="thirdPartyId">The third party id of the account.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">A description of this account.</param>
    /// <param name="balance">The current balance of the account.</param>
    /// <param name="currency">The currency this account is denominated in.</param>
    public Account(Guid thirdPartyId, string? name, string? description, decimal? balance, string? currency)
    {
        ThirdPartyId = thirdPartyId;
        Name = name;
        Description = description;
        Balance = balance;
        Currency = currency;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Account"/>.
    /// </summary>
    /// <param name="thirdPartyId">The third party id of the account.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">A description of this account.</param>
    /// <param name="balance">The current balance of the account.</param>
    /// <param name="currency">The currency this account is denominated in.</param>
    /// <param name="transactions">Transactions that are associated with this account.</param>
    public Account(Guid thirdPartyId, string? name, string? description, decimal? balance, string? currency, List<Transaction> transactions)
    {
        ThirdPartyId = thirdPartyId;
        Name = name;
        Description = description;
        Balance = balance;
        Currency = currency;
        Transactions = transactions;
    }
}
