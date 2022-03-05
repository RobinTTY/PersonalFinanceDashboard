namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroAccountActivity
{
    public DateTime Date { get; set; }
    public EtoroAccountActivityType Type { get; set; }
    public string Details { get; set; }
    public decimal Amount { get; set; }
    public decimal RealizedEquityChange { get; set; }
    public decimal RealizedEquity { get; set; }
    public decimal Balance { get; set; }
    public long PositionId { get; set; }
    public decimal Nwd { get; set; }
}

public enum EtoroAccountActivityType
{
    Unknown,
    Deposit,
    OpenPosition,
    ProfitLossOfTrade
}