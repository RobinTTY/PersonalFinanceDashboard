namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

/// <summary>
/// Represents the data from the <see cref="EtoroAccountStatementSheet.FinancialSummary"/>.
/// </summary>
public class EtoroFinancialSummary
{
    /// <summary>
    /// Profit or loss from contracts for difference (CFDs) in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair ProfitOrLossCfds { get; set; }
    /// <summary>
    /// Profit or loss from crypto assets in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair ProfitOrLossCrypto { get; set; }
    /// <summary>
    /// Profit or loss from total return swaps in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair ProfitOrLossTotalReturnSwaps { get; set; }
    /// <summary>
    /// Profit or loss from stocks in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair ProfitOrLossStocks { get; set; }
    /// <summary>
    /// Profit or loss from exchange-traded funds in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair ProfitOrLossEtfs { get; set; }
    /// <summary>
    /// Profit from dividends in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair ProfitStockDividends { get; set; }
    /// <summary>
    /// Profit or loss from contracts for difference (CFDs) dividends in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair ProfitOrLossCfdDividends { get; set; }
    /// <summary>
    /// Income made from refunds in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair IncomeFromRefunds { get; set; }
    /// <summary>
    /// Income made from airdrops in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair IncomeFromAirDrops { get; set; }
    /// <summary>
    /// Income made from staking in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair IncomeFromStaking { get; set; }
    /// <summary>
    /// Commission received from contracts for difference (CFDs) in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair CommissionsCfds { get; set; }
    /// <summary>
    /// Commission received from crypto assets in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair CommissionsCrypto { get; set; }
    /// <summary>
    /// Commission received from trade repositories? (I don't really know what this abbreviation stands for) in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair CommissionsTrs { get; set; }
    /// <summary>
    /// Commission received from stocks in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair CommissionsStocks { get; set; }
    /// <summary>
    /// Commission received from exchange-traded funds in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair CommissionsEtfs { get; set; }
    /// <summary>
    /// Fees incurred in the time period of this account statement.
    /// </summary>
    public EtoroAmountTaxRatePair Fees { get; set; }
}

/// <summary>
/// Represents the amount, tax rate data pair, as it appears on the "Financial Summary" sheet of the eToro account statement.
/// </summary>
public class EtoroAmountTaxRatePair
{
    /// <summary>
    /// The currency amount.
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// The tax rate.
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroAmountTaxRatePair"/>
    /// </summary>
    /// <param name="amount">The currency amount.</param>
    /// <param name="taxRate">The tax rate.</param>
    public EtoroAmountTaxRatePair(decimal amount, decimal taxRate)
    {
        Amount = amount;
        TaxRate = taxRate;
    }
}