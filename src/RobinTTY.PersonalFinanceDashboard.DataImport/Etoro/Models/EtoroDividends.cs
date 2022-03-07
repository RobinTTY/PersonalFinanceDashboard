using Npoi.Mapper.Attributes;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroDividends
{
    [Column("Date of Payment")]
    public DateTime DateOfPayment { get; set; }
    [Column("Instrument Name")]
    public string InstrumentName { get; set; }
    [Column("Net Dividend Received (USD)")]
    public decimal NetDividendReceived { get; set; }
    [Column("Withholding Tax Rate (%)")]
    public ushort WithholdingTaxRate { get; set; }
    [Column("Withholding Tax Amount (USD)")]
    public decimal WithholdingTaxAmount { get; set; }
    [Column("Position ID")]
    public long PositionId { get; set; }
    [Column("Type")]
    public EtoroInstrumentType Type { get; set; }
    [Column("ISIN")]
    public string Isin { get; set; }
}