using Model;
using System.Configuration;
using NLog;

namespace HomeAutomationDevice
{
    public class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            // Create a device manager instance
            // Use device manager to send pin status to hub at every 10 sec.
            // Create receiver for reading messages from hub
            // Reading messages should toggle status of pins between on and off showing read processing.

            var connectionString = ConfigurationManager.AppSettings["ConnectionStringDevice"];
            var machineName = ConfigurationManager.AppSettings["MachineName"];
            if (connectionString == null || machineName == null)
            {
                logger.Fatal("Connection string or machine name is defined in config.");
                return;
            }

            IOTDeviceManager deviceMgr;
            try
            {
                deviceMgr = new IOTDeviceManager(connectionString, machineName);
            }
            catch (Exception ex)
            {
                logger.Fatal($"Device manager creation failed due to : {ex}");
                return;
            }

            deviceMgr.Run();
        }
    }
}