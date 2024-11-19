namespace IOTHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("IOTHub program started!!");

            var manager = new IOTHubManager();
            manager.SendSampleData();

            Console.WriteLine("IOTHub program finished!!");
            Console.ReadLine();
        }
    }

}