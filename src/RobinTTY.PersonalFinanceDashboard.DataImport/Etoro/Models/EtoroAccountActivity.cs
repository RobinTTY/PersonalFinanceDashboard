using Npoi.Mapper.Attributes;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

/// <summary>
/// Represents the data from the <see cref="EtoroAccountStatementSheet.AccountActivity"/>.
/// </summary>
public class EtoroAccountActivity
{
    /// <summary>
    /// The date on which this account activity occurred.
    /// </summary>
    [Column("Date")]
    public DateTime Date { get; set; }
    /// <summary>
    /// The type of the account activity.
    /// </summary>
    [Column("Type")]
    public EtoroAccountActivityType Type { get; set; }
    /// <summary>
    /// Details of the account activity, such as the traded instruments.
    /// </summary>
    [Column("Details")]
    public string? Details { get; set; }
    /// <summary>
    /// The amount of money this account activity was valued at, at the time the activity occurred.
    /// </summary>
    [Column("Amount")]
    public decimal Amount { get; set; }
    /// <summary>
    /// The amount the realized equity has changed, by this account activity.
    /// </summary>
    [Column("Realized Equity Change")]
    public decimal RealizedEquityChange { get; set; }
    /// <summary>
    /// The realized equity after this account activity.
    /// </summary>
    [Column("Realized Equity")]
    public decimal RealizedEquity { get; set; }
    /// <summary>
    /// The balance of uncommitted funds, which are available to open new positions, after this account activity.
    /// </summary>
    [Column("Balance")]
    public decimal Balance { get; set; }
    /// <summary>
    /// The position id of this account activity.
    /// </summary>
    [Column("Position ID")]
    public long PositionId { get; set; }
    /// <summary>
    /// The non withdrawable amount of this account activity.
    /// </summary>
    [Column("NWA")]
    public decimal NonWithdrawableAmount { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroAccountActivity"/>.
    /// </summary>
    public EtoroAccountActivity() { }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroAccountActivity"/>.
    /// </summary>
    /// <param name="date">The date on which this account activity occurred.</param>
    /// <param name="type">The type of the account activity.</param>
    /// <param name="details">Details of the account activity, such as the traded instruments.</param>
    /// <param name="amount">The amount of money this account activity was valued at, at the time the activity occurred.</param>
    /// <param name="realizedEquityChange">The amount the realized equity has changed, by this account activity.</param>
    /// <param name="realizedEquity">The realized equity after this account activity.</param>
    /// <param name="balance">The balance of uncommitted funds, which are available to open new positions, after this account activity.</param>
    /// <param name="positionId">The position id of this account activity.</param>
    /// <param name="nonWithdrawableAmount">The non withdrawable amount of this account activity.</param>
    public EtoroAccountActivity(DateTime date, EtoroAccountActivityType type, string? details, decimal amount, decimal realizedEquityChange, decimal realizedEquity, decimal balance, long positionId, decimal nonWithdrawableAmount)
    {
        Date = date;
        Type = type;
        Details = details;
        Amount = amount;
        RealizedEquityChange = realizedEquityChange;
        RealizedEquity = realizedEquity;
        Balance = balance;
        PositionId = positionId;
        NonWithdrawableAmount = nonWithdrawableAmount;
    }
}

public enum EtoroAccountActivityType
{
    Unknown,
    Deposit,
    OpenPosition,
    ProfitLossOfTrade
}