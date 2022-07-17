using Dapper;
using System.Data.SQLite;

namespace RobinTTY.PersonalFinanceDashboard.Database.Sqlite
{
    public class SqliteAccessService
    {
        private const string ConnectionString = "Data Source=Sqlite\\application_data.db";
        private readonly SQLiteConnection _dbConnection;

        public SqliteAccessService()
        {
            _dbConnection = new SQLiteConnection(ConnectionString);
        }

        public void ReadSampleMetadata()
        {
            var metadata = _dbConnection.Query<MetadataModel>("SELECT * FROM ApplicationMetadata");
            foreach (var data in metadata)
            {
                Console.WriteLine(data.Id);
                Console.WriteLine(data.InstanceName);
            }
        }

        public void WriteSampleMetadata()
        {
            _dbConnection.Execute("INSERT INTO ApplicationMetadata (Id, InstanceName) VALUES (1, 'test')");
        }

        public class MetadataModel
        {
            public int Id { get; set; }
            public string InstanceName { get; set; }
        }
    }
}