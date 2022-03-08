using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro;

public interface IEtoroImporter
{
    /// <summary>
    /// Imports the <see cref="EtoroAccountStatement"/> from the given file.
    /// </summary>
    /// <param name="filePath">The file path to import the data from.</param>
    /// <returns>The imported <see cref="EtoroAccountStatement"/>.</returns>
    EtoroAccountStatement ImportAccountStatement(string filePath);
}