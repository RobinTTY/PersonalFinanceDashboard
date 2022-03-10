using System.ComponentModel;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models
{
    /// <summary>
    /// Represents the account sheets which are contained in a <see cref="EtoroAccountStatement"/>.
    /// </summary>
    public enum EtoroAccountStatementSheet
    {
        /// <summary>
        /// The Account Summary sheet.
        /// </summary>
        [Description("Account Summary")]
        AccountSummary,
        /// <summary>
        /// The Closed Positions sheet.
        /// </summary>
        [Description("Closed Positions")]
        ClosedPositions,
        /// <summary>
        /// The Account Activity sheet.
        /// </summary>
        [Description("Account Activity")]
        AccountActivity,
        /// <summary>
        /// The dividends sheet.
        /// </summary>
        [Description("Dividends")]
        Dividends,
        /// <summary>
        /// The Financial Summary sheet.
        /// </summary>
        [Description("Financial Summary")]
        FinancialSummary
    }
}
