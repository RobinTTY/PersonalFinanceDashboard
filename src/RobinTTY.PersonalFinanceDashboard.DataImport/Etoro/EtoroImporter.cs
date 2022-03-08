using System.Globalization;
using Npoi.Mapper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro;

/// <summary>
/// Importer for data from eToro (etoro.com) statements.
/// </summary>
public class EtoroImporter : IEtoroImporter
{
    /// <summary>
    /// Imports the <see cref="EtoroAccountStatement"/> from the given file.
    /// </summary>
    /// <param name="filePath">The file path to import the data from.</param>
    /// <returns>The imported <see cref="EtoroAccountStatement"/>.</returns>
    public EtoroAccountStatement ImportAccountStatement(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var workbook = new XSSFWorkbook(stream);

        // Make adjustments to internal sheet model to fix inconsistencies
        ReplaceDecimalSeparatorOfUnitsColumn(workbook.GetSheet("Closed Positions"));

        // Create mapper and register custom mappings
        var mapper = new Mapper(workbook);
        RegisterCustomMappings(mapper);

        // Import data
        var accountSummary = GetAccountSummary(workbook);
        var closedPositions = mapper.GetSheetValues<EtoroClosedPositions>(EtoroAccountStatementSheet.ClosedPositions).ToList();
        var accountActivity = mapper.GetSheetValues<EtoroAccountActivity>(EtoroAccountStatementSheet.AccountActivity).ToList();
        var dividends = mapper.GetSheetValues<EtoroDividends>(EtoroAccountStatementSheet.Dividends).ToList();
        var financialSummary = GetFinancialSummary(workbook);

        return new EtoroAccountStatement
        {
            AccountSummary = accountSummary,
            ClosedPositions = closedPositions,
            AccountActivity = accountActivity,
            Dividends = dividends,
            FinancialSummary = financialSummary
        };
    }

    private EtoroAccountSummary GetAccountSummary(IWorkbook accountSummaryWorkbook)
    {
        var accountSummarySheet = accountSummaryWorkbook.GetSheet("Account Summary");

        // Represents the structure of the eToro account summary sheet
        const int headerColumnIndex = 0;
        const int valueColumnIndex = 1;
        var documentCategoryStructure = new Dictionary<string, int>
        {
            { "Name", 2 },
            { "Username", 3 },
            { "Currency", 4 },
            { "Date Created", 5 },
            { "Start Date", 6 },
            { "End Date", 7 },
            { "Beginning Realized Equity", 10 },
            { "Deposits", 11 },
            { "Refunds", 12 },
            { "Credits", 13 },
            { "Adjustments", 14 },
            { "Profit or Loss (Closed positions only)", 15 },
            { "Rollover Fees", 16 },
            { "Withdrawals", 17 },
            { "Withdrawal Fees", 18 },
            { "Ending Realized Equity", 19 },
            { "Beginning Unrealized Equity", 22 },
            { "Ending Unrealized Equity", 23 }
        };

        // Check if category structure matches expected structure
        EnsureDocumentHeaders(accountSummarySheet, documentCategoryStructure, headerColumnIndex);

        return new EtoroAccountSummary
        {
            AccountDetails = GetAccountDetails(accountSummarySheet, documentCategoryStructure, valueColumnIndex),
            RealizedAccountSummary = GetRealizedAccountSummary(accountSummarySheet, documentCategoryStructure, valueColumnIndex),
            UnrealizedAccountSummary = GetUnrealizedAccountSummary(accountSummarySheet, documentCategoryStructure, valueColumnIndex)
        };
    }

    private EtoroAccountDetails GetAccountDetails(ISheet sheet, IReadOnlyDictionary<string, int> categories, int valueColumnIndex)
    {
        var name = GetCellByCategoryString(sheet, "Name", categories, valueColumnIndex).StringCellValue;
        var username = GetCellByCategoryString(sheet, "Username", categories, valueColumnIndex).StringCellValue;
        var currency = GetCellByCategoryString(sheet, "Currency", categories, valueColumnIndex).StringCellValue;
        var creationDate = GetCellByCategoryString(sheet, "Date Created", categories, valueColumnIndex).StringCellValue;
        var reportStartDate = GetCellByCategoryString(sheet, "Start Date", categories, valueColumnIndex).StringCellValue;
        var reportEndDate = GetCellByCategoryString(sheet, "End Date", categories, valueColumnIndex).StringCellValue;

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

    private EtoroRealizedAccountSummary GetRealizedAccountSummary(ISheet sheet, IReadOnlyDictionary<string, int> categories, int valueColumnIndex)
    {
        var beginRealizedEquity = GetCellByCategoryString(sheet, "Beginning Realized Equity", categories, valueColumnIndex).NumericCellValue;
        var deposits = GetCellByCategoryString(sheet, "Deposits", categories, valueColumnIndex).NumericCellValue;
        var refunds = GetCellByCategoryString(sheet, "Refunds", categories, valueColumnIndex).NumericCellValue;
        var credits = GetCellByCategoryString(sheet, "Credits", categories, valueColumnIndex).NumericCellValue;
        var adjustments = GetCellByCategoryString(sheet, "Adjustments", categories, valueColumnIndex).NumericCellValue;
        var profitOrLoss = GetCellByCategoryString(sheet, "Profit or Loss (Closed positions only)", categories, valueColumnIndex).NumericCellValue;
        var rolloverFees = GetCellByCategoryString(sheet, "Rollover Fees", categories, valueColumnIndex).NumericCellValue;
        var withdrawals = GetCellByCategoryString(sheet, "Withdrawals", categories, valueColumnIndex).NumericCellValue;
        var withdrawalFees = GetCellByCategoryString(sheet, "Withdrawal Fees", categories, valueColumnIndex).NumericCellValue;
        var endingRealizedEquity = GetCellByCategoryString(sheet, "Ending Realized Equity", categories, valueColumnIndex).NumericCellValue;

        return new EtoroRealizedAccountSummary
        {
            BeginningRealizedEquity = (decimal)beginRealizedEquity,
            Deposits = (decimal)deposits,
            Refunds = (decimal)refunds,
            Credits = (decimal)credits,
            Adjustments = (decimal)adjustments,
            ProfitOrLoss = (decimal)profitOrLoss,
            RolloverFees = (decimal)rolloverFees,
            Withdrawals = (decimal)withdrawals,
            WithdrawalFees = (decimal)withdrawalFees,
            EndingRealizedEquity = (decimal)endingRealizedEquity
        };
    }

    private EtoroUnrealizedAccountSummary GetUnrealizedAccountSummary(ISheet sheet, IReadOnlyDictionary<string, int> categories, int valueColumnIndex)
    {
        var beginningUnrealizedEquity = GetCellByCategoryString(sheet, "Beginning Unrealized Equity", categories, valueColumnIndex).NumericCellValue;
        var endingUnrealizedEquity = GetCellByCategoryString(sheet, "Ending Unrealized Equity", categories, valueColumnIndex).NumericCellValue;

        return new EtoroUnrealizedAccountSummary
        {
            BeginningUnrealizedEquity = (decimal)beginningUnrealizedEquity,
            EndingUnrealizedEquity = (decimal)endingUnrealizedEquity
        };
    }

    private EtoroFinancialSummary GetFinancialSummary(IWorkbook workbook)
    {
        var financialSummarySheet = workbook.GetSheet("Financial Summary");
        var documentCategoryStructure = new Dictionary<string, int>
        {
            { "CFDs (Profit or Loss)", 1 },
            { "Crypto (Profit or Loss)", 2 },
            { "Stocks (Profit or Loss)", 3 },
            { "ETFs (Profit or Loss)", 4 },
            { "Stock Dividends (Profit)", 5 },
            { "CFD Dividends (Profit or Loss)", 6 },
            { "Income from Refunds", 7 },
            { "Commissions (spread) on CFDs", 8 },
            { "Commissions (spread) on Crypto", 9 },
            { "Commissions (spread) on Stocks", 10 },
            { "Commissions (spread) on ETFs", 11 },
            { "Fees", 12 },
        };

        EnsureDocumentHeaders(financialSummarySheet, documentCategoryStructure, 0);
        var cfds = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["CFDs (Profit or Loss)"]);
        var crypto = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Crypto (Profit or Loss)"]);
        var stocks = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Stocks (Profit or Loss)"]);
        var etfs = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["ETFs (Profit or Loss)"]);
        var stockDividends = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Stock Dividends (Profit)"]);
        var cfdDividends = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["CFD Dividends (Profit or Loss)"]);
        var incomeRefunds = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Income from Refunds"]);
        var commissionCfds = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Commissions (spread) on CFDs"]);
        var commissionCrypto = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Commissions (spread) on Crypto"]);
        var commissionStocks = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Commissions (spread) on Stocks"]);
        var commissionEtfs = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Commissions (spread) on ETFs"]);
        var fees = GetAmountTaxRatePair(financialSummarySheet, documentCategoryStructure["Fees"]);
        
        return new EtoroFinancialSummary
        {
            Cfds = cfds,
            Crypto = crypto,
            Stocks = stocks,
            Etfs = etfs,
            StockDividends = stockDividends,
            CfdDividends = cfdDividends,
            IncomeFromRefunds = incomeRefunds,
            CommissionsCfds = commissionCfds,
            CommissionsCrypto = commissionCrypto,
            CommissionsStocks = commissionStocks,
            CommissionsEtfs = commissionEtfs,
            Fees = fees
        };
    }

    /// <summary>
    /// Gets an <see cref="EtoroAmountTaxRatePair"/> from the given <see cref="ISheet"/> and the row where the values appear.
    /// </summary>
    /// <param name="sheet">The sheet from which to get the <see cref="EtoroAmountTaxRatePair"/> (should be from sheet "Financial Summary").</param>
    /// <param name="rowIndex">The row of the sheet from which to get the <see cref="EtoroAmountTaxRatePair"/>.</param>
    /// <returns>The <see cref="EtoroAmountTaxRatePair"/> that was retrieved.</returns>
    private EtoroAmountTaxRatePair GetAmountTaxRatePair(ISheet sheet, int rowIndex)
    {
        const int amountIndex = 1;
        const int taxRateIndex = 2;
        var amount = sheet.GetCell(rowIndex, amountIndex).NumericCellValue;
        // TODO: introduce method to read both en-US/de number format correctly
        var taxRateString = sheet.GetCell(rowIndex, taxRateIndex).StringCellValue;
        var taxRate = decimal.Parse(taxRateString, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"));

        return new EtoroAmountTaxRatePair((decimal)amount, taxRate);
    }

    /// <summary>
    /// Checks whether the internal header model concurs with the retrieved sheet values.
    /// </summary>
    /// <param name="sheet">The sheet to retrieve the values from.</param>
    /// <param name="headerRows">The mapping of headers to rows.</param>
    /// <param name="headerColumnIndex">The index of the headers.</param>
    /// <exception cref="Exception">Thrown if an unknown header value is encountered.</exception>
    private void EnsureDocumentHeaders(ISheet sheet, Dictionary<string, int> headerRows, int headerColumnIndex)
    {
        foreach (var (expectedHeader, row) in headerRows)
        {
            var cell = sheet.GetCell(row, headerColumnIndex);
            if (cell.StringCellValue != expectedHeader)
                throw new Exception("Encountered unknown sheet header.");
        }
    }

    /// <summary>
    /// Registers custom type mappings for data which can't be imported with the default configuration.
    /// </summary>
    /// <param name="mapper">The mapper on which to register the custom mappings.</param>
    private void RegisterCustomMappings(Mapper mapper)
    {
        mapper.Map<EtoroAccountActivity>("Type", activity => activity.Type, (column, target) =>
        {
            if (column.HeaderValue == null || column.CurrentValue == null) return false;
            if (column.CurrentValue is string typeString)
            {
                ((EtoroAccountActivity)target).Type = typeString switch
                {
                    "Deposit" => EtoroAccountActivityType.Deposit,
                    "Open Position" => EtoroAccountActivityType.OpenPosition,
                    "Profit/Loss of Trade" => EtoroAccountActivityType.ProfitLossOfTrade,
                    _ => EtoroAccountActivityType.Unknown
                };
            }
            // TODO: should this return false?
            return true;
        });
        mapper.Map<EtoroDividends>("Withholding Tax Rate (%)", dividend => dividend.WithholdingTaxRate,
            (column, target) =>
            {
                if (column.HeaderValue == null || column.CurrentValue == null) return false;
                if (column.CurrentValue is string valueString)
                {
                    var valuePart = valueString.Split(' ')[0];
                    var value = ushort.Parse(valuePart);
                    ((EtoroDividends)target).WithholdingTaxRate = value;
                }
                return true;
            });
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

    /// <summary>
    /// Gets the cell by querying the document value structure for the given category and retrieving the row/cell index.
    /// </summary>
    /// <param name="sheet">The <see cref="ISheet"/> to retrieve the cells from.</param>
    /// <param name="category">The category to retrieve.</param>
    /// <param name="documentValueStructure">The structure of the <see cref="ISheet"/>, mapping categories to row indices.</param>
    /// <param name="valueColumnIndex">The index of the column containing the values.</param>
    /// <returns></returns>
    private ICell GetCellByCategoryString(ISheet sheet, string category, IReadOnlyDictionary<string, int> documentValueStructure, int valueColumnIndex)
    {
        var row = documentValueStructure[category];
        return sheet.GetCell(row, valueColumnIndex);
    }
}
