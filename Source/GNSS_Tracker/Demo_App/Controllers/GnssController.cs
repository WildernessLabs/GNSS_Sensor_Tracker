using System;
using Meadow;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Logging;
using Meadow.Peripherals.Sensors.Location.Gnss;

namespace Demo_App.Controllers
{
    /// <summary>
    /// Responsible for initializing the GPS stuff and running all things GPS
    /// </summary>
    public static class GnssController
    {
        public static event EventHandler<GnssPositionInfo> GnssPositionInfoUpdated = delegate { };

        private static Logger Log { get => Resolver.Log; }
        private static NeoM8? GnssDevice { get; set; }

        /// <summary>
        /// Last Gnss Position
        /// </summary>
        public static GnssPositionInfo? LastGnssPositionInfo { get; private set; }

        public static void Initialize(NeoM8 gnssDevice)
        {
            if (gnssDevice is { } gnss)
            {
                GnssDevice = gnss;

                GnssDevice.GllReceived += (object sender, GnssPositionInfo location) =>
                {
                    if (location.Valid)
                    {
                        Log.Debug($"GNSS Position: lat: [{location.Position.Latitude}], long: [{location.Position.Longitude}]");
                    }
                };

                GnssDevice.GsaReceived += (object sender, ActiveSatellites activeSatellites) =>
                {
                    if (activeSatellites.SatellitesUsedForFix is { } sats)
                    {
                        Log.Debug($"Number of active satellites: {sats.Length}");
                    }
                };

                GnssDevice.VtgReceived += (object sender, CourseOverGround courseAndVelocity) =>
                {
                    if (courseAndVelocity is { } cv)
                    {
                        Log.Debug($"{cv}");
                    };
                };

                GnssDevice.GsvReceived += (object sender, SatellitesInView satellites) =>
                {
                    if (satellites is { } s)
                    {
                        Log.Debug($"Satellites in view: {s.Satellites.Length}");
                    }
                };

                GnssDevice.RmcReceived += (object sender, GnssPositionInfo positionCourseAndTime) =>
                {
                    if (positionCourseAndTime.Valid)
                    {
                        LastGnssPositionInfo = positionCourseAndTime;

                        GnssPositionInfoUpdated(sender, positionCourseAndTime);
                    }
                };
            }
        }

        public static void StartUpdating()
        {
            try
            {
                if (GnssDevice is { } gnss) 
                {
                    GnssDevice.StartUpdating();
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"{ex.Message}");
            }
        }
    }
}