using GnssTracker_Demo.Models.Logical;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Logging;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;

namespace GnssTracker_Demo.Controllers
{
    public static class DisplayController
    {
        private static int counter = 0;
        private static Logger Log { get => Resolver.Log; }
        private static MicroGraphics graphics { get; set; }

        private static Font12x20 largeFont { get; set; }
        private static Font4x8 smallFont { get; set; }

        private static bool rendering { get; set; }
        private static object renderLock = new object();

        public static void Initialize(IGraphicsDisplay display)
        {
            largeFont = new Font12x20();
            smallFont = new Font4x8();

            graphics = new MicroGraphics(display)
            {
                Rotation = RotationType._270Degrees,
                Stroke = 1
            };

            ShowSplashScreen();
        }

        private static void ShowSplashScreen()
        {
            graphics.Clear(Color.White);

            DrawJPG(0, 0, "gnss_tracker.jpg");
            graphics.DrawRectangle(5, 5, 240, 112, Color.Black);

            graphics.Show();
        }

        private static byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"GnssTracker_Demo.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        private static void DrawJPG(int x, int y, string filename)
        {
            var jpgData = LoadResource(filename);
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int x_offset = 0;
            int y_offset = 0;
            byte r, g, b;

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                graphics.DrawPixel(x + x_offset, y + y_offset, Color.FromRgb(r, g, b).Brightness < 0.85);

                x_offset++;
                if (x_offset % decoder.Width == 0)
                {
                    y_offset++;
                    x_offset = 0;
                }
            }
        }

        public static void UpdateDisplay(AtmosphericModel conditions, LocationModel locationInfo)
        {
            lock (renderLock)
            {
                if (rendering)
                {
                    return;
                }
                rendering = true;
            }

            try
            {
                graphics.CurrentFont = largeFont;

                graphics.DrawRectangle(10, 10, 230, 102, Color.White, true);
                // graphics.DrawText(10, 10, $"Temp: {conditions.Temperature?.Fahrenheit:N1}°F", Color.Black);
                graphics.DrawText(10, 10, $"Temp:     {conditions.Temperature?.Celsius:N1}°C", Color.Black);
                graphics.DrawText(10, 30, $"Humidity: {conditions.RelativeHumidity:N1}%", Color.Black);
                graphics.DrawText(10, 50, $"Pressure: {conditions.Pressure?.StandardAtmosphere:N2}atm", Color.Black);

                string latitude = locationInfo.PositionInformation == null
                    ? $"Lat: 19°42'39.0\""
                    : $"Lat: " +
                    $"{locationInfo?.PositionInformation?.Position?.Latitude?.Degrees}°" +
                    $"{locationInfo?.PositionInformation?.Position?.Latitude?.Minutes:n2}'" +
                    $"{locationInfo?.PositionInformation?.Position?.Latitude?.seconds}\"";

                graphics.DrawText(10, 72, latitude, Color.Black);

                string longitud = locationInfo.PositionInformation == null
                    ? $"Lon: 173°45'47.9\""
                    : $"Lon: " +
                    $"{locationInfo?.PositionInformation?.Position?.Longitude?.Degrees}°" +
                    $"{locationInfo?.PositionInformation?.Position?.Longitude?.Minutes:n2}'" +
                    $"{locationInfo?.PositionInformation?.Position?.Longitude?.seconds}\"";

                graphics.DrawText(10, 92, longitud, Color.Black);

                counter++;

                graphics.CurrentFont = smallFont;
                graphics.DrawRectangle(222, 113, 20, 8, Color.White, true);
                graphics.DrawText(224, 113, $"{counter.ToString("D4")}", Color.Black);

                graphics.Show();
            }
            catch (Exception e)
            {
                Log?.Error($"err while rendering: {e.Message}");
            }
            finally
            {
                rendering = false;
            }
        }
    }
}