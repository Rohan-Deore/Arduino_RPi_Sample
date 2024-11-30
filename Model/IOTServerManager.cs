using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel;
using System.Device.Gpio;
using System.Text;


namespace Model
{
    public class IOTServerManager
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private ServiceClient? serverClient = null;
        private string? machineName = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();
        //private GpioController controller = new GpioController();

        public IOTServerManager(string connectionString, string machineName)
        {
            if (connectionString == null)
            {
                logger.Fatal("Connection string is empty");
                return;
            }

            if (machineName == null)
            {
                machineName = string.Empty;
                logger.Warn("Machine Name is empty.");
                return;
            }

            logger.Debug("IOTDevice manager constructor called");
            serverClient = ServiceClient.CreateFromConnectionString(connectionString);
        }

        public async Task Run()
        {
            // keep receiving messages.
            // Get device ID from receiving command so that there will always be correct.

            serverClient?.OpenAsync();
            var fbReceiver = serverClient?.GetFeedbackReceiver();
            if (fbReceiver == null)
            {
                logger.Error("Service client feedback receiver is null");
                return;
            }

            while (true)
            {
                try
                {
                    Console.Write(". ");
                    var token = new CancellationTokenSource();
                    var feedbackBatch = await fbReceiver.ReceiveAsync(token.Token);

                    if (feedbackBatch != null)
                    {
                        logger.Debug("Feedback received:");
                        foreach (var record in feedbackBatch.Records)
                        {
                            logger.Debug($"DeviceId: {record.DeviceId}, Status: {record.StatusCode}, Description: {record.Description}");
                            ProcessMessage(record);
                        }

                        // Complete the feedback batch
                        var cts = new CancellationTokenSource();

                        await fbReceiver.CompleteAsync(feedbackBatch, cts.Token);
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }

            serverClient?.CloseAsync();
        }

        public void ProcessMessage(FeedbackRecord fbRecord)
        {
            logger.Info("Data needs to be processed here and send some information to devices.");
        }
    }
}
