namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroAccountStatement
{
    public EtoroAccountSummary AccountSummary { get; set; }
    public EtoroClosedPositions ClosedPositions { get; set; }
    public EtoroAccountActivity AccountActivity { get; set; }
    public EtoroDividends Dividends { get; set; }
    public EtoroFinancialSummary FinancialSummary { get; set; }
}