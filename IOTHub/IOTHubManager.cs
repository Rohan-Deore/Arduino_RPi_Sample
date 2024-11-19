using System.Configuration;
using Microsoft.Azure.Devices;
using NLog;

namespace IOTHub
{
    internal class IOTHubManager
    {
        private ServiceClient? client;
        private string? machineName = string.Empty;
        private string? deviceID = string.Empty;
        private Logger logger = LogManager.GetCurrentClassLogger();

        internal IOTHubManager()
        {
            logger.Debug("IOTHub manager constructor called");
            var connectionString = ConfigurationManager.AppSettings["ConnectionStringHub"];
            machineName = ConfigurationManager.AppSettings["MachineName"];
            deviceID = ConfigurationManager.AppSettings["DeviceID"];
            client = ServiceClient.CreateFromConnectionString(connectionString);

            if (machineName == null)
            {
                machineName = string.Empty;
            }
        }

        public void SendSampleData()
        {
            logger.Debug("Sample data message sent.");
            SendMessage("This is server message.");
        }

        public void SendMessage(string message)
        {
            if (machineName == null)
            {
                machineName = string.Empty;
                return;
            }

            logger.Debug($"Message sent : {message}");
            IOTMessage iotMsg = new IOTMessage() { MachineName = machineName, Message = message };
            client?.SendAsync(deviceID, iotMsg.ToMessage());
        }
    }
}
