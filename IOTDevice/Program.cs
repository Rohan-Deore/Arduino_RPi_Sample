﻿public class Program
{
    public static void Main(string[] args)
    {
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Hello, World!");
        var hubManager = new IOTHubManager();
        hubManager.SendSampleData();
    }
}
