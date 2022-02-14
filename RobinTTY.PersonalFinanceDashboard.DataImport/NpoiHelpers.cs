using NPOI.SS.UserModel;

namespace RobinTTY.PersonalFinanceDashboard.DataImport;

public static class NpoiHelpers
{
    public static ICell GetCell(this ISheet workbookSheet, int row, int column)
    {
        return workbookSheet.GetRow(row).GetCell(column);
    }
}