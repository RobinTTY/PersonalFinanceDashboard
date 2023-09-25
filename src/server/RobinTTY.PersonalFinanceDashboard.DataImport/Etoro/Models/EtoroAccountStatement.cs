namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

/// <summary>
/// Represents the data from the eToro account statement, which can be downloaded from ones account.
/// </summary>
public class EtoroAccountStatement
{
    /// <summary>
    /// Represents the data from the <see cref="EtoroAccountStatementSheet.AccountSummary"/>.
    /// </summary>
    public EtoroAccountSummary? AccountSummary { get; set; }
    /// <summary>
    /// Represents the data from the <see cref="EtoroAccountStatementSheet.ClosedPositions"/>.
    /// </summary>
    public List<EtoroClosedPositions>? ClosedPositions { get; set; }
    /// <summary>
    /// Represents the data from the <see cref="EtoroAccountStatementSheet.AccountActivity"/>.
    /// </summary>
    public List<EtoroAccountActivity>? AccountActivity { get; set; }
    /// <summary>
    /// Represents the data from the <see cref="EtoroAccountStatementSheet.Dividends"/>.
    /// </summary>
    public List<EtoroDividends>? Dividends { get; set; }
    /// <summary>
    /// Represents the data from the <see cref="EtoroAccountStatementSheet.FinancialSummary"/>.
    /// </summary>
    public EtoroFinancialSummary? FinancialSummary { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroAccountStatement"/>.
    /// </summary>
    public EtoroAccountStatement() { }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroAccountStatement"/>.
    /// </summary>
    /// <param name="accountSummary">Represents the data from the <see cref="EtoroAccountStatementSheet.AccountSummary"/>.</param>
    /// <param name="closedPositions">Represents the data from the <see cref="EtoroAccountStatementSheet.ClosedPositions"/>.</param>
    /// <param name="accountActivity">Represents the data from the <see cref="EtoroAccountStatementSheet.AccountActivity"/>.</param>
    /// <param name="dividends">Represents the data from the <see cref="EtoroAccountStatementSheet.Dividends"/>.</param>
    /// <param name="financialSummary">Represents the data from the <see cref="EtoroAccountStatementSheet.FinancialSummary"/>.</param>
    public EtoroAccountStatement(EtoroAccountSummary? accountSummary, List<EtoroClosedPositions>? closedPositions, List<EtoroAccountActivity>? accountActivity, List<EtoroDividends>? dividends, EtoroFinancialSummary? financialSummary)
    {
        AccountSummary = accountSummary;
        ClosedPositions = closedPositions;
        AccountActivity = accountActivity;
        Dividends = dividends;
        FinancialSummary = financialSummary;
    }
}