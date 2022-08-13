using RobinTTY.PersonalFinanceDashboard;
using RobinTTY.PersonalFinanceDashboard.Core;
using RobinTTY.PersonalFinanceDashboard.Database.Sqlite;

await StockMarketDataArchiver.ImportHistoricalPrices();

var db = new SqliteAccessService();
db.WriteSampleMetadata();
db.ReadSampleMetadata();

