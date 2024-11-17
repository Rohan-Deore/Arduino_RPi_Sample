using CommandLine;

public class Parameter
{
    [Option('g', "GPIO", HelpText = "If selected GPIO program will be run")]
    public bool IsGPIO { get; set; }

    [Option('i', "IOT", HelpText = "Azure IOT message hub sample application")]
    public bool IsIOT { get; set; }
}
