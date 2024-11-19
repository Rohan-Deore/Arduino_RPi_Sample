using System.Configuration;
using Microsoft.Azure.Devices.Client;
using NLog;

namespace IOTDevice
{
    internal class IOTDeviceManager
    {
        private DeviceClient? client;
        private string? machineName = string.Empty;

        private Logger logger = LogManager.GetCurrentClassLogger();

        internal IOTDeviceManager()
        {
            logger.Debug("IOTDevice manager constructor called");
            var connectionString = ConfigurationManager.AppSettings["ConnectionStringDevice"];
            machineName = ConfigurationManager.AppSettings["MachineName"];
            client = DeviceClient.CreateFromConnectionString(connectionString);

            if (machineName == null)
            {
                machineName = string.Empty;
            }
        }

        public void SendSampleData()
        {
            logger.Debug("Sample data message sent.");
            SendMessage("This is user message.");
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
            client?.SendEventAsync(iotMsg.ToMessage());
        }
    }
}