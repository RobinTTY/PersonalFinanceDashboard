using Npoi.Mapper.Attributes;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroAccountActivity
{
    [Column("Date")]
    public DateTime Date { get; set; }
    [Column("Type")]
    public EtoroAccountActivityType Type { get; set; }
    [Column("Details")]
    public string Details { get; set; }
    [Column("Amount")]
    public decimal Amount { get; set; }
    [Column("Realized Equity Change")]
    public decimal RealizedEquityChange { get; set; }
    [Column("Realized Equity")]
    public decimal RealizedEquity { get; set; }
    [Column("Balance")]
    public decimal Balance { get; set; }
    [Column("Position ID")]
    public long PositionId { get; set; }
    [Column("NWA")]
    public decimal Nwd { get; set; }
}

public enum EtoroAccountActivityType
{
    Unknown,
    Deposit,
    OpenPosition,
    ProfitLossOfTrade
}