using System;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.GnssTracker.Core.Models;
using Meadow.Logging;
using static SQLite.SQLite3;

namespace Demo_App
{
    public static class DisplayController
    {
        private static Logger Log { get => Resolver.Log; }
        private static Ssd1680 ePaperDisplay { get; set; }
        private static MicroGraphics canvas { get; set; }
        private static bool Rendering { get; set; }
        private static TrackingModel CurrentConditions { get; set; }


        public static void Initialize(Ssd1680 display)
        {
            Log.Info("Initializing DisplayController.");
            ePaperDisplay = display;

            canvas = new MicroGraphics(display)
            {
                Rotation = RotationType._270Degrees, 
            };

            //canvas.Clear(Color.White);
            //canvas.CurrentFont = new Font12x16();
            //canvas.DrawText(2, 20, "Hello, Meadow", Color.Red);
            //canvas.Show();

            Log.Info("DisplayController up.");
        }

        public static void UpdateConditions(
            TrackingModel conditions
            //IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> result
            )
        {
            CurrentConditions = conditions;
            UpdateDisplay();
        }

        static void UpdateDisplay()
        {
            // if already rendering, bail out
            if (Rendering) {
                Log.Info("Already rendering, bailing out.");
                return;
            }

            Rendering = true;

            Log.Info("DisplayController.UpdateConditions()");
            canvas.Clear(Color.White);
            canvas.CurrentFont = new Font12x20();
            canvas.DrawText(2, 2, $"Temp: {CurrentConditions.Temperature?.Celsius:N1}°C/{CurrentConditions.Temperature?.Fahrenheit:N1}°F", Color.Black);
            canvas.DrawText(2, 22, $"Humidity: {CurrentConditions.RelativeHumidity:N1}%", Color.Black);
            canvas.DrawText(2, 42, $"Pressure: {CurrentConditions.Pressure?.StandardAtmosphere:N2}atm", Color.Black);
            try
            {
                if (CurrentConditions.PositionCourseAndTime?.Position is { } pos)
                {
                    if (pos.Latitude is { } lat && pos.Longitude is { } lon)
                    {
                        canvas.DrawText(2, 62, $"Lat: {lat.Degrees} {lat.Minutes:n2}' {lat.seconds}\"");
                        canvas.DrawText(2, 82, $"Long: {lon.Degrees} {lon.Minutes:n2}' {lon.seconds}\"");
                    }
                }
            }
            catch (Exception ex) {
                Log.Info($"Render exception:{ex.Message}");
            }
            Log.Info("DisplayController.Update 2");
            canvas.Show();
            Log.Info("Display Updated.");

            Rendering = false;
        }
    }
}