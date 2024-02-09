using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Logging;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;

namespace GnssTracker_Demo.Controllers
{
    public class DisplayController
    {
        int margin_x = 8;
        int offset_y = 5;

        protected int counter = 0;
        protected Logger Log { get => Resolver.Log; }
        protected DisplayScreen DisplayScreen { get; set; }

        protected AbsoluteLayout DataLayout { get; set; }

        protected Font8x12 LargeFont { get; set; } = new Font8x12();
        protected Font6x8 SmallFont { get; set; } = new Font6x8();

        protected Label DateLabel { get; set; }
        protected Label VoltageLabel { get; set; }
        protected Label TemperatureLabel { get; set; }
        protected Label HumidityLabel { get; set; }
        protected Label PressureLabel { get; set; }
        protected Label CO2LevelsLabel { get; set; }
        protected Label LatitudeLabel { get; set; }
        protected Label LongitudeLabel { get; set; }
        protected Label CounterLabel { get; set; }

        public DisplayController(IPixelDisplay display)
        {
            DisplayScreen = new DisplayScreen(display, RotationType._90Degrees);

            DataLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
            {
                BackgroundColor = Color.White
            };

            DataLayout.Controls.Add(new Box(0, 0 + offset_y, DisplayScreen.Width, 15)
            {
                ForeColor = Color.Red,
                IsFilled = true
            });

            DateLabel = new Label(margin_x, 3 + offset_y, DisplayScreen.Width, LargeFont.Height)
            {
                Text = $"--/--/----",
                TextColor = Color.White,
                Font = LargeFont
            };
            DataLayout.Controls.Add(DateLabel);

            DataLayout.Controls.Add(new Label(margin_x, 18 + offset_y, DisplayScreen.Width, LargeFont.Height)
            {
                Text = $"BATTERY/SOLAR:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            VoltageLabel = new Label(0, 18 + offset_y, DisplayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"-.-V/-.-V",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            DataLayout.Controls.Add(VoltageLabel);

            DataLayout.Controls.Add(new Label(margin_x, 33 + offset_y, DisplayScreen.Width / 2, LargeFont.Height)
            {
                Text = $"TEMPERATURE:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            TemperatureLabel = new Label(0, 33 + offset_y, DisplayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"--.-  °C",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            DataLayout.Controls.Add(TemperatureLabel);

            DataLayout.Controls.Add(new Label(margin_x, 48 + offset_y, DisplayScreen.Width, LargeFont.Height)
            {
                Text = $"HUMIDITY:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            HumidityLabel = new Label(0, 48 + offset_y, DisplayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"--.-   %",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            DataLayout.Controls.Add(HumidityLabel);

            DataLayout.Controls.Add(new Label(margin_x, 63 + offset_y, DisplayScreen.Width, LargeFont.Height)
            {
                Text = $"PRESSURE:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            PressureLabel = new Label(0, 63 + offset_y, DisplayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"-.-- ATM",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            DataLayout.Controls.Add(PressureLabel);

            DataLayout.Controls.Add(new Label(margin_x, 78 + offset_y, DisplayScreen.Width, LargeFont.Height)
            {
                Text = $"CO2 LEVELS:",
                TextColor = Color.Black,
                Font = LargeFont
            });
            CO2LevelsLabel = new Label(0, 78 + offset_y, DisplayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"--.- PPM",
                TextColor = Color.Black,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            DataLayout.Controls.Add(CO2LevelsLabel);

            DataLayout.Controls.Add(new Box(0, 90 + offset_y, DisplayScreen.Width, 32)
            {
                ForeColor = Color.Red,
                IsFilled = true
            });

            DataLayout.Controls.Add(new Label(margin_x, 94 + offset_y, DisplayScreen.Width, LargeFont.Height)
            {
                Text = $"LATITUDE:",
                TextColor = Color.White,
                Font = LargeFont
            });
            LatitudeLabel = new Label(0, 94 + offset_y, DisplayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"-- --' --.-\"",
                TextColor = Color.White,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            DataLayout.Controls.Add(LatitudeLabel);

            DataLayout.Controls.Add(new Label(margin_x, 109 + offset_y, DisplayScreen.Width, LargeFont.Height)
            {
                Text = $"LONGITUDE:",
                TextColor = Color.White,
                Font = LargeFont
            });
            LongitudeLabel = new Label(0, 109 + offset_y, DisplayScreen.Width - margin_x, LargeFont.Height)
            {
                Text = $"-- --' --.-\"",
                TextColor = Color.White,
                Font = LargeFont,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            DataLayout.Controls.Add(LongitudeLabel);

            DrawBatteryIndicator();

            DisplayScreen.Controls.Add(DataLayout);
        }

        void DrawBatteryIndicator()
        {
            DataLayout.Controls.Add(new Box(232, 2 + offset_y, 14, 11)
            {
                ForeColor = Color.White,
                IsFilled = false
            });
            DataLayout.Controls.Add(new Box(246, 4 + offset_y, 1, 7)
            {
                ForeColor = Color.White,
                IsFilled = true
            });
            DataLayout.Controls.Add(new Box(234, 4 + offset_y, 10, 7)
            {
                ForeColor = Color.White,
                IsFilled = true
            });
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
            VoltageLabel.Text = $"{BatteryVoltage?.Volts:n1}V/{SolarVoltage?.Volts:n1}V";
            TemperatureLabel.Text = $"{Temperature?.Celsius:n1}  °C";
            HumidityLabel.Text = $"{Humidity?.Percent:n1}   %";
            PressureLabel.Text = $"{Pressure?.StandardAtmosphere:n2} ATM";
            CO2LevelsLabel.Text = $"{Concentration?.PartsPerMillion:n1} PPM";

            string lat = locationInfo == null
                ? $"-- --' --.-\""
                : $"" +
                $"{locationInfo?.Position?.Latitude?.Degrees}°".PadLeft(4) +
                $"{locationInfo?.Position?.Latitude?.Minutes:n2}'" +
                $"{locationInfo?.Position?.Latitude?.Seconds}\"";
            LatitudeLabel.Text = lat;

            string lon = locationInfo == null
                ? $"-- --' --.-\""
                : $"" +
                $"{locationInfo?.Position?.Longitude?.Degrees}°".PadLeft(4) +
                $"{locationInfo?.Position?.Longitude?.Minutes:n2}'" +
                $"{locationInfo?.Position?.Longitude?.Seconds}\"";
            LongitudeLabel.Text = lon;
        }
    }
}