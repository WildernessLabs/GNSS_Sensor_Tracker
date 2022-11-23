using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;

namespace Demo_App
{
    public static class DisplayController
    {
        private static Ssd1680 ePaperDisplay { get; set; }
        private static MicroGraphics canvas { get; set; }

        public static void Initialize(Ssd1680 display)
        {
            ePaperDisplay = display;

            canvas = new MicroGraphics(display)
            {
                Rotation = RotationType._270Degrees, 
            };

            canvas.Clear(Color.White);
            canvas.CurrentFont = new Font12x16();
            canvas.DrawText(2, 20, "Hello, Meadow", Color.Red);
            canvas.Show();
        }

        public static void UpdateConditions(IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> result)
        {
            canvas.Clear(Color.White);
            canvas.CurrentFont = new Font12x16();
            canvas.DrawText(2, 18, $"Temp: {result.New.Temperature?.Celsius:N2}C", Color.Black);
            canvas.DrawText(2, 38, $"Humidity: {result.New.Humidity:N2}%", Color.Black);
            canvas.DrawText(2, 58, $"Pressure: {result.New.Pressure?.StandardAtmosphere:N2}A", Color.Black);
            canvas.Show();
        }
    }
}