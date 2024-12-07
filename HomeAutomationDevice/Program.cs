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
            Console.WriteLine("Starting Home Automation Device");
            var connectionString = ConfigurationManager.AppSettings["ConnectionStringDevice"];
            var machineName = ConfigurationManager.AppSettings["MachineName"];
            var deviceName = ConfigurationManager.AppSettings["DeviceID"];
            if (connectionString == null || machineName == null)
            {
                logger.Fatal("Connection string or machine name is defined in config.");
                return;
            }

            logger.Debug($"Machine name : {machineName}");
            logger.Debug($"Device ID : {deviceName}");

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