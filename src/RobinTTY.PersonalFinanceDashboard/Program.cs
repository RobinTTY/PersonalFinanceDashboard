using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro;

var importer = new EtoroImporter();
var accountStatement = importer.ImportAccountStatement(@"C:\Users\Robin\Desktop\eToro\etoro-account-statement-1-1-2022-2-12-2022.xlsx");
