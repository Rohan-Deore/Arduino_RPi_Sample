using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel;
using System.Device.Gpio;
using System.Text;

namespace Model
{
    public class IOTDeviceManager
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private DeviceClient? deviceClient = null;
        private string? machineName = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();
        private GpioController controller = new GpioController();

        /// <summary>
        /// List of output pins can be modified. So that command from server can be validated for commands
        /// </summary>
        private List<int> outputPins = new List<int>();

        public IOTDeviceManager(string connectionString, string machineName)
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
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
            outputPins = new List<int>() { 24 };
        }

        public void Run()
        {
            worker.DoWork += Worker_DoWork;

            int pin = 23;
            controller.OpenPin(pin, PinMode.Input);
            foreach (var p in outputPins)
            {
                controller.OpenPin(p, PinMode.Output);
            }

            while (true)
            {
                Console.Write(". ");
                // send status here
                var readValue = (bool)controller.Read(pin);
                logger.Debug($"Read Pin {pin} status : {readValue}");
                IOTMessage message = new IOTMessage(machineName, "Door", pin, readValue);
                deviceClient?.SendEventAsync(message.ToClientMessage());
                Thread.Sleep(10000); // 10 sec sleep
            }
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            ReadMessagesAsync();
        }

        private void UpdateDevice(IOTMessage msg)
        {
            if (msg.DeviceName != machineName)
            {
                logger.Warn($"Message is for {msg.DeviceName} - {msg.InstrumentName} and not for {machineName}");
                return;
            }

            if (outputPins.Contains(msg.Pin))
            {
                controller.Write(msg.Pin, msg.Status);
                logger.Debug($"Output set to pin : {msg.Pin} as {msg.Status}");
            }
            else
            {
                logger.Warn($"Message is for {msg.Pin} which is not output pin");
            }
        }

        private async Task ReadMessagesAsync()
        {
            while (true)
            {
                if (deviceClient == null)
                {
                    logger.Error("Device client is not created here.");
                    return;
                }

                var resMessage = await deviceClient.ReceiveAsync();
                if (resMessage == null)
                {
                    continue;
                }

                // Acknowledge message reception
                await deviceClient.CompleteAsync(resMessage);
                var msgStr = Encoding.UTF8.GetString(resMessage.GetBytes());
                var message = JsonConvert.DeserializeObject<IOTMessage>(msgStr);
                if (message == null)
                {
                    logger.Error("Message is null");
                    continue;
                }

                logger.Debug($"Message received : {msgStr}");
                UpdateDevice(message);

            }
        }
    }
}
