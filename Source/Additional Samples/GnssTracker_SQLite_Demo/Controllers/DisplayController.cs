using GnssTracker_SQLite_Demo.Models.Logical;
using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;

namespace GnssTracker_SQLite_Demo.Controllers
{
    public class DisplayController
    {
        private int counter = 0;

        private DisplayScreen displayScreen;

        private AbsoluteLayout splashLayout;
        private AbsoluteLayout dataLayout;

        private Font12x20 largeFont;
        private Font4x8 smallFont;

        private Label temperatureLabel;
        private Label humidityLabel;
        private Label pressureLabel;
        private Label latitudeLabel;
        private Label longitudeLabel;
        private Label counterLabel;

        public DisplayController(IPixelDisplay display)
        {
            largeFont = new Font12x20();
            smallFont = new Font4x8();

            displayScreen = new DisplayScreen(display, RotationType._270Degrees);

            splashLayout = new AbsoluteLayout(displayScreen, 0, 0, displayScreen.Width, displayScreen.Height);

            var image = Image.LoadFromResource("GnssTracker_SQLite_Demo.gnss_tracker.bmp");
            var displayImage = new Picture(0, 0, 250, 122, image)
            {
                BackColor = Color.FromHex("#23ABE3"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            splashLayout.Controls.Add(displayImage);

            dataLayout = new AbsoluteLayout(displayScreen, 0, 0, displayScreen.Width, displayScreen.Height);

            var box = new Box(0, 0, displayScreen.Width, displayScreen.Height)
            {
                ForeColor = Color.White,
                IsFilled = true
            };
            var frame = new Box(5, 5, 240, 112)
            {
                ForeColor = Color.Black,
                IsFilled = false
            };
            temperatureLabel = new Label(10, 10, displayScreen.Width - 20, largeFont.Height)
            {
                Text = $"Temp:     0.00°C",
                TextColor = Color.Black,
                BackColor = Color.White,
                Font = largeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            humidityLabel = new Label(10, 30, displayScreen.Width - 20, largeFont.Height)
            {
                Text = $"Humidity: 0.00%",
                TextColor = Color.Black,
                BackColor = Color.White,
                Font = largeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            pressureLabel = new Label(10, 50, displayScreen.Width - 20, largeFont.Height)
            {
                Text = $"Pressure: 0.00atm",
                TextColor = Color.Black,
                BackColor = Color.White,
                Font = largeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            latitudeLabel = new Label(10, 72, displayScreen.Width - 20, largeFont.Height)
            {
                Text = $"Lat: 0°0'0.0\"",
                TextColor = Color.White,
                BackColor = Color.Red,
                Font = largeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            longitudeLabel = new Label(10, 92, displayScreen.Width - 20, largeFont.Height)
            {
                Text = $"Lon: 0°0'0.0\"",
                TextColor = Color.White,
                BackColor = Color.Red,
                Font = largeFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            counter++;
            counterLabel = new Label(222, 113, 20, 8)
            {
                Text = $"{counter.ToString("D4")}",
                TextColor = Color.Black,
                BackColor = Color.White,
                Font = smallFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            dataLayout.Controls.Add(box, frame, temperatureLabel, humidityLabel, pressureLabel, latitudeLabel, longitudeLabel, counterLabel);

            displayScreen.Controls.Add(splashLayout, dataLayout);

            dataLayout.IsVisible = false;
        }

        public void UpdateDisplay(AtmosphericModel conditions, LocationModel locationInfo)
        {
            splashLayout.IsVisible = false;
            dataLayout.IsVisible = true;

            temperatureLabel.Text = $"Temp:     {conditions.Temperature?.Celsius:n2}°C";
            humidityLabel.Text = $"Humidity: {conditions.RelativeHumidity?.Percent:n2}%";
            pressureLabel.Text = $"Pressure: {conditions.Pressure?.StandardAtmosphere:n2}atm";

            string lat = locationInfo.PositionInformation == null
                ? $"Lat: 0°0'0.0\""
                : $"Lat: " +
                $"{locationInfo.PositionInformation?.Position?.Latitude?.Degrees}°" +
                $"{locationInfo.PositionInformation?.Position?.Latitude?.Minutes:n2}'" +
                $"{locationInfo.PositionInformation?.Position?.Latitude?.Seconds}\"";
            latitudeLabel.Text = lat;

            string lon = locationInfo.PositionInformation == null
                ? $"Lon: 0°0'0.0\""
                : $"Lon: " +
                $"{locationInfo.PositionInformation?.Position?.Longitude?.Degrees}°" +
                $"{locationInfo.PositionInformation?.Position?.Longitude?.Minutes:n2}'" +
                $"{locationInfo.PositionInformation?.Position?.Longitude?.Seconds}\"";
            longitudeLabel.Text = lon;

            counter++;
            counterLabel.Text = $"{counter.ToString("D4")}";
        }
    }
}