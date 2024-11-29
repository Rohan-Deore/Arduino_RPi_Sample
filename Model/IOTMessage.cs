using Newtonsoft.Json;
using System.Text;
using Microsoft.Azure.Devices;

namespace Model
{
    /// <summary>
    /// Class to keep information Instruments getting turned on/off
    /// </summary>
    public class IOTMessage
    {
        /// <summary>
        /// Device name hub to which Instruments are attached.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Name of instrument being manipulated.
        /// </summary>
        public string InstrumentName { get; set; }

        /// <summary>
        /// Pin number that is being modified
        /// </summary>
        public int Pin { get; set; }

        /// <summary>
        /// Status of instrument on or off
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Instrument which are analog in nature will have status value.
        /// </summary>
        public double StatusValue { get; set; }

        public Message ToMessage()
        {
            var serialise = JsonConvert.SerializeObject(this);
            var bytesMsg = Encoding.UTF8.GetBytes(serialise);
            return new Message(bytesMsg);
        }
    }
}
