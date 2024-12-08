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
        public PlotModel MyModelLB { get; private set; }
        public PlotModel MyModelRT { get; private set; }
        public PlotModel MyModelRB { get; private set; }

        private static Logger logger = LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            InitializeComponent();

            logger.Debug($"IOT Dashboard starting..");

            DataContext = this;

            var model = new PlotModel { Title = "Device 1" };
            var series = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };

            // Add X and Y axes
            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time", StringFormat = "dd/MM/yy HH:mm", Angle=45 });
            model.Axes.Add(new CategoryAxis { Position = AxisPosition.Left, Title = "Value", ItemsSource = new[] { "False", "True" } });
            model.Series.Add(series);

            MyModel = model;

            var modelRT = new PlotModel { Title = "Device 2" };
            var series1 = new LineSeries { Title = "Series 2", MarkerType = MarkerType.Circle };

            // Add X and Y axes
            modelRT.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time", StringFormat = "dd/MM/yy HH:mm", Angle = 45 });
            modelRT.Axes.Add(new CategoryAxis { Position = AxisPosition.Left, Title = "Value", ItemsSource = new[] { "False", "True" } });
            modelRT.Series.Add(series1);

            MyModelRT = modelRT;


            var modelLB = new PlotModel { Title = "Device 3" };
            var series2 = new LineSeries { Title = "Series 3", MarkerType = MarkerType.Circle };

            // Add X and Y axes
            modelLB.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time", StringFormat = "dd/MM/yy HH:mm", Angle = 45 });
            modelLB.Axes.Add(new CategoryAxis { Position = AxisPosition.Left, Title = "Value", ItemsSource = new[] { "False", "True" } });
            modelLB.Series.Add(series2);
            Task.Run(() => {
                DummyDatainLB(modelLB, series2);
                });
            MyModelLB = modelLB;

            var modelRB = new PlotModel { Title = "Device 4" };
            var series3 = new LineSeries { Title = "Series 4", MarkerType = MarkerType.Circle };

            // Add X and Y axes
            modelRB.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Time", StringFormat = "dd/MM/yy HH:mm", Angle = 45 });
            modelRB.Axes.Add(new CategoryAxis { Position = AxisPosition.Left, Title = "Value", ItemsSource = new[] { "False", "True" } });
            modelRB.Series.Add(series3);

            MyModelRB = modelRB;

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

        private void DeviceMgr_StatusEvent(string deviceName, bool status, DateTime time)
        {
            LineSeries? series = null;
            if (deviceName == "RaspberryPi-1")
            {
                series = MyModel.Series[0] as LineSeries;
            }
            else if (deviceName == "RaspberryPi-2")
            {
                series = MyModelRT.Series[0] as LineSeries;
            }
            else
            {
                series = MyModelRB.Series[0] as LineSeries;
                logger.Fatal($"There is no machine of name : {deviceName}");
                //return;
            }

            var statusVal = status ? 1 : 0;
            series?.Points.Add(new DataPoint(DateTimeAxis.ToDouble(time), statusVal));

            MyModel.InvalidatePlot(true);
            MyModelRT.InvalidatePlot(true);
            MyModelRB.InvalidatePlot(true);
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

        private void DummyDatainLB(PlotModel model, LineSeries series)
        {
            int date = 0;
            int month = 1;
            int year = 2024;
            var rand = new Random();

            while (true)
            {
                Thread.Sleep(5000);

                if (date == 30)
                {
                    date = 0;
                    month++;
                }

                if (month > 12)
                {
                    year++;
                }

                date++;
                var num = rand.Next(0, 2);
                series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(year, month, date)), num));
                MyModelLB.InvalidatePlot(true);
            }
        }
    }
}