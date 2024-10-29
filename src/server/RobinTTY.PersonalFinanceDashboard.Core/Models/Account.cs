namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// An account is an entity that represents for instance a checking account at a bank or an investment account.
/// </summary>
public class Account
{
    // TODO: The graphql data model should use the id scalar type
    // see: https://chillicream.com/docs/hotchocolate/v13/fetching-data/pagination#changing-the-node-type
    /// <summary>
    /// The id of the account.
    /// </summary>
    public Guid Id { get; set; }
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
    public List<Transaction> Transactions { get; set; }
    
    /// <summary>
    /// Creates a new instance of <see cref="Account"/>.
    /// </summary>
    /// <param name="id">The id of the account.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">A description of this account.</param>
    /// <param name="balance">The current balance of the account.</param>
    /// <param name="currency">The currency this account is denominated in.</param>
    public Account(Guid id, string? name, string? description, decimal? balance, string? currency)
    {
        Id = id;
        Name = name;
        Description = description;
        Balance = balance;
        Currency = currency;
        Transactions = [];
    }

    /// <summary>
    /// Creates a new instance of <see cref="Account"/>.
    /// </summary>
    /// <param name="id">The id of the account.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">A description of this account.</param>
    /// <param name="balance">The current balance of the account.</param>
    /// <param name="currency">The currency this account is denominated in.</param>
    /// <param name="transactions">Transactions that are associated with this account.</param>
    public Account(Guid id, string? name, string? description, decimal? balance, string? currency, List<Transaction> transactions)
    {
        Id = id;
        Name = name;
        Description = description;
        Balance = balance;
        Currency = currency;
        Transactions = transactions;
    }
}

/// <summary>
/// Identifies the type of account.
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Account for which the value only changes through transactions (e.g. checking account at a bank).
    /// </summary>
    General,
    /// <summary>
    /// Account for which the value can change without any transaction occurring (e.g. investment account holding stocks).
    /// </summary>
    Investment
}
