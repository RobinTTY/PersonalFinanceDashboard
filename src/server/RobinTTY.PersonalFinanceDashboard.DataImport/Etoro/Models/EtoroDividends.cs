using Npoi.Mapper.Attributes;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

/// <summary>
/// Represents the data from the <see cref="EtoroAccountStatementSheet.ClosedPositions"/>.
/// </summary>
public class EtoroDividends
{
    /// <summary>
    /// The date when the dividend payment was made.
    /// </summary>
    [Column("Date of Payment")]
    public DateTime DateOfPayment { get; set; }
    /// <summary>
    /// The name of the instrument for which the dividend was paid.
    /// </summary>
    [Column("Instrument Name")]
    public string InstrumentName { get; set; }
    /// <summary>
    /// The dividend that was paid out.
    /// </summary>
    [Column("Net Dividend Received (USD)")]
    public decimal NetDividendReceived { get; set; }
    /// <summary>
    /// Tax rate paid to the government by the payer of the income (eToro).
    /// </summary>
    [Column("Withholding Tax Rate (%)")]
    public ushort WithholdingTaxRate { get; set; }
    /// <summary>
    /// Tax amount paid to the government by the payer of the income (eToro).
    /// </summary>
    [Column("Withholding Tax Amount (USD)")]
    public decimal WithholdingTaxAmount { get; set; }
    /// <summary>
    /// The position id of this dividend.
    /// </summary>
    [Column("Position ID")]
    public long PositionId { get; set; }
    /// <summary>
    /// The instrument type for which this dividend payment was made.
    /// </summary>
    [Column("Type")]
    public EtoroInstrumentType Type { get; set; }
    /// <summary>
    /// The International Securities Identification Number of the instrument for which this dividend payment was made.
    /// </summary>
    [Column("ISIN")]
    public string Isin { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroDividends"/>.
    /// </summary>
    public EtoroDividends(){ }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroDividends"/>.
    /// </summary>
    /// <param name="dateOfPayment">The date when the dividend payment was made.</param>
    /// <param name="instrumentName">The name of the instrument for which the dividend was paid.</param>
    /// <param name="netDividendReceived">The dividend that was paid out.</param>
    /// <param name="withholdingTaxRate">Tax rate paid to the government by the payer of the income (eToro).</param>
    /// <param name="withholdingTaxAmount">Tax amount paid to the government by the payer of the income (eToro).</param>
    /// <param name="positionId">The position id of this dividend.</param>
    /// <param name="type">The instrument type for which this dividend payment was made.</param>
    /// <param name="isin">The International Securities Identification Number of the instrument for which this dividend payment was made.</param>
    public EtoroDividends(DateTime dateOfPayment, string instrumentName, decimal netDividendReceived, ushort withholdingTaxRate, decimal withholdingTaxAmount, long positionId, EtoroInstrumentType type, string isin)
    {
        DateOfPayment = dateOfPayment;
        InstrumentName = instrumentName;
        NetDividendReceived = netDividendReceived;
        WithholdingTaxRate = withholdingTaxRate;
        WithholdingTaxAmount = withholdingTaxAmount;
        PositionId = positionId;
        Type = type;
        Isin = isin;
    }
}