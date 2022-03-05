using System.Globalization;
using Npoi.Mapper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;
using RobinTTY.PersonalFinanceDashboard.DataImport.Extensions;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro;

public class EtoroImporter
{
    public EtoroAccountStatement ImportAccountStatement(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var workbook = new XSSFWorkbook(stream);
        
        // Make adjustments to internal sheet model to fix inconsistencies
        ReplaceDecimalSeparatorOfUnitsColumn(workbook.GetSheet("Closed Positions"));

        var accountSummary = GetAccountSummary(workbook);
        var closedPositions = ImportSheetValues<EtoroClosedPositions>(workbook, "Closed Positions").ToList();
        var accountActivitySheet = ImportSheetValues<EtoroAccountActivity>(workbook, "Account Activity").ToList();
        var dividendsSheet = workbook.GetSheet("Dividends");
        var financialSummarySheet = workbook.GetSheet("Financial Summary");

        return null;
    }

    private EtoroAccountSummary GetAccountSummary(IWorkbook accountSummaryWorkbook)
    {
        var accountSummarySheet = accountSummaryWorkbook.GetSheet("Account Summary");

        // represents the structure of the eToro account summary sheet
        var documentCategoryStructure = new Dictionary<string, (int, int)>
        {
            { "Name", (2, 0) },
            { "Username", (3, 0) },
            { "Currency", (4, 0) },
            { "Date Created", (5, 0) },
            { "Start Date", (6, 0) },
            { "End Date", (7, 0) },
            { "Beginning Realized Equity", (10, 0) },
            { "Deposits", (11, 0) },
            { "Refunds", (12, 0) },
            { "Credits", (13, 0) },
            { "Adjustments", (14, 0) },
            { "Profit or Loss (Closed positions only)", (15, 0) },
            { "Rollover Fees", (16, 0) },
            { "Withdrawals", (17, 0) },
            { "Withdrawal Fees", (18, 0) },
            { "Ending Realized Equity", (19, 0) },
            { "Beginning Unrealized Equity", (22, 0) },
            { "Ending Unrealized Equity", (23, 0) }
        };

        var documentValueStructure = documentCategoryStructure
            .ToDictionary(kvp => kvp.Key, kvp => (kvp.Value.Item1, kvp.Value.Item2 + 1));

        // Check if category structure matches expected structure
        foreach (var kvp in documentCategoryStructure)
        {
            var (row, column) = kvp.Value;
            var cell = accountSummarySheet.GetCell(row, column);
            if (!StringValueOfCellIsEqual(cell, kvp.Key))
                throw new Exception("Encountered unknown sheet value");
        }

        return new EtoroAccountSummary
        {
            AccountDetails = GetAccountDetails(accountSummarySheet, documentValueStructure),
            RealizedAccountSummary = GetRealizedAccountSummary(accountSummarySheet, documentValueStructure),
            UnrealizedAccountSummary = GetUnrealizedAccountSummary(accountSummarySheet, documentValueStructure)
        };
    }

    private EtoroAccountDetails GetAccountDetails(ISheet sheet, IReadOnlyDictionary<string, (int, int)> categories)
    {
        var name = GetCellByCategoryString(sheet, "Name", categories).StringCellValue;
        var username = GetCellByCategoryString(sheet, "Username", categories).StringCellValue;
        var currency = GetCellByCategoryString(sheet, "Currency", categories).StringCellValue;
        var creationDate = GetCellByCategoryString(sheet, "Date Created", categories).StringCellValue;
        var reportStartDate = GetCellByCategoryString(sheet, "Start Date", categories).StringCellValue;
        var reportEndDate = GetCellByCategoryString(sheet, "End Date", categories).StringCellValue;
        
        return new EtoroAccountDetails
        {
            Name = name,
            Username = username,
            Currency = currency,
            CreationDate = DateTime.Parse(creationDate),
            ReportStartDate = DateTime.Parse(reportStartDate),
            ReportEndDate = DateTime.Parse(reportEndDate)
        };
    }

    private EtoroRealizedAccountSummary GetRealizedAccountSummary(ISheet sheet, IReadOnlyDictionary<string, (int, int)> categories)
    {
        var beginRealizedEquity = GetCellByCategoryString(sheet, "Beginning Realized Equity", categories).NumericCellValue;
        var deposits = GetCellByCategoryString(sheet, "Deposits", categories).NumericCellValue;
        var refunds = GetCellByCategoryString(sheet, "Refunds", categories).NumericCellValue;
        var credits = GetCellByCategoryString(sheet, "Credits", categories).NumericCellValue;
        var adjustments = GetCellByCategoryString(sheet, "Adjustments", categories).NumericCellValue;
        var profitOrLoss = GetCellByCategoryString(sheet, "Profit or Loss (Closed positions only)", categories).NumericCellValue;
        var rolloverFees = GetCellByCategoryString(sheet, "Rollover Fees", categories).NumericCellValue;
        var withdrawals = GetCellByCategoryString(sheet, "Withdrawals", categories).NumericCellValue;
        var withdrawalFees = GetCellByCategoryString(sheet, "Withdrawal Fees", categories).NumericCellValue;
        var endingRealizedEquity = GetCellByCategoryString(sheet, "Ending Realized Equity", categories).NumericCellValue;

        return new EtoroRealizedAccountSummary
        {
            BeginningRealizedEquity = (decimal) beginRealizedEquity,
            Deposits = (decimal) deposits,
            Refunds = (decimal) refunds,
            Credits = (decimal) credits,
            Adjustments = (decimal) adjustments,
            ProfitOrLoss = (decimal) profitOrLoss,
            RolloverFees = (decimal) rolloverFees,
            Withdrawals = (decimal) withdrawals,
            WithdrawalFees = (decimal) withdrawalFees,
            EndingRealizedEquity = (decimal) endingRealizedEquity
        };
    }

    private EtoroUnrealizedAccountSummary GetUnrealizedAccountSummary(ISheet sheet, IReadOnlyDictionary<string, (int, int)> categories)
    {
        var beginningUnrealizedEquity = GetCellByCategoryString(sheet, "Beginning Unrealized Equity", categories).NumericCellValue;
        var endingUnrealizedEquity = GetCellByCategoryString(sheet, "Ending Unrealized Equity", categories).NumericCellValue;

        return new EtoroUnrealizedAccountSummary
        {
            BeginningUnrealizedEquity = (decimal) beginningUnrealizedEquity,
            EndingUnrealizedEquity = (decimal) endingUnrealizedEquity
        };
    }

    // TODO: replace sheetName with enum
    private IEnumerable<T> ImportSheetValues<T>(IWorkbook workbook, string sheetName) where T : class
    {
        var sheetIndex = workbook.GetSheetIndex(sheetName);
        var mapper = new Mapper(workbook);
        return mapper.Take<T>(sheetIndex).Select(row => row.Value);
    }

    private void RegisterCustomMappings(IWorkbook workbook)
    {
        
        var mapper = new Mapper(workbook);
        
    }

    /// <summary>
    /// For some reason the units column of the "Closed Positions" sheet uses an inconsistent decimal separator.
    /// This method replaces the decimal separator to be consistent with all other separators.
    /// </summary>
    private void ReplaceDecimalSeparatorOfUnitsColumn(ISheet closedPositionsSheet)
    {
        // TODO: All these row/cell numbers should be constants and only be changed in one place
        var usCulture = new CultureInfo("en-US");
        var unitsHeader = closedPositionsSheet.GetRow(0).GetCell(3).StringCellValue;
        if (unitsHeader != "Units")
            throw new Exception($"Units header had unexpected value: {unitsHeader}");

        for (var rowIndex = 1; rowIndex < closedPositionsSheet.LastRowNum; rowIndex++)
        {
            var currentValue = closedPositionsSheet.GetRow(rowIndex).GetCell(3).StringCellValue;
            closedPositionsSheet.GetRow(rowIndex).GetCell(3).SetCellValue(double.Parse(currentValue, usCulture));
        }
    }

    private bool StringValueOfCellIsEqual(ICell cell, string comparisonValue) => cell.StringCellValue == comparisonValue;

    private ICell GetCellByCategoryString(ISheet sheet, string category, IReadOnlyDictionary<string, (int, int)> documentValueStructure)
    {
        var (row, col) = documentValueStructure[category];
        return sheet.GetCell(row, col);
    }

}

public static class MapperExtensions
{
    public static IEnumerable<RowInfo<T>> GetValues<T>(this Mapper mapper, EtoroAccountStatementSheets sheet)
    {
        var description = sheet.GetDescription();
        if(description != null)
            return mapper.Take<>(description)
    }
}