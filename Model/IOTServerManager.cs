using Microsoft.Azure.Devices;
using NLog;
using System.ComponentModel;
using System.Text;
using Azure.Messaging.EventHubs.Consumer;

namespace Model
{
    public class IOTServerManager
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private ServiceClient? serverClient = null;
        private string connectionStr = string.Empty;
        private string? machineName = string.Empty;
        private string deviceName = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();
        //private GpioController controller = new GpioController();

        public IOTServerManager(string connectionString, string machineID, string deviceID)
        {
            if (connectionString == null)
            {
                logger.Fatal("Connection string is empty");
                return;
            }

            if (machineID == null)
            {
                machineName = string.Empty;
                logger.Warn("Machine Name is empty.");
                return;
            }

            if (deviceID == null)
            {
                deviceName = string.Empty;
                logger.Warn("Device name is empty");
                return;
            }

            connectionStr = connectionString;
            machineName = machineID;
            deviceName = deviceID;

            logger.Debug("IOTDevice manager constructor called");
            serverClient = ServiceClient.CreateFromConnectionString(connectionString);
        }

        public async Task Run1()
        {
            // Working sample application
            int pin = 24;
            var readValue = true;
            logger.Debug($"Read Pin {pin} status : {readValue}");
            IOTMessage message = new IOTMessage(machineName, "Window", pin, readValue);
            await serverClient?.SendAsync(deviceName, message.ToServerMessage());
        }

        public async Task Run()
        {
            // Set up a way for the user to gracefully shutdown
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            // Run the sample
            await ReceiveMessagesFromDeviceAsync(cts.Token);

            // keep receiving messages.
            // Get device ID from receiving command so that there will always be correct.
            int pin = 24;
            var readValue = true;
            logger.Debug($"Read Pin {pin} status : {readValue}");
            IOTMessage message = new IOTMessage(machineName, "Window", pin, readValue);

            try
            {
                await serverClient?.SendAsync(deviceName, message.ToServerMessage());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            Thread.Sleep(10000);

            //ReceiveFeedback();
        }

        private async Task ReceiveFeedback()
        {
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
                        using var cToken = new CancellationTokenSource();

                        await fbReceiver.CompleteAsync(feedbackBatch, cToken.Token);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }

                Thread.Sleep(1000);
            }

            serverClient?.CloseAsync();
        }

        public void ProcessMessage(FeedbackRecord fbRecord)
        {
            logger.Info("Data needs to be processed here and send some information to devices.");
        }

        private async Task ReceiveMessagesFromDeviceAsync(CancellationToken ct)
        {

            // Create the consumer using the default consumer group using a direct connection to the service.
            // Information on using the client with a proxy can be found in the README for this quick start, here:
            // https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/main/iot-hub/Quickstarts/ReadD2cMessages/README.md#websocket-and-proxy-support
            await using var consumer = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName,
                connectionStr,
                machineName); // ToDo : Rohan there has to be Eventhub name in machine name to get this working.

            Console.WriteLine("Listening for messages on all partitions.");

            try
            {
                // Begin reading events for all partitions, starting with the first event in each partition and waiting indefinitely for
                // events to become available. Reading can be canceled by breaking out of the loop when an event is processed or by
                // signaling the cancellation token.
                //
                // The "ReadEventsAsync" method on the consumer is a good starting point for consuming events for prototypes
                // and samples. For real-world production scenarios, it is strongly recommended that you consider using the
                // "EventProcessorClient" from the "Azure.Messaging.EventHubs.Processor" package.
                //
                // More information on the "EventProcessorClient" and its benefits can be found here:
                //   https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs.Processor/README.md
                await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync(ct))
                {
                    Console.WriteLine($"\nMessage received on partition {partitionEvent.Partition.PartitionId}:");

                    string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
                    Console.WriteLine($"\tMessage body: {data}");

                    Console.WriteLine("\tApplication properties (set by device):");
                    foreach (KeyValuePair<string, object> prop in partitionEvent.Data.Properties)
                    {
                        PrintProperties(prop);
                    }

                    Console.WriteLine("\tSystem properties (set by IoT hub):");
                    foreach (KeyValuePair<string, object> prop in partitionEvent.Data.SystemProperties)
                    {
                        PrintProperties(prop);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // This is expected when the token is signaled; it should not be considered an
                // error in this scenario.
            }
        }

        private static void PrintProperties(KeyValuePair<string, object> prop)
        {
            string propValue = prop.Value is DateTime time
                ? time.ToString("O") // using a built-in date format here that includes milliseconds
                : prop.Value.ToString();

            Console.WriteLine($"\t\t{prop.Key}: {propValue}");
        }
    }
}
