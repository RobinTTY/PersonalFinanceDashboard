using NPOI.SS.UserModel;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Extensions;

/// <summary>
/// Provides extensions for the <see cref="NPOI"/> library.
/// </summary>
public static class NpoiExtensions
{
    /// <summary>
    /// Gets the cell of the provided <see cref="ISheet"/>, given the row and column index.
    /// </summary>
    /// <param name="workbookSheet">The sheet to retrieve the cell from.</param>
    /// <param name="row">The row index of the cell to retrieve.</param>
    /// <param name="column">The column index of the cell to retrieve.</param>
    /// <returns></returns>
    public static ICell GetCell(this ISheet workbookSheet, int row, int column) => workbookSheet.GetRow(row).GetCell(column);
}