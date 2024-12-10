using Microsoft.Data.Sqlite;
using NLog;

namespace Database
{
    public class DatabaseManager : IDisposable
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private SqliteConnection connection;

        public DatabaseManager()
        {
            connection = new SqliteConnection("Data Source=iot.db");
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
        }

        public void CreateDatabase()
        {
            var command = connection.CreateCommand();
            command.CommandText = @"CREATE TABLE DeviceData (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                device_name TEXT NOT NULL,
                status BOOLEAN NOT NULL);";

            command.ExecuteNonQuery();
        }
    }
}
