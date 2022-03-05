using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro;

var importer = new EtoroImporter();
importer.ImportAccountStatement(@"C:\Users\Robin\OneDrive\etoro-account-statement-1-1-2022-1-22-2022.xlsx");
