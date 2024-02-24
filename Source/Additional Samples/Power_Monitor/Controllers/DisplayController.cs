using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System.Collections.Generic;

namespace Power_Monitor.Controllers
{
    public class DisplayController
    {
        private readonly int offsetY = 6;

        List<double> batteryVoltages;
        List<double> solarInputVoltages;

        private Color backgroundColor = Color.White;
        private Color foregroundColor = Color.Black;
        private Color highlight = Color.Red;

        private Font8x12 font8x12 = new Font8x12();

        private DisplayScreen displayScreen;
        private LineChartSeries batteryChartSeries;
        private LineChartSeries solarInputChartSeries;

        public DisplayController(IPixelDisplay display)
        {
            batteryVoltages = new List<double>();

            solarInputVoltages = new List<double>();

            displayScreen = new DisplayScreen(display, RotationType._90Degrees)
            {
                BackgroundColor = backgroundColor
            };

            displayScreen.Controls.Add(new Box(2, 2 + offsetY, displayScreen.Width - 4, displayScreen.Height - offsetY - 4)
            {
                ForeColor = foregroundColor,
                IsFilled = false
            });

            displayScreen.Controls.Add(new Label(125, 105 + offsetY, 120, 12)
            {
                Text = $"SOLAR",
                TextColor = foregroundColor,
                Font = font8x12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            displayScreen.Controls.Add(new Label(5, 105 + offsetY, 120, 12)
            {
                Text = $"BATTERY",
                TextColor = highlight,
                Font = font8x12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var lineChart = new LineChart(
                5,
                7 + offsetY,
                240,
                95)
            {
                BackgroundColor = backgroundColor,
                AxisLabelColor = foregroundColor,
                AxisColor = foregroundColor,
                ShowYAxisLabels = true,
                AlwaysShowYOrigin = false,
            };
            batteryChartSeries = new LineChartSeries()
            {
                LineColor = highlight,
                PointColor = highlight,
                LineStroke = 1,
                PointSize = 2,
                ShowLines = true,
                ShowPoints = true,
            };
            lineChart.Series.Add(batteryChartSeries);
            solarInputChartSeries = new LineChartSeries()
            {
                LineColor = foregroundColor,
                PointColor = foregroundColor,
                LineStroke = 1,
                PointSize = 2,
                ShowLines = true,
                ShowPoints = true,
            };
            lineChart.Series.Add(solarInputChartSeries);
            displayScreen.Controls.Add(lineChart);
        }

        public void UpdateGraph(double batteryVoltage, double solarVoltageInput)
        {
            if (batteryVoltages.Count == 10)
            {
                batteryVoltages.RemoveAt(0);
                solarInputVoltages.RemoveAt(0);
            }

            batteryVoltages.Add(batteryVoltage);
            solarInputVoltages.Add(solarVoltageInput);

            displayScreen.BeginUpdate();

            batteryChartSeries.Points.Clear();
            solarInputChartSeries.Points.Clear();

            for (var p = 0; p < batteryVoltages.Count; p++)
            {
                batteryChartSeries.Points.Add(p * 2, batteryVoltages[p]);
                solarInputChartSeries.Points.Add(p * 2, solarInputVoltages[p]);
            }

            displayScreen.EndUpdate();
        }
    }
}