using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Logging;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;

namespace GnssTracker_Demo.Controllers
{
    public class DisplayController
    {
        protected int counter = 0;
        protected Logger Log { get => Resolver.Log; }
        protected DisplayScreen DisplayScreen { get; set; }

        protected AbsoluteLayout SplashLayout { get; set; }
        protected AbsoluteLayout DataLayout { get; set; }

        protected Font12x20 LargeFont { get; set; }
        protected Font4x8 SmallFont { get; set; }

        protected Label TemperatureLabel { get; set; }
        protected Label HumidityLabel { get; set; }
        protected Label PressureLabel { get; set; }
        protected Label LatitudeLabel { get; set; }
        protected Label LongitudeLabel { get; set; }
        protected Label CounterLabel { get; set; }

        public DisplayController(IGraphicsDisplay display)
        {
            LargeFont = new Font12x20();
            SmallFont = new Font4x8();

            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees);

            SplashLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height);

            var image = Image.LoadFromResource("GnssTracker_Demo.gnss_tracker.bmp");
            var displayImage = new Picture(0, 0, 250, 122, image)
            {
                BackColor = Color.FromHex("#23ABE3"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            SplashLayout.Controls.Add(displayImage);

            DataLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height);

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
            TemperatureLabel = new Label(10, 10, DisplayScreen.Width - 20, LargeFont.Height)
            {
                Text = $"Temp:     0.00°C",
                TextColor = Color.Black,
                BackColor = Color.White,
                Font = LargeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            HumidityLabel = new Label(10, 30, DisplayScreen.Width - 20, LargeFont.Height)
            {
                Text = $"Humidity: 0.00%",
                TextColor = Color.Black,
                BackColor = Color.White,
                Font = LargeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            PressureLabel = new Label(10, 50, DisplayScreen.Width - 20, LargeFont.Height)
            {
                Text = $"Pressure: 0.00atm",
                TextColor = Color.Black,
                BackColor = Color.White,
                Font = LargeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            LatitudeLabel = new Label(10, 72, DisplayScreen.Width - 20, LargeFont.Height)
            {
                Text = $"Lat: 0°0'0.0\"",
                TextColor = Color.White,
                BackColor = Color.Red,
                Font = LargeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            LongitudeLabel = new Label(10, 92, DisplayScreen.Width - 20, LargeFont.Height)
            {
                Text = $"Lon: 0°0'0.0\"",
                TextColor = Color.White,
                BackColor = Color.Red,
                Font = LargeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            counter++;
            CounterLabel = new Label(222, 113, 20, 8)
            {
                Text = $"{counter.ToString("D4")}",
                TextColor = Color.Black,
                BackColor = Color.White,
                Font = SmallFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            DataLayout.Controls.Add(box, frame, TemperatureLabel, HumidityLabel, PressureLabel, LatitudeLabel, LongitudeLabel, CounterLabel);

            DisplayScreen.Controls.Add(SplashLayout, DataLayout);

            DataLayout.Visible = false;
        }

        public void UpdateDisplay((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) conditions, GnssPositionInfo locationInfo)
        {
            SplashLayout.Visible = false;
            DataLayout.Visible = true;

            TemperatureLabel.Text = $"Temp:     {conditions.Temperature?.Celsius:n1}°C";
            HumidityLabel.Text = $"Humidity: {conditions.Humidity?.Percent:n1}%";
            PressureLabel.Text = $"Pressure: {conditions.Pressure?.StandardAtmosphere:n2}atm";

            string lat = locationInfo == null
                ? $"Lat: 0°0'0.0\""
                : $"Lat: " +
                $"{locationInfo?.Position?.Latitude?.Degrees}°".PadLeft(4) +
                $"{locationInfo?.Position?.Latitude?.Minutes:n2}'" +
                $"{locationInfo?.Position?.Latitude?.Seconds}\"";
            LatitudeLabel.Text = lat;

            string lon = locationInfo == null
                ? $"Lon: 0°0'0.0\""
                : $"Lon: " +
                $"{locationInfo?.Position?.Longitude?.Degrees}°".PadLeft(4) +
                $"{locationInfo?.Position?.Longitude?.Minutes:n2}'" +
                $"{locationInfo?.Position?.Longitude?.Seconds}\"";
            LongitudeLabel.Text = lon;

            counter++;
            CounterLabel.Text = $"{counter.ToString("D4")}";
        }
    }
}