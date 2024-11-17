using CommandLine;

namespace IOTDevice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("IOT Hub sample started!");

            var parameterRes = Parser.Default.ParseArguments<Parameter>(args).Value;

            if (parameterRes.IsIOT)
            {
                var hubManager = new IOTHubManager();

                hubManager.SendSampleData();
            }
            else if (parameterRes.IsGPIO)
            {
                var controller = new GPIOController();
                controller.GPIOSample();
            }
            else
            {
                Console.WriteLine("Nothing was executed!");
            }

            Console.WriteLine("IOT Hub sample finished!");
        }
    }
}