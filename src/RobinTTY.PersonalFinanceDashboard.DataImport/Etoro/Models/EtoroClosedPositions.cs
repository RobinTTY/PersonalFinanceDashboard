using Npoi.Mapper.Attributes;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

/// <summary>
/// Represents the data from the <see cref="EtoroAccountStatementSheet.ClosedPositions"/>.
/// </summary>
public class EtoroClosedPositions
{
    /// <summary>
    /// The position id of this position.
    /// </summary>
    [Column("Position ID")]
    public long Id { get; set; }
    /// <summary>
    /// The description of this action.
    /// </summary>
    [Column("Action")]
    public string? Action { get; set; }
    /// <summary>
    /// The amount of money this position was valued at, at the time the position was closed.
    /// </summary>
    [Column("Amount")]
    public decimal Amount { get; set; }
    /// <summary>
    /// The units that were transferred in this position.
    /// </summary>
    [Column("Units")]
    public decimal Units { get; set; }
    /// <summary>
    /// The date on which this position was opened.
    /// </summary>
    [Column("Open Date")]
    public DateTime OpenDate { get; set; }
    /// <summary>
    /// The date on which this position was closed.
    /// </summary>
    [Column("Close Date")]
    public DateTime CloseDate { get; set; }
    /// <summary>
    /// The leverage that was used for this position.
    /// </summary>
    [Column("Leverage")]
    public ushort Leverage { get; set; }
    /// <summary>
    /// The spread of this position.
    /// </summary>
    [Column("Spread")]
    public decimal Spread { get; set; }
    /// <summary>
    /// The profit of this position.
    /// </summary>
    [Column("Profit")]
    public decimal Profit { get; set; }
    /// <summary>
    /// The price at which this position was opened.
    /// </summary>
    [Column("Open Rate")]
    public decimal OpenRate { get; set; }
    /// <summary>
    /// The price at which this position was closed.
    /// </summary>
    [Column("Close Rate")]
    public decimal CloseRate { get; set; }
    /// <summary>
    /// The instrument price at which the position must be closed in order to achieve the desired gain.
    /// </summary>
    [Column("Take profit rate")]
    public decimal TakeProfitRate { get; set; }
    /// <summary>
    /// The instrument price at which the position must be closed in order to limit losses. A value of 0 means that no stop loss was configured.
    /// </summary>
    [Column("Stop lose rate")]
    public decimal StopLoseRate { get; set; }
    /// <summary>
    /// The rollover fess and dividends incurred by this position.
    /// </summary>
    [Column("Rollover Fees and Dividends")]
    public decimal RolloverFeesAndDividends { get; set; }
    /// <summary>
    /// The user this position was copied from if applicable.
    /// </summary>
    [Column("Copied From")]
    public string? CopiedFrom { get; set; }
    /// <summary>
    /// The type of the instrument.
    /// </summary>
    [Column("Type")]
    public EtoroInstrumentType Type { get; set; }
    /// <summary>
    /// The International Securities Identification Number of the traded instrument.
    /// </summary>
    [Column("ISIN")]
    public string? Isin { get; set; }
    /// <summary>
    /// Notes for this position.
    /// </summary>
    [Column("Notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroClosedPositions"/>.
    /// </summary>
    public EtoroClosedPositions() { }

    /// <summary>
    /// Creates a new instance of <see cref="EtoroClosedPositions"/>.
    /// </summary>
    /// <param name="id">The position id of this position.</param>
    /// <param name="action">The description of this action.</param>
    /// <param name="amount">The amount of money this position was valued at, at the time the position was closed.</param>
    /// <param name="units">The units that were transferred in this position.</param>
    /// <param name="openDate">The date on which this position was opened.</param>
    /// <param name="closeDate">The date on which this position was closed.</param>
    /// <param name="leverage">The leverage that was used for this position.</param>
    /// <param name="spread">The spread of this position.</param>
    /// <param name="profit">The profit of this position.</param>
    /// <param name="openRate">The price at which this position was opened.</param>
    /// <param name="closeRate">The price at which this position was closed.</param>
    /// <param name="takeProfitRate">The instrument price at which the position must be closed in order to achieve the desired gain.</param>
    /// <param name="stopLoseRate">The instrument price at which the position must be closed in order to limit losses. A value of 0 means that no stop loss was configured.</param>
    /// <param name="rolloverFeesAndDividends">The rollover fess and dividends incurred by this position.</param>
    /// <param name="copiedFrom">The user this position was copied from if applicable.</param>
    /// <param name="type">The type of the instrument.</param>
    /// <param name="isin">The International Securities Identification Number of the traded instrument.</param>
    /// <param name="notes">Notes for this position.</param>
    public EtoroClosedPositions(long id, string? action, decimal amount, decimal units, DateTime openDate, DateTime closeDate, ushort leverage, decimal spread, decimal profit, decimal openRate, decimal closeRate, decimal takeProfitRate, decimal stopLoseRate, decimal rolloverFeesAndDividends, string? copiedFrom, EtoroInstrumentType type, string? isin, string? notes)
    {
        Id = id;
        Action = action;
        Amount = amount;
        Units = units;
        OpenDate = openDate;
        CloseDate = closeDate;
        Leverage = leverage;
        Spread = spread;
        Profit = profit;
        OpenRate = openRate;
        CloseRate = closeRate;
        TakeProfitRate = takeProfitRate;
        StopLoseRate = stopLoseRate;
        RolloverFeesAndDividends = rolloverFeesAndDividends;
        CopiedFrom = copiedFrom;
        Type = type;
        Isin = isin;
        Notes = notes;
    }
}