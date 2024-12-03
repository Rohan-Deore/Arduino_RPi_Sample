using Model;
using System.Configuration;
using NLog;

namespace HomeAutomationServer
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
            Console.WriteLine("Starting Home Automation Server");

            var connectionString = ConfigurationManager.AppSettings["ConnectionStringHub"];
            var eventHubString = ConfigurationManager.AppSettings["EventHub"];
            var machineName = ConfigurationManager.AppSettings["MachineName"];
            var deviceName = ConfigurationManager.AppSettings["DeviceID"];

            if (connectionString == null || machineName == null || 
                eventHubString == null || deviceName == null)
            {
                logger.Fatal("Connection string or machine name is defined in config.");
                return;
            }

            logger.Debug($"Device ID : {deviceName}");

            IOTServerManager deviceMgr;
            try
            {
                deviceMgr = new IOTServerManager(connectionString, eventHubString, machineName, deviceName);
            }
            catch (Exception ex)
            {
                logger.Fatal($"Device manager creation failed due to : {ex}");
                return;
            }

            try
            {
                deviceMgr.Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                return;
            }

            Console.WriteLine("Home automation server stopping..");
            Console.ReadLine();
        }
    }
}