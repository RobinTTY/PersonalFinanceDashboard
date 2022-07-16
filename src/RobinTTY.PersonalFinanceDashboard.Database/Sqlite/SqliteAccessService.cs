using Dapper;
using System.Data;
using System.Data.SQLite;

namespace RobinTTY.PersonalFinanceDashboard.Database.Sqlite
{
    public class SqliteAccessService
    {
        private const string ConnectionString = "Data Source=application_data.db";
        private readonly SQLiteConnection _dbConnection;

        public SqliteAccessService()
        {
            _dbConnection = new SQLiteConnection(ConnectionString);
        }

        public void ReadSampleMetadata()
        {
            var metadata = _dbConnection.Query<MetadataModel>("select * from application_metadata");
            foreach (var data in metadata)
            {
                Console.WriteLine(data.Id);
                Console.WriteLine(data.InstanceName);
            }
        }

        public void WriteSampleMetadata()
        {
            _dbConnection.Execute($"insert into application_metadata (id, instance_name) values (1, test)");
        }

        public class MetadataModel
        {
            public int Id { get; set; }
            public string InstanceName { get; set; }
        }
    }
}