using CommandLine;

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
            // GPIO sample here
        }
        else
        {
            Console.WriteLine("Nothing was executed!");
        }

        Console.WriteLine("IOT Hub sample finished!");
    }
}
