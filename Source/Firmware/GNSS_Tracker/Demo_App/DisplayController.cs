using System;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Displays.ePaper;
using Meadow.Foundation.Graphics;
using Meadow.Gateways.Bluetooth;
using Meadow.Peripherals.Displays;

namespace Demo_App
{
    public static class DisplayController
    {
        private static Epd2in13b EPaperDisplay { get; set; }
        private static MicroGraphics Canvas { get; set; }

        public static void Initialize(Epd2in13b display)
        {
            EPaperDisplay = display;

            Canvas = new MicroGraphics(display)
            {
                Rotation = RotationType._270Degrees
            };

            Canvas.Clear(Color.White);
            Canvas.CurrentFont = new Font12x16();
            Canvas.DrawText(2, 20, "Hello, Meadow", Color.Red);
            Canvas.Show();
        }

        public static void UpdateConditions(IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> result)
        {
            Canvas.Clear(Color.White);
            Canvas.CurrentFont = new Font12x16();
            Canvas.DrawText(2, 18, $"Temp: {result.New.Temperature?.Celsius:N2}C", Color.Black);
            Canvas.DrawText(2, 38, $"Humidity: {result.New.Humidity:N2}%", Color.Black);
            Canvas.DrawText(2, 58, $"Pressure: {result.New.Pressure?.StandardAtmosphere:N2}A", Color.Black);
            Canvas.Show();
        }
    }
}

