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
        private static Logger Log { get => Resolver.Log; }
        private static MicroGraphics graphics { get; set; }
        private static bool Rendering { get; set; }
        private static AtmosphericModel? CurrentAtmosphericConditions { get; set; }
        private static LocationModel? CurrentPositionInfo { get; set; }
        private static object renderLock = new object();

        public static void Initialize(IGraphicsDisplay display)
        {
            Log.Info("Initializing DisplayController.");

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

        public static void UpdateAtmosphericConditions(AtmosphericModel conditions)
        {
            CurrentAtmosphericConditions = conditions;
            UpdateDisplay(true);
        }

        public static void UpdateGnssPositionInformation(LocationModel locationInfo)
        {
            CurrentPositionInfo = locationInfo;
            UpdateDisplay(false);
        }

        static void UpdateDisplay(bool atmosphericValues)
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
                if (atmosphericValues)
                {
                    graphics.DrawRectangle(10, 10, 230, 60, Color.White, true);
                    graphics.DrawText(10, 10, $"Temp: {CurrentAtmosphericConditions.Temperature?.Celsius:N1}°C/{CurrentAtmosphericConditions.Temperature?.Fahrenheit:N1}°F", Color.Black);
                    graphics.DrawText(10, 30, $"Humidity: {CurrentAtmosphericConditions.RelativeHumidity:N1}%", Color.Black);
                    graphics.DrawText(10, 50, $"Pressure: {CurrentAtmosphericConditions.Pressure?.StandardAtmosphere:N2}atm", Color.Black);
                }
                else
                {
                    graphics.DrawRectangle(10, 72, 230, 40, Color.White, true);
                    graphics.DrawText(10, 72, $"Lat: {CurrentPositionInfo?.PositionInformation?.Position?.Latitude?.Degrees}°{CurrentPositionInfo?.PositionInformation?.Position?.Latitude?.Minutes:n2}'{CurrentPositionInfo?.PositionInformation?.Position?.Latitude?.seconds}\"", Color.Black);
                    graphics.DrawText(10, 92, $"Long: {CurrentPositionInfo?.PositionInformation?.Position?.Longitude?.Degrees}°{CurrentPositionInfo?.PositionInformation?.Position?.Longitude?.Minutes:n2}'{CurrentPositionInfo?.PositionInformation?.Position?.Longitude?.seconds}\"", Color.Black);
                }
                
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