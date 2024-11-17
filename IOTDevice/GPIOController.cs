using System.Device.Gpio;
using System.Threading;

namespace IOTDevice
{
    public class GPIOController
    {
        public void GPIOSample()
        {
            int pin = 18;
            using var controller = new GpioController();
            controller.OpenPin(pin, PinMode.Output);
            bool ledOn = true;
            int count = 0;
            while (count++ < 10)
            {
                controller.Write(pin, ((ledOn) ? PinValue.High : PinValue.Low));
                Thread.Sleep(1000);
                ledOn = !ledOn;
            }
        }
    }
}