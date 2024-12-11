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
                status_time DATETIME NOT NULL,
                status BOOLEAN NOT NULL);";

            command.ExecuteNonQuery();
        }

        public void AddData()
        {
            var command = connection.CreateCommand();

            var date = new DateTime(new DateOnly(2024, 12, 10), new TimeOnly(10, 0));

            command.CommandText = $@"INSERT INTO DeviceData (device_name, status, status_time)
            VALUES ('RaspberryPi-1', 1, '2024-12-10 10:00:00')";


            command.ExecuteNonQuery();
        }

        public void GetData(ref List<Device> device1List, ref List<Device> device2List)
        {
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM DeviceData";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var device = new Device { 
                        DeviceName = reader["device_name"].ToString(),
                        StatusTime = Convert.ToDateTime(reader["status_time"]),
                        Status = Convert.ToBoolean(reader["status"])
                    };

                    if (device.DeviceName == "RaspberryPi-1")
                    {
                        device1List.Add(device);
                    }
                    else if (device.DeviceName == "RaspberryPi-2")
                    {
                        device2List.Add(device);
                    }
                    else 
                    {
                        throw new Exception("Invalid device name");
                    }
                }
            }
        }
    }

    public class Device
    {
        public string DeviceName { get; set; }
        public bool Status { get; set; }
        public DateTime StatusTime { get; set; }
    }
}
