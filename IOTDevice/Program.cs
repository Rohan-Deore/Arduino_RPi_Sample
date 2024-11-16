using System.Configuration;
using Microsoft.Azure.Devices.Client;

public class Program
{
    public static void Main(string[] args)
    {
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, World!");
        var hubManager = new IOTHubManager();
        hubManager.SendSampleData();
    }
}

internal class IOTHubManager
{
    public void SendSampleData()
    {
        var connectionString = ConfigurationManager.AppSettings["PCIPADDRESS"];
        var client = DeviceClient.CreateFromConnectionString(".");
    }

}