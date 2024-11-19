using CommandLine;

namespace IOTDevice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("IOT Device sample started!");

            var parameterRes = Parser.Default.ParseArguments<Parameter>(args).Value;

            if (parameterRes.IsIOT)
            {
                Console.WriteLine("IOT Device manager code");
                var hubManager = new IOTDeviceManager();

                hubManager.SendSampleData();
            }
            else if (parameterRes.IsGPIO)
            {
                Console.WriteLine("GPIO controller code");
                var controller = new GPIOController();
                controller.GPIOSample();
            }
            else
            {
                Console.WriteLine("Nothing was executed!");
            }

            Console.WriteLine("IOT Device sample finished!");
        }
    }
}