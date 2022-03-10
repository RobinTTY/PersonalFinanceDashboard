namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

/// <summary>
/// Represents the data from the <see cref="EtoroAccountStatementSheet.AccountSummary"/>.
/// </summary>
public class EtoroAccountSummary
{
    /// <summary>
    /// The account details.
    /// </summary>
    public EtoroAccountDetails? AccountDetails { get; set; }
    /// <summary>
    /// Summary of realized account equity.
    /// </summary>
    public EtoroRealizedAccountSummary? RealizedAccountSummary { get; set; }
    /// <summary>
    /// Summary of unrealized account equity.
    /// </summary>
    public EtoroUnrealizedAccountSummary? UnrealizedAccountSummary { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="EtoroAccountSummary"/>.
    /// </summary>
    public EtoroAccountSummary() { }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroAccountSummary"/>.
    /// </summary>
    /// <param name="accountDetails">The account details.</param>
    /// <param name="realizedAccountSummary">Summary of realized account equity.</param>
    /// <param name="unrealizedAccountSummary">Summary of unrealized account equity.</param>
    public EtoroAccountSummary(EtoroAccountDetails? accountDetails, EtoroRealizedAccountSummary? realizedAccountSummary, EtoroUnrealizedAccountSummary? unrealizedAccountSummary)
    {
        AccountDetails = accountDetails;
        RealizedAccountSummary = realizedAccountSummary;
        UnrealizedAccountSummary = unrealizedAccountSummary;
    }
}

/// <summary>
/// General eToro account details.
/// </summary>
public class EtoroAccountDetails
{
    /// <summary>
    /// Full name of the account holder.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Username of the account holder.
    /// </summary>
    public string? Username { get; set; }
    /// <summary>
    /// The currency in which all money amounts are denominated in.
    /// </summary>
    public string? Currency { get; set; }
    /// <summary>
    /// The date on which this account statement was created.
    /// </summary>
    public DateTime CreationDate { get; set; }
    /// <summary>
    /// The starting date of the time period for this account statement.
    /// </summary>
    public DateTime ReportStartDate { get; set; }
    /// <summary>
    /// The ending date of the time period for this account statement.
    /// </summary>
    public DateTime ReportEndDate { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroAccountDetails"/>.
    /// </summary>
    public EtoroAccountDetails() { }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroAccountDetails"/>.
    /// </summary>
    /// <param name="name">Full name of the account holder.</param>
    /// <param name="username">Username of the account holder.</param>
    /// <param name="currency">The currency in which all money amounts are denominated in.</param>
    /// <param name="creationDate">The date on which this account statement was created.</param>
    /// <param name="reportStartDate">The starting date of the time period for this account statement.</param>
    /// <param name="reportEndDate">The ending date of the time period for this account statement.</param>
    public EtoroAccountDetails(string? name, string? username, string? currency, DateTime creationDate, DateTime reportStartDate, DateTime reportEndDate)
    {
        Name = name;
        Username = username;
        Currency = currency;
        CreationDate = creationDate;
        ReportStartDate = reportStartDate;
        ReportEndDate = reportEndDate;
    }
}

/// <summary>
/// Summary of the realized account equity.
/// </summary>
public class EtoroRealizedAccountSummary
{
    /// <summary>
    /// The realized equity at the start of this account statement.
    /// </summary>
    public decimal BeginningRealizedEquity { get; set; }
    /// <summary>
    /// The deposits made in the time period of this account statement.
    /// </summary>
    public decimal Deposits { get; set; }
    /// <summary>
    /// The refunds that were provided in the time period of this account statement.
    /// </summary>
    public decimal Refunds { get; set; }
    /// <summary>
    /// The credits that were provided in the time period of this account statement.
    /// </summary>
    public decimal Credits { get; set; }
    /// <summary>
    /// The adjustments that were made in the time period of this account statement.
    /// </summary>
    public decimal Adjustments { get; set; }
    /// <summary>
    /// The realized profit/loss (closed positions only) in the time period of this account statement.
    /// </summary>
    public decimal ProfitOrLoss { get; set; }
    /// <summary>
    /// The incurred rollover fees (payment that applies if you hold a position overnight, e.g. for CFDs) in the time period of this account statement.
    /// </summary>
    public decimal RolloverFees { get; set; }
    /// <summary>
    /// The withdrawals made in the time period of this account statement.
    /// </summary>
    public decimal Withdrawals { get; set; }
    /// <summary>
    /// The withdrawal fees which were incurred in the time period of this account statement.
    /// </summary>
    public decimal WithdrawalFees { get; set; }
    /// <summary>
    /// The realized equity at the end of this account statement.
    /// </summary>
    public decimal EndingRealizedEquity { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroRealizedAccountSummary"/>.
    /// </summary>
    public EtoroRealizedAccountSummary() { }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroRealizedAccountSummary"/>.
    /// </summary>
    /// <param name="beginningRealizedEquity">The realized equity at the start of this account statement.</param>
    /// <param name="deposits">The deposits made in the time period of this account statement.</param>
    /// <param name="refunds">The refunds that were provided in the time period of this account statement.</param>
    /// <param name="credits">The credits that were provided in the time period of this account statement.</param>
    /// <param name="adjustments">The adjustments that were made in the time period of this account statement.</param>
    /// <param name="profitOrLoss">The realized profit/loss (closed positions only) in the time period of this account statement.</param>
    /// <param name="rolloverFees">The incurred rollover fees (payment that applies if you hold a position overnight, e.g. for CFDs) in the time period of this account statement.</param>
    /// <param name="withdrawals">The withdrawals made in the time period of this account statement.</param>
    /// <param name="withdrawalFees">The withdrawal fees which were incurred in the time period of this account statement.</param>
    /// <param name="endingRealizedEquity">The realized equity at the end of this account statement.</param>
    public EtoroRealizedAccountSummary(decimal beginningRealizedEquity, decimal deposits, decimal refunds, decimal credits, decimal adjustments, decimal profitOrLoss, decimal rolloverFees, decimal withdrawals, decimal withdrawalFees, decimal endingRealizedEquity)
    {
        BeginningRealizedEquity = beginningRealizedEquity;
        Deposits = deposits;
        Refunds = refunds;
        Credits = credits;
        Adjustments = adjustments;
        ProfitOrLoss = profitOrLoss;
        RolloverFees = rolloverFees;
        Withdrawals = withdrawals;
        WithdrawalFees = withdrawalFees;
        EndingRealizedEquity = endingRealizedEquity;
    }
}

/// <summary>
/// Summary of the unrealized account equity.
/// </summary>
public class EtoroUnrealizedAccountSummary
{
    /// <summary>
    /// The unrealized equity at the start of this account statement.
    /// </summary>
    public decimal BeginningUnrealizedEquity { get; set; }
    /// <summary>
    /// The unrealized equity at the end of this account statement.
    /// </summary>
    public decimal EndingUnrealizedEquity { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroUnrealizedAccountSummary"/>.
    /// </summary>
    public EtoroUnrealizedAccountSummary() { }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroUnrealizedAccountSummary"/>.
    /// </summary>
    /// <param name="beginningUnrealizedEquity">The unrealized equity at the start of this account statement.</param>
    /// <param name="endingUnrealizedEquity">The unrealized equity at the end of this account statement.</param>
    public EtoroUnrealizedAccountSummary(decimal beginningUnrealizedEquity, decimal endingUnrealizedEquity)
    {
        BeginningUnrealizedEquity = beginningUnrealizedEquity;
        EndingUnrealizedEquity = endingUnrealizedEquity;
    }
}