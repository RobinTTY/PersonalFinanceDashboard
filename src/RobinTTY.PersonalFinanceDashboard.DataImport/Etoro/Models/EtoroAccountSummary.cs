namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroAccountSummary
{
    public EtoroAccountDetails AccountDetails { get; set; }
    public EtoroRealizedAccountSummary RealizedAccountSummary { get; set; }
    public EtoroUnrealizedAccountSummary UnrealizedAccountSummary { get; set; }
}

public class EtoroAccountDetails
{
    /// <summary>
    /// Full name of the account holder.
    /// </summary>
    public string Name { get; set; }

    public string Username { get; set; }
    public string Currency { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ReportStartDate { get; set; }
    public DateTime ReportEndDate { get; set; }
}

public class EtoroRealizedAccountSummary
{
    public decimal BeginningRealizedEquity { get; set; }
    public decimal Deposits { get; set; }
    public decimal Refunds { get; set; }
    public decimal Credits { get; set; }
    public decimal Adjustments { get; set; }
    /// <summary>
    /// The realized profit/loss (closed positions only).
    /// </summary>
    public decimal ProfitOrLoss { get; set; }
    public decimal RolloverFees { get; set; }
    public decimal Withdrawals { get; set; }
    public decimal WithdrawalFees { get; set; }
    public decimal EndingRealizedEquity { get; set; }
}

public class EtoroUnrealizedAccountSummary
{
    public decimal BeginningUnrealizedEquity { get; set; }
    public decimal EndingUnrealizedEquity { get; set; }
}