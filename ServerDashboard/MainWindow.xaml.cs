using Model;
using NLog;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Configuration;
using System.Windows;

namespace ServerDashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PlotModel MyModel { get; private set; }
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            InitializeComponent();

            logger.Debug($"IOT Dashboard starting..");

            DataContext = this;
            var model = new PlotModel { Title = "Device 1" };
            var series = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };

            // Add X and Y axes
            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time", StringFormat = "MMM dd, yy HH:mm" });
            model.Axes.Add(new CategoryAxis { Position = AxisPosition.Left, Title = "Value", ItemsSource = new[] { "False", "True" } });
            model.Series.Add(series);

            MyModel = model;

            var connectionString = ConfigurationManager.AppSettings["ConnectionStringHub"];
            var eventHubString = ConfigurationManager.AppSettings["EventHub"];
            var machineName = ConfigurationManager.AppSettings["MachineName"];
            var deviceName = ConfigurationManager.AppSettings["DeviceID"];

            if (connectionString == null || machineName == null ||
                eventHubString == null || deviceName == null)
            {
                logger.Fatal("Connection string or machine name is defined in config.");
                return;
            }

            IOTServerManager deviceMgr;
            try
            {
                deviceMgr = new IOTServerManager(connectionString, eventHubString, machineName, deviceName);
                deviceMgr.StatusEvent += DeviceMgr_StatusEvent;
                Task.Run(() =>
                {
                    deviceMgr.Run();
                });
            }
            catch (Exception ex)
            {
                logger.Fatal($"Device manager creation failed due to : {ex}");
                return;
            }
        }

        private void DeviceMgr_StatusEvent(bool status, DateTime time)
        {
            var series = MyModel.Series[0] as LineSeries;
            var statusVal = status ? 1 : 0;
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), statusVal));

            MyModel.InvalidatePlot(true);
        }

        private void BinaryData(PlotModel model, LineSeries series)
        {
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 1, 1)), 1));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 1, 2)), 0));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 1, 3)), 1));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 1, 4)), 0));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 1, 5)), 1));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 1, 6)), 0));

            // Add X and Y axes
            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time", StringFormat = "MMM dd, yyyy" });
            model.Axes.Add(new CategoryAxis { Position = AxisPosition.Left, Title = "Value", ItemsSource = new[] { "False", "True" } });
        }
    }
}