using RobinTTY.PersonalFinanceDashboard.Database.Sqlite;

var db = new SqliteAccessService();
db.WriteSampleMetadata();
db.ReadSampleMetadata();
