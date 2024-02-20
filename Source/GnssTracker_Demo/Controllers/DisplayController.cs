using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;

namespace GnssTracker_Demo.Controllers
{
    public class DisplayController
    {
        private int margin_x = 8;
        private int offset_y = 5;

        private Font8x12 LargeFont = new Font8x12();
        private Font6x8 SmallFont = new Font6x8();

        private DisplayScreen displayScreen;
        private AbsoluteLayout dataLayout;

        private Label batteryVoltageLabel;
        private Label solarVoltageLabel;
        private Label temperatureLabel;
        private Label humidityLabel;
        private Label pressureLabel;
        private Label co2LevelsLabel;
        private Label latitudeLabel;
        private Label longitudeLabel;

        public DisplayController(IPixelDisplay display)
        {
            displayScreen = new DisplayScreen(display, RotationType._90Degrees);

            dataLayout = new AbsoluteLayout(displayScreen, 0, 0, displayScreen.Width, displayScreen.Height)
            {
                BackgroundColor = Color.White
            };

            dataLayout.Controls.Add(new Box(0, 0 + offset_y, displayScreen.Width, 15)
            {
                ForeColor = Color.Red,
                IsFilled = true
            });

            dataLayout.Controls.Add(new Label(margin_x, 3 + offset_y, displayScreen.Width, LargeFont.Height)
            {
                Text = $"BATTERY VOLTAGE:",
                TextColor = Color.White,
                Font = LargeFont
            });
            batteryVoltageLabel = new Label(0, 3 + offset_y, displayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"0.00  V",
                TextColor = Color.White,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            dataLayout.Controls.Add(batteryVoltageLabel);

            dataLayout.Controls.Add(new Label(margin_x, 18 + offset_y, displayScreen.Width, LargeFont.Height)
            {
                Text = $"SOLAR VOLTAGE:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            solarVoltageLabel = new Label(0, 18 + offset_y, displayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"0.00  V",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(solarVoltageLabel);

            dataLayout.Controls.Add(new Label(margin_x, 33 + offset_y, displayScreen.Width / 2, LargeFont.Height)
            {
                Text = $"TEMPERATURE:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            temperatureLabel = new Label(0, 33 + offset_y, displayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"0.0   C",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(temperatureLabel);

            dataLayout.Controls.Add(new Label(margin_x, 48 + offset_y, displayScreen.Width, LargeFont.Height)
            {
                Text = $"HUMIDITY:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            humidityLabel = new Label(0, 48 + offset_y, displayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"0.0   %",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(humidityLabel);

            dataLayout.Controls.Add(new Label(margin_x, 63 + offset_y, displayScreen.Width, LargeFont.Height)
            {
                Text = $"PRESSURE:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            pressureLabel = new Label(0, 63 + offset_y, displayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"0.0 ATM",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(pressureLabel);

            dataLayout.Controls.Add(new Label(margin_x, 78 + offset_y, displayScreen.Width, LargeFont.Height)
            {
                Text = $"CO2 LEVELS:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            co2LevelsLabel = new Label(0, 78 + offset_y, displayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"0.0 PPM",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(co2LevelsLabel);

            dataLayout.Controls.Add(new Box(0, 90 + offset_y, displayScreen.Width, 32)
            {
                ForeColor = Color.Red,
                IsFilled = true
            });

            dataLayout.Controls.Add(new Label(margin_x, 94 + offset_y, displayScreen.Width, LargeFont.Height)
            {
                Text = $"LATITUDE:",
                TextColor = Color.White,
                Font = LargeFont
            });
            latitudeLabel = new Label(0, 94 + offset_y, displayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"00 00' 0.0\"",
                TextColor = Color.White,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(latitudeLabel);

            dataLayout.Controls.Add(new Label(margin_x, 109 + offset_y, displayScreen.Width, LargeFont.Height)
            {
                Text = $"LONGITUDE:",
                TextColor = Color.White,
                Font = LargeFont
            });
            longitudeLabel = new Label(0, 109 + offset_y, displayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"00 00' 0.0\"",
                TextColor = Color.White,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(longitudeLabel);

            displayScreen.Controls.Add(dataLayout);
        }

        public void UpdateDisplay(
            Voltage? BatteryVoltage,
            Voltage? SolarVoltage,
            Temperature? Temperature,
            RelativeHumidity? Humidity,
            Pressure? Pressure,
            Concentration? Concentration,
            GnssPositionInfo locationInfo)
        {
            batteryVoltageLabel.Text = $"{BatteryVoltage?.Volts:N2}   V";
            solarVoltageLabel.Text = $"{SolarVoltage?.Volts:N2}   V";
            temperatureLabel.Text = $"{Temperature?.Celsius:N1}   C";
            humidityLabel.Text = $"{Humidity?.Percent:N1}   %";
            pressureLabel.Text = $"{Pressure?.StandardAtmosphere:N1} ATM";
            co2LevelsLabel.Text = $"{Concentration?.PartsPerMillion:N1} PPM";

            string lat = locationInfo == null
                ? $"00 00' 0.00\""
                : $"" +
                $"{locationInfo?.Position?.Latitude?.Degrees:N2} " +
                $"{locationInfo?.Position?.Latitude?.Minutes:N2}'" +
                $"{locationInfo?.Position?.Latitude?.Seconds:N2}\"";
            latitudeLabel.Text = lat;

            string lon = locationInfo == null
                ? $"00 00' 0.00\""
                : $"" +
                $"{locationInfo?.Position?.Longitude?.Degrees:N2} " +
                $"{locationInfo?.Position?.Longitude?.Minutes:N2}'" +
                $"{locationInfo?.Position?.Longitude?.Seconds:N2}\"";
            longitudeLabel.Text = lon;
        }
    }
}