namespace RobinTTY.PersonalFinanceDashboard.DataImport.Etoro.Models;

public class EtoroAccountStatement
{
    public EtoroAccountSummary AccountSummary { get; set; }
    public List<EtoroClosedPositions> ClosedPositions { get; set; }
    public List<EtoroAccountActivity> AccountActivity { get; set; }
    public List<EtoroDividends> Dividends { get; set; }
    public EtoroFinancialSummary FinancialSummary { get; set; }
}