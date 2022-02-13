namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroClosedPositions
{
    public long Id { get; set; }
    public string Action { get; set; }
    public decimal Amount { get; set; }
    public decimal Units { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime CloseDate { get; set; }
    public ushort Leverage { get; set; }
    public decimal Spread { get; set; }
    public decimal Profit { get; set; }
    public decimal OpenRate { get; set; }
    public decimal CloseRate { get; set; }
    public decimal TakeProfitRate { get; set; }
    public decimal StopLoseRate { get; set; }
    public decimal RolloverFeesAndDividends { get; set; }
    public string CopiedFrom { get; set; }
    public EtoroPositionType Type { get; set; }
    public string Isin { get; set; }
    public string Notes { get; set; }
}

public enum EtoroPositionType
{
    Unknown,
    Stocks
}