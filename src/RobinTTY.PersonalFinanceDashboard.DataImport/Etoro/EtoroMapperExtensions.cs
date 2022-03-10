using Npoi.Mapper;
using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;
using RobinTTY.PersonalFinanceDashboard.DataImport.Extensions;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro
{
    /// <summary>
    /// Provides extensions for the <see cref="Mapper"/> class in the context of the eToro data importer.
    /// </summary>
    public static class EtoroMapperExtensions
    {
        /// <summary>
        /// Gets the values from the specified <see cref="EtoroAccountStatementSheet"/>.
        /// </summary>
        /// <typeparam name="T">The type representing the worksheet to get the values from.</typeparam>
        /// <param name="mapper">The mapper to use for the operation.</param>
        /// <param name="sheet">The sheet from which to retrieve the values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the values of the specified type (column).</returns>
        public static IEnumerable<T> GetSheetValues<T>(this Mapper mapper, EtoroAccountStatementSheet sheet) where T : class
        {
            var description = sheet.GetDescription();
            if (description != null)
                return mapper.Take<T>(description).Select(row => row.Value);
            throw new Exception($"Missing description of sheet {sheet}.");
        }
    }
}
