using Npoi.Mapper.Attributes;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroClosedPositions
{
    [Column("Position ID")]
    public long Id { get; set; }
    [Column("Action")]
    public string Action { get; set; }
    [Column("Amount")]
    public decimal Amount { get; set; }
    [Column("Units")]
    public decimal Units { get; set; }
    [Column("Open Date")]
    public DateTime OpenDate { get; set; }
    [Column("Close Date")]
    public DateTime CloseDate { get; set; }
    [Column("Leverage")]
    public ushort Leverage { get; set; }
    [Column("Spread")]
    public decimal Spread { get; set; }
    [Column("Profit")]
    public decimal Profit { get; set; }
    [Column("Open Rate")]
    public decimal OpenRate { get; set; }
    [Column("Close Rate")]
    public decimal CloseRate { get; set; }
    [Column("Take profit rate")]
    public decimal TakeProfitRate { get; set; }
    [Column("Stop lose rate")]
    public decimal StopLoseRate { get; set; }
    [Column("Rollover Fees and Dividends")]
    public decimal RolloverFeesAndDividends { get; set; }
    [Column("Copied From")]
    public string CopiedFrom { get; set; }
    [Column("Type")]
    public EtoroInstrumentType Type { get; set; }
    [Column("ISIN")]
    public string Isin { get; set; }
    [Column("Notes")]
    public string Notes { get; set; }
}