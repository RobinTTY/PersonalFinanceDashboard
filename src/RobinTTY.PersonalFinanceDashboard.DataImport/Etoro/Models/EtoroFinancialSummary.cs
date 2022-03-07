namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroFinancialSummary
{
    public EtoroAmountTaxRatePair Cfds { get; set; }
    public EtoroAmountTaxRatePair Crypto { get; set; }
    public EtoroAmountTaxRatePair Stocks { get; set; }
    public EtoroAmountTaxRatePair Etfs { get; set; }
    public EtoroAmountTaxRatePair StockDividends { get; set; }
    public EtoroAmountTaxRatePair CfdDividends { get; set; }
    public EtoroAmountTaxRatePair IncomeFromRefunds { get; set; }
    public EtoroAmountTaxRatePair CommissionsCfds { get; set; }
    public EtoroAmountTaxRatePair CommissionsCrypto { get; set; }
    public EtoroAmountTaxRatePair CommissionsStocks { get; set; }
    public EtoroAmountTaxRatePair CommissionsEtfs { get; set; }
    public EtoroAmountTaxRatePair Fees { get; set; }
}

public class EtoroAmountTaxRatePair
{
    public decimal Amount { get; set; }
    public decimal TaxRate { get; set; }

    public EtoroAmountTaxRatePair(decimal amount, decimal taxRate)
    {
        Amount = amount;
        TaxRate = taxRate;
    }
}