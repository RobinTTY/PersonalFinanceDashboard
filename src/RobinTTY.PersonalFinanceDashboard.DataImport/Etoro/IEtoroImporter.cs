using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro;

/// <summary>
/// Provides functionality to import data from eToro (www.etoro.com) data sources.
/// </summary>
public interface IEtoroImporter
{
    /// <summary>
    /// Imports the <see cref="EtoroAccountStatement"/> from the given file.
    /// </summary>
    /// <param name="filePath">The file path to import the data from.</param>
    /// <returns>The imported <see cref="EtoroAccountStatement"/>.</returns>
    EtoroAccountStatement ImportAccountStatement(string filePath);
}