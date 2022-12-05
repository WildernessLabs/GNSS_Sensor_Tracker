using System;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.GnssTracker.Core.Models;
using Meadow.GnssTracker.Core.Models.Logical;
using Meadow.Logging;
using Meadow.Peripherals.Sensors.Location.Gnss;
using static SQLite.SQLite3;

namespace Demo_App.Controllers
{
    public static class DisplayController
    {
        private static Logger Log { get => Resolver.Log; }
        private static Ssd1680 ePaperDisplay { get; set; }
        private static MicroGraphics canvas { get; set; }
        private static bool Rendering { get; set; }
        private static AtmosphericModel? CurrentAtmosphericConditions { get; set; }
        private static LocationModel? CurrentPositionInfo { get; set; }
        private static object renderLock = new object();

        public static void Initialize(Ssd1680 display)
        {
            Log.Info("Initializing DisplayController.");
            ePaperDisplay = display;

            canvas = new MicroGraphics(display)
            {
                Rotation = RotationType._270Degrees,
                CurrentFont = new Font12x20(),
            };

            Log.Info("DisplayController up.");
        }

        public static void UpdateAtmosphericConditions(AtmosphericModel conditions)
        {
            CurrentAtmosphericConditions = conditions;
            UpdateDisplay();
        }

        public static void UpdateGnssPositionInformation(LocationModel locationInfo)
        {
            CurrentPositionInfo = locationInfo;
            UpdateDisplay();
        }

        static void UpdateDisplay()
        {
            Log.Info($"DisplayController.UpdateDisplay() @ {DateTime.Now.ToShortTimeString()}.");

            lock (renderLock)
            {
                // if already rendering, bail out
                if (Rendering)
                {
                    Log.Info("Already rendering, bailing out.");
                    return;
                }
                Rendering = true;
            }

            try
            {
                // clear out the display buffer
                canvas.Clear(Color.White);

                // atomospheric info
                canvas.DrawText(2, 2, $"Temp: {CurrentAtmosphericConditions.Temperature?.Celsius:N1}°C/{CurrentAtmosphericConditions.Temperature?.Fahrenheit:N1}°F", Color.Black);
                canvas.DrawText(2, 22, $"Humidity: {CurrentAtmosphericConditions.RelativeHumidity:N1}%", Color.Black);
                canvas.DrawText(2, 42, $"Pressure: {CurrentAtmosphericConditions.Pressure?.StandardAtmosphere:N2}atm", Color.Black);

                // position info
                canvas.DrawText(2, 62, $"Lat: {CurrentPositionInfo?.PositionInformation?.Position?.Latitude?.Degrees}°{CurrentPositionInfo?.PositionInformation?.Position?.Latitude?.Minutes:n2}'{CurrentPositionInfo?.PositionInformation?.Position?.Latitude?.seconds}\"");
                canvas.DrawText(2, 82, $"Long: {CurrentPositionInfo?.PositionInformation?.Position?.Longitude?.Degrees}°{CurrentPositionInfo?.PositionInformation?.Position?.Longitude?.Minutes:n2}'{CurrentPositionInfo?.PositionInformation?.Position?.Longitude?.seconds}\"");

                // blit/push to the display
                canvas.Show();
                Log.Info($"Display Updated @ {DateTime.Now.ToShortTimeString()}.");
            }
            catch (Exception e)
            {
                Log?.Info($"err while rendering: {e.Message}");
            }
            finally {
                Log?.Info($"Letting go of render lock @ {DateTime.Now.ToShortTimeString()}.");
                Rendering = false;
            }
        }
    }
}