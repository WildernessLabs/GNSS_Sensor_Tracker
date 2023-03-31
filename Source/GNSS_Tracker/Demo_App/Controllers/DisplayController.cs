using System;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.GnssTracker.Core.Models.Logical;
using Meadow.Logging;

namespace Demo_App.Controllers
{
    public static class DisplayController
    {
        private static int counter = 0;
        private static Logger Log { get => Resolver.Log; }
        private static MicroGraphics graphics { get; set; }

        private static Font12x20 largeFont { get; set; }

        private static Font4x8 smallFont { get; set; }

        private static bool Rendering { get; set; }

        private static object renderLock = new object();

        public static void Initialize(IGraphicsDisplay display)
        {
            Log.Info("Initializing DisplayController.");

            largeFont = new Font12x20();
            smallFont = new Font4x8();

            graphics = new MicroGraphics(display)
            {
                Rotation = RotationType._270Degrees,
                CurrentFont = new Font12x20(),
                Stroke = 1
            };

            Log.Info("DisplayController up.");

            DrawBackground();
        }

        static void DrawBackground() 
        {
            graphics.Clear(Color.White);

            graphics.DrawRectangle(5, 5, 240, 112, Color.Black);

            graphics.DrawText(10, 10, $"Temp:", Color.Black);
            graphics.DrawText(10, 30, $"Humidity:", Color.Black);
            graphics.DrawText(10, 50, $"Pressure:", Color.Black);
            graphics.DrawText(10, 72, $"Lat:", Color.Black);
            graphics.DrawText(10, 92, $"Long:", Color.Black);

            graphics.Show();
        }

        public static void UpdateDisplay(AtmosphericModel conditions, LocationModel locationInfo)
        {
            lock (renderLock)
            {
                if (Rendering)
                {
                    return;
                }
                Rendering = true;
            }

            try
            {
                graphics.CurrentFont = largeFont;

                graphics.DrawRectangle(10, 10, 230, 60, Color.White, true);
                // graphics.DrawText(10, 10, $"Temp: {conditions.Temperature?.Fahrenheit:N1}°F", Color.Black);
                graphics.DrawText(10, 10, $"Temperature: {conditions.Temperature?.Celsius:N1}°C", Color.Black);
                graphics.DrawText(10, 30, $"Humidity: {conditions.RelativeHumidity:N1}%", Color.Black);
                graphics.DrawText(10, 50, $"Pressure: {conditions.Pressure?.StandardAtmosphere:N2}atm", Color.Black);
                
                graphics.DrawRectangle(10, 72, 230, 40, Color.White, true);
                graphics.DrawText(10, 72, $"Latd: {locationInfo?.PositionInformation?.Position?.Latitude?.Degrees}°{locationInfo?.PositionInformation?.Position?.Latitude?.Minutes:n2}'{locationInfo?.PositionInformation?.Position?.Latitude?.seconds}\"", Color.Black);
                graphics.DrawText(10, 92, $"Long: {locationInfo?.PositionInformation?.Position?.Longitude?.Degrees}°{locationInfo?.PositionInformation?.Position?.Longitude?.Minutes:n2}'{locationInfo?.PositionInformation?.Position?.Longitude?.seconds}\"", Color.Black);

                counter++;

                graphics.CurrentFont = smallFont;
                graphics.DrawRectangle(222, 113, 20, 8, Color.White, true);
                graphics.DrawText(224, 113, $"{counter.ToString("D4")}", Color.Black);

                graphics.Show();
            }
            catch (Exception e)
            {
                Log?.Info($"err while rendering: {e.Message}");
            }
            finally 
            {
                Rendering = false;
            }
        }
    }
}