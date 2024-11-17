using Newtonsoft.Json;
using System.Text;
using Microsoft.Azure.Devices.Client;

namespace IOTDevice
{
    internal class IOTMessage
    {
        public string MachineName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public Message ToMessage()
        {
            var serialise = JsonConvert.SerializeObject(this);
            var bytesMsg = Encoding.UTF8.GetBytes(serialise);
            return new Message(bytesMsg);
        }
    }
}