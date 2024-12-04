using System.Device.Gpio;

public class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Application Started");
		using GpioController controller = new GpioController();
		int pin = 22;
		controller.OpenPin(pin, PinMode.Input);
		int outPin = 18;
		controller.OpenPin(outPin, PinMode.Output);

		while(true)
		{
		if((bool)controller.Read(pin))
		{
			controller.Write(outPin, true);
		}
		else
		{
			controller.Write(outPin, false);
		}
		Thread.Sleep(1000);
		}

		Console.WriteLine("Application finished!");
	}
}

