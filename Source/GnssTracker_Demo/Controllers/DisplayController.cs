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
        private readonly int marginX = 8;
        private readonly int offsetY = 5;

        private readonly Font8x12 largeFont = new Font8x12();

        private readonly DisplayScreen displayScreen;
        private readonly AbsoluteLayout dataLayout;

        private readonly Label solarVoltageLabel;
        private readonly Label temperatureLabel;
        private readonly Label humidityLabel;
        private readonly Label pressureLabel;
        private readonly Label co2LevelsLabel;
        private readonly Label latitudeLabel;
        private readonly Label longitudeLabel;
        private readonly Label batteryVoltageLabel;

        public DisplayController(IPixelDisplay display)
        {
            displayScreen = new DisplayScreen(display, RotationType._90Degrees);

            displayScreen.BeginUpdate();

            dataLayout = new AbsoluteLayout(displayScreen, 0, 0, displayScreen.Width, displayScreen.Height)
            {
                BackgroundColor = Color.White
            };

            dataLayout.Controls.Add(new Box(0, 0 + offsetY, displayScreen.Width, 15)
            {
                ForeColor = Color.Red,
                IsFilled = true
            });

            dataLayout.Controls.Add(new Label(marginX, 3 + offsetY, displayScreen.Width, largeFont.Height)
            {
                Text = $"BATTERY VOLTAGE:",
                TextColor = Color.White,
                Font = largeFont
            });
            batteryVoltageLabel = new Label(0, 3 + offsetY, displayScreen.Width - marginX, largeFont.Height)
            {
                Text = $"0.00  V",
                TextColor = Color.White,
                Font = largeFont,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            dataLayout.Controls.Add(batteryVoltageLabel);

            dataLayout.Controls.Add(new Label(marginX, 18 + offsetY, displayScreen.Width, largeFont.Height)
            {
                Text = $"SOLAR VOLTAGE:",
                TextColor = Color.Black,
                Font = largeFont
            });
            solarVoltageLabel = new Label(0, 18 + offsetY, displayScreen.Width - marginX, largeFont.Height)
            {
                Text = $"0.00  V",
                TextColor = Color.Black,
                Font = largeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(solarVoltageLabel);

            dataLayout.Controls.Add(new Label(marginX, 33 + offsetY, displayScreen.Width / 2, largeFont.Height)
            {
                Text = $"TEMPERATURE:",
                TextColor = Color.Black,
                Font = largeFont
            });
            temperatureLabel = new Label(0, 33 + offsetY, displayScreen.Width - marginX, largeFont.Height)
            {
                Text = $"0.0   C",
                TextColor = Color.Black,
                Font = largeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(temperatureLabel);

            dataLayout.Controls.Add(new Label(marginX, 48 + offsetY, displayScreen.Width, largeFont.Height)
            {
                Text = $"HUMIDITY:",
                TextColor = Color.Black,
                Font = largeFont
            });
            humidityLabel = new Label(0, 48 + offsetY, displayScreen.Width - marginX, largeFont.Height)
            {
                Text = $"0.0   %",
                TextColor = Color.Black,
                Font = largeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(humidityLabel);

            dataLayout.Controls.Add(new Label(marginX, 63 + offsetY, displayScreen.Width, largeFont.Height)
            {
                Text = $"PRESSURE:",
                TextColor = Color.Black,
                Font = largeFont
            });
            pressureLabel = new Label(0, 63 + offsetY, displayScreen.Width - marginX, largeFont.Height)
            {
                Text = $"0.0 ATM",
                TextColor = Color.Black,
                Font = largeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(pressureLabel);

            dataLayout.Controls.Add(new Label(marginX, 78 + offsetY, displayScreen.Width, largeFont.Height)
            {
                Text = $"CO2 LEVELS:",
                TextColor = Color.Black,
                Font = largeFont
            });
            co2LevelsLabel = new Label(0, 78 + offsetY, displayScreen.Width - marginX, largeFont.Height)
            {
                Text = $"0.0 PPM",
                TextColor = Color.Black,
                Font = largeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(co2LevelsLabel);

            dataLayout.Controls.Add(new Box(0, 90 + offsetY, displayScreen.Width, 32)
            {
                ForeColor = Color.Red,
                IsFilled = true
            });

            dataLayout.Controls.Add(new Label(marginX, 94 + offsetY, displayScreen.Width, largeFont.Height)
            {
                Text = $"LATITUDE:",
                TextColor = Color.White,
                Font = largeFont
            });
            latitudeLabel = new Label(0, 94 + offsetY, displayScreen.Width - marginX, largeFont.Height)
            {
                Text = $"00 00' 0.0\"",
                TextColor = Color.White,
                Font = largeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(latitudeLabel);

            dataLayout.Controls.Add(new Label(marginX, 109 + offsetY, displayScreen.Width, largeFont.Height)
            {
                Text = $"LONGITUDE:",
                TextColor = Color.White,
                Font = largeFont
            });
            longitudeLabel = new Label(0, 109 + offsetY, displayScreen.Width - marginX, largeFont.Height)
            {
                Text = $"00 00' 0.0\"",
                TextColor = Color.White,
                Font = largeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            dataLayout.Controls.Add(longitudeLabel);

            displayScreen.Controls.Add(dataLayout);

            displayScreen.EndUpdate();
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
            displayScreen.BeginUpdate();

            batteryVoltageLabel.Text = $"{BatteryVoltage?.Volts:N2}   V";
            solarVoltageLabel.Text = $"{SolarVoltage?.Volts:N2}   V";
            temperatureLabel.Text = $"{Temperature?.Celsius:N1}   C";
            humidityLabel.Text = $"{Humidity?.Percent:N1}   %";
            pressureLabel.Text = $"{Pressure?.StandardAtmosphere:N1} ATM";

            if (Concentration != null)
            {
                co2LevelsLabel.Text = $"{Concentration?.PartsPerMillion:N1} PPM";
            }

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

            displayScreen.EndUpdate();
        }
    }
}