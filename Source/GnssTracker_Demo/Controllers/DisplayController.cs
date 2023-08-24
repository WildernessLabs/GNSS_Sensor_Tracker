using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Logging;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;
using System;

namespace GnssTracker_Demo.Controllers
{
    public class DisplayController
    {
        protected int counter = 0;
        protected Logger Log { get => Resolver.Log; }
        protected DisplayScreen graphics { get; set; }

        protected Font12x20 largeFont { get; set; }
        protected Font4x8 smallFont { get; set; }

        protected DisplayLabel TempLabel { get; set; }
        protected DisplayLabel HumidityLabel { get; set; }
        protected DisplayLabel PressureLabel { get; set; }
        protected DisplayLabel LatitudeLabel { get; set; }
        protected DisplayLabel LongitudeLabel { get; set; }
        protected DisplayLabel CounterLabel { get; set; }

        public DisplayController(IGraphicsDisplay display)
        {
            largeFont = new Font12x20();
            smallFont = new Font4x8();

            graphics = new DisplayScreen(display, RotationType._270Degrees);

            //ShowSplashScreen();
            LoadDataScreen();
        }

        private void ShowSplashScreen()
        {
            var image = Image.LoadFromResource("GnssTracker_Demo.gnss_tracker.bmp");

            var displayImage = new DisplayImage(0, 0, 250, 122, image)
            {
                BackColor = Color.FromHex("#23ABE3"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            graphics.Controls.Add(displayImage);
        }

        private void LoadDataScreen()
        {
            try
            {
                graphics.Controls.Clear();

                var box = new DisplayBox(0, 0, graphics.Width, graphics.Height)
                {
                    ForeColor = Color.White,
                    Filled = true
                };

                var frame = new DisplayBox(5, 5, 240, 112)
                {
                    ForeColor = Color.Black,
                    Filled = false
                };

                TempLabel = new DisplayLabel(10, 10, graphics.Width - 20, largeFont.Height)
                {
                    Text = $"Temp:     0.00°C",
                    TextColor = Color.Black,
                    BackColor = Color.White,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                HumidityLabel = new DisplayLabel(10, 30, graphics.Width - 20, largeFont.Height)
                {
                    Text = $"Humidity: 0.00%",
                    TextColor = Color.Black,
                    BackColor = Color.White,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                PressureLabel = new DisplayLabel(10, 50, graphics.Width - 20, largeFont.Height)
                {
                    Text = $"Pressure: 0.00atm",
                    TextColor = Color.Black,
                    BackColor = Color.White,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                LatitudeLabel = new DisplayLabel(10, 72, graphics.Width - 20, largeFont.Height)
                {
                    Text = $"Lat: 0°0'0.0\"",
                    TextColor = Color.White,
                    BackColor = Color.Red,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                LongitudeLabel = new DisplayLabel(10, 92, graphics.Width - 20, largeFont.Height)
                {
                    Text = $"Lon: 0°0'0.0\"",
                    TextColor = Color.White,
                    BackColor = Color.Red,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                counter++;
                CounterLabel = new DisplayLabel(222, 113, 20, 8)
                {
                    Text = $"{counter.ToString("D4")}",
                    TextColor = Color.Black,
                    BackColor = Color.White,
                    Font = smallFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                graphics.Controls.Add(box, frame, TempLabel, HumidityLabel, PressureLabel, LatitudeLabel, LongitudeLabel, CounterLabel);
            }
            catch (Exception e)
            {
                Log?.Error($"err while rendering: {e.Message}");
            }
        }

        public void UpdateDisplay((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) conditions, GnssPositionInfo locationInfo)
        {
            TempLabel.Text = $"Temp:     {conditions.Temperature?.Celsius:n2}°C";
            HumidityLabel.Text = $"Humidity: {conditions.Humidity?.Percent:n2}%";
            PressureLabel.Text = $"Pressure: {conditions.Pressure?.StandardAtmosphere:n2}atm";

            string lat = locationInfo == null
                ? $"Lat: 0°0'0.0\""
                : $"Lat: " +
                $"{locationInfo?.Position?.Latitude?.Degrees}°" +
                $"{locationInfo?.Position?.Latitude?.Minutes:n2}'" +
                $"{locationInfo?.Position?.Latitude?.seconds}\"";
            LatitudeLabel.Text = lat;

            string lon = locationInfo == null
                ? $"Lon: 0°0'0.0\""
                : $"Lon: " +
                $"{locationInfo?.Position?.Longitude?.Degrees}°" +
                $"{locationInfo?.Position?.Longitude?.Minutes:n2}'" +
                $"{locationInfo?.Position?.Longitude?.seconds}\"";
            LongitudeLabel.Text = lon;

            counter++;
            CounterLabel.Text = $"{counter.ToString("D4")}";
        }
    }
}