﻿using System.ComponentModel;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models
{
    public enum EtoroAccountStatementSheets
    {
        [Description("Account Summary")]
        AccountSummary,
        [Description("Closed Positions")]
        ClosedPositions,
        [Description("Account Activity")]
        AccountActivity,
        [Description("Dividends")]
        Dividends,
        [Description("Financial Summary")]
        FinancialSummary
    }
}
