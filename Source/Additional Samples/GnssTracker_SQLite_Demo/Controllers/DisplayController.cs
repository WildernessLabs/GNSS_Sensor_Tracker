using GnssTracker_SQLite_Demo.Models.Logical;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Logging;
using System;

namespace GnssTracker_SQLite_Demo.Controllers
{
    public class DisplayController
    {
        protected int counter = 0;
        protected Logger Log { get => Resolver.Log; }
        protected DisplayScreen DisplayScreen { get; set; }

        protected Font12x20 largeFont { get; set; }
        protected Font4x8 smallFont { get; set; }

        protected Label TempLabel { get; set; }
        protected Label HumidityLabel { get; set; }
        protected Label PressureLabel { get; set; }
        protected Label LatitudeLabel { get; set; }
        protected Label LongitudeLabel { get; set; }
        protected Label CounterLabel { get; set; }

        public DisplayController(IGraphicsDisplay display)
        {
            largeFont = new Font12x20();
            smallFont = new Font4x8();

            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees);
        }

        public void ShowSplashScreen()
        {
            var image = Image.LoadFromResource("GnssTracker_SQLite_Demo.gnss_tracker.bmp");

            var displayImage = new Picture(0, 0, 250, 122, image)
            {
                BackColor = Color.FromHex("#23ABE3"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            DisplayScreen.Controls.Add(displayImage);
        }

        public void LoadDataScreen()
        {
            try
            {
                DisplayScreen.Controls.Clear();

                var box = new Box(0, 0, DisplayScreen.Width, DisplayScreen.Height)
                {
                    ForeColor = Color.White,
                    Filled = true
                };

                var frame = new Box(5, 5, 240, 112)
                {
                    ForeColor = Color.Black,
                    Filled = false
                };

                TempLabel = new Label(10, 10, DisplayScreen.Width - 20, largeFont.Height)
                {
                    Text = $"Temp:     0.00°C",
                    TextColor = Color.Black,
                    BackColor = Color.White,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                HumidityLabel = new Label(10, 30, DisplayScreen.Width - 20, largeFont.Height)
                {
                    Text = $"Humidity: 0.00%",
                    TextColor = Color.Black,
                    BackColor = Color.White,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                PressureLabel = new Label(10, 50, DisplayScreen.Width - 20, largeFont.Height)
                {
                    Text = $"Pressure: 0.00atm",
                    TextColor = Color.Black,
                    BackColor = Color.White,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                LatitudeLabel = new Label(10, 72, DisplayScreen.Width - 20, largeFont.Height)
                {
                    Text = $"Lat: 0°0'0.0\"",
                    TextColor = Color.White,
                    BackColor = Color.Red,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                LongitudeLabel = new Label(10, 92, DisplayScreen.Width - 20, largeFont.Height)
                {
                    Text = $"Lon: 0°0'0.0\"",
                    TextColor = Color.White,
                    BackColor = Color.Red,
                    Font = largeFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                counter++;
                CounterLabel = new Label(222, 113, 20, 8)
                {
                    Text = $"{counter.ToString("D4")}",
                    TextColor = Color.Black,
                    BackColor = Color.White,
                    Font = smallFont,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                DisplayScreen.Controls.Add(box, frame, TempLabel, HumidityLabel, PressureLabel, LatitudeLabel, LongitudeLabel, CounterLabel);
            }
            catch (Exception e)
            {
                Log?.Error($"err while rendering: {e.Message}");
            }
        }

        public void UpdateDisplay(AtmosphericModel conditions, LocationModel locationInfo)
        {
            TempLabel.Text = $"Temp:     {conditions.Temperature?.Celsius:n2}°C";
            HumidityLabel.Text = $"Humidity: {conditions.RelativeHumidity?.Percent:n2}%";
            PressureLabel.Text = $"Pressure: {conditions.Pressure?.StandardAtmosphere:n2}atm";

            string lat = locationInfo.PositionInformation == null
                ? $"Lat: 0°0'0.0\""
                : $"Lat: " +
                $"{locationInfo.PositionInformation?.Position?.Latitude?.Degrees}°" +
                $"{locationInfo.PositionInformation?.Position?.Latitude?.Minutes:n2}'" +
                $"{locationInfo.PositionInformation?.Position?.Latitude?.seconds}\"";
            LatitudeLabel.Text = lat;

            string lon = locationInfo.PositionInformation == null
                ? $"Lon: 0°0'0.0\""
                : $"Lon: " +
                $"{locationInfo.PositionInformation?.Position?.Longitude?.Degrees}°" +
                $"{locationInfo.PositionInformation?.Position?.Longitude?.Minutes:n2}'" +
                $"{locationInfo.PositionInformation?.Position?.Longitude?.seconds}\"";
            LongitudeLabel.Text = lon;

            counter++;
            CounterLabel.Text = $"{counter.ToString("D4")}";
        }
    }
}