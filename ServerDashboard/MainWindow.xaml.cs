using NLog;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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

            //PointsData(model, series);
            //DateTimeData1(model, series);
            BinaryData(model, series);
 
            model.Series.Add(series);

            MyModel = model;
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

        private void DateTimeData(PlotModel model, LineSeries series)
        {
            // Add data points (X: numeric, Y: DateTime converted to double)
            series.Points.Add(new DataPoint(0, DateTimeAxis.ToDouble(new DateTime(2024, 1, 1))));
            series.Points.Add(new DataPoint(10, DateTimeAxis.ToDouble(new DateTime(2024, 2, 1))));
            series.Points.Add(new DataPoint(20, DateTimeAxis.ToDouble(new DateTime(2024, 3, 1))));
            series.Points.Add(new DataPoint(30, DateTimeAxis.ToDouble(new DateTime(2024, 4, 1))));
            series.Points.Add(new DataPoint(40, DateTimeAxis.ToDouble(new DateTime(2024, 5, 1))));
            series.Points.Add(new DataPoint(50, DateTimeAxis.ToDouble(new DateTime(2024, 6, 1))));
            // Add X and Y axes
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X Axis" });
            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Left, Title = "Y Axis (DateTime)", StringFormat = "MMM dd, yyyy" });
        }

        private void DateTimeData1(PlotModel model, LineSeries series)
        {
            // Add data points (X: numeric, Y: DateTime converted to double)
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 1, 1)), 0));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 2, 1)), 10));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 3, 1)), 20));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 4, 1)), 30));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 5, 1)), 40));
            series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(new DateTime(2024, 6, 1)), 50));
            // Add X and Y axes
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "X Axis" });
            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Y Axis (DateTime)", StringFormat = "MMM dd, yyyy" });
        }

        private void PointsData(PlotModel model, LineSeries series)
        {
            series.Points.Add(new DataPoint(0, 0));
            series.Points.Add(new DataPoint(10, 18));
            series.Points.Add(new DataPoint(20, 12));
            series.Points.Add(new DataPoint(30, 8));
            series.Points.Add(new DataPoint(40, 15));
            series.Points.Add(new DataPoint(50, 10));
        }
    }
}