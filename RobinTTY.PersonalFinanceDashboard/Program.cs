using RobinTTY.PersonalFinanceDashboard.DataImport.Etoro;

var importer = new EtoroImporter();
importer.ImportAccountStatement(@"C:\Users\Robin\Downloads\etoro-account-statement-1-1-2022-2-12-2022 (1).xlsx");
