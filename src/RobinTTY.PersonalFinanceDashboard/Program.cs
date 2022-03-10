using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro;

var etoroImporter = new EtoroImporter();
var accountStatement = etoroImporter.ImportAccountStatement(@"C:\Users\Robin\Desktop\eToro\etoro-account-statement-1-1-2022-3-6-2022.xlsx");
Console.ReadKey();