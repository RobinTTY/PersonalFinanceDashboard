namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroDividends
{
    public DateTime DateOfPayment { get; set; }
    public string InstrumentName { get; set; }
    public decimal NetDividendReceived { get; set; }
    public ushort WithholdingTaxRate { get; set; }
    public decimal WithholdingTaxAmount { get; set; }
    public long PositionId { get; set; }
    public EtoroInstrumentType Type { get; set; }
    public string Isin { get; set; }
}