using Microsoft.Azure.Devices;
using NLog;
using System.ComponentModel;
using System.Text;
using Azure.Messaging.EventHubs.Consumer;
using Newtonsoft.Json;

namespace Model
{
    public class IOTServerManager
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private ServiceClient? serverClient = null;
        private string connectionStr = string.Empty;
        private string eventHubStr = string.Empty;
        private string? machineName = string.Empty;
        private string deviceName = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();

        public delegate void StatusDelegate(string machineName, bool status, DateTime time);
        public event StatusDelegate StatusEvent;
        //private GpioController controller = new GpioController();

        public IOTServerManager(string connectionString, string eventHub, string machineID, string deviceID)
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
            eventHubStr = eventHub;
            machineName = machineID;
            deviceName = deviceID;

            logger.Debug("IOTDevice manager constructor called");
            serverClient = ServiceClient.CreateFromConnectionString(connectionString);
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
            //await SendDummyMessage();

            Thread.Sleep(10000);

            //ReceiveFeedback();
        }

        private async Task SendMessage(string instrument, int pin, bool status)
        {

            // keep receiving messages.
            // Get device ID from receiving command so that there will always be correct.
            logger.Debug($"Write {instrument} Pin : {pin} status : {status}");
            IOTMessage message = new IOTMessage(machineName, instrument, pin, status);

            try
            {
                await serverClient?.SendAsync(deviceName, message.ToServerMessage());
            }
            catch (Exception ex)
            {
                // even if there is error for one of the message others should be processed hence no throw here
                logger.Error(ex);
            }
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
                            //ProcessMessage(record);
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

        private async Task ReceiveMessagesFromDeviceAsync(CancellationToken ct)
        {
            Console.WriteLine("EventHub consumer client...");
            // Create the consumer using the default consumer group using a direct connection to the service.
            // Information on using the client with a proxy can be found in the README for this quick start, here:
            // https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/main/iot-hub/Quickstarts/ReadD2cMessages/README.md#websocket-and-proxy-support
            await using var consumer = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName,
                eventHubStr,
                machineName);

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
                    string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
                    logger.Debug($"\tMessage body: {data}");

                    var message = JsonConvert.DeserializeObject<IOTMessage>(data);
                    if (message == null)
                    {
                        logger.Error("Message is null");
                        continue;
                    }

                    if (message.InstrumentName == "Door" && message.Status)
                    {
                        await SendMessage("Window", 18, true);
                    }
                    else if (message.InstrumentName == "Door" && !message.Status)
                    {
                        await SendMessage("Window", 18, false);
                    }
                    else 
                    {
                        logger.Warn("Invalid data received by event hub");
                    }
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

                    StatusEvent?.Invoke(message.DeviceName, message.Status, partitionEvent.Data.EnqueuedTime.DateTime);
                    //Thread.Sleep(1000);
                }
            }
            catch (TaskCanceledException ex)
            {
                // This is expected when the token is signaled; it should not be considered an
                // error in this scenario.
                logger.Error(ex);
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
