using Meadow;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Peripherals.Sensors.Location.Gnss;
using System;

namespace GnssTracker_SQLite_Demo.Controllers
{
    public static class GnssController
    {
        public static event EventHandler<GnssPositionInfo> GnssPositionInfoUpdated = delegate { };

        private static NeoM8? GnssDevice { get; set; }

        public static GnssPositionInfo? LastGnssPositionInfo { get; private set; }

        public static void Initialize(NeoM8 gnssDevice)
        {
            if (gnssDevice is { } gnss)
            {
                GnssDevice = gnss;

                //GnssDevice.GllReceived += (object sender, GnssPositionInfo location) =>
                //{
                //    if (location.Valid)
                //    {
                //        Log.Debug($"GNSS   - Position:    LAT: [{location.Position.Latitude}], LON: [{location.Position.Longitude}]");
                //    }
                //};

                //GnssDevice.GsaReceived += (object sender, ActiveSatellites activeSatellites) =>
                //{
                //    if (activeSatellites.SatellitesUsedForFix is { } sats)
                //    {
                //        Log.Debug($"GNSS   - Number of active satellites: {sats.Length}");
                //    }
                //};

                //GnssDevice.VtgReceived += (object sender, CourseOverGround courseAndVelocity) =>
                //{
                //    if (courseAndVelocity is { } cv)
                //    {
                //        Log.Debug($"GNSS   - {cv}");
                //    };
                //};

                //GnssDevice.GsvReceived += (object sender, SatellitesInView satellites) =>
                //{
                //    if (satellites is { } s)
                //    {
                //        Log.Debug($"GNSS   - Satellites in view: {s.Satellites.Length}");
                //    }
                //};

                GnssDevice.RmcReceived += (object sender, GnssPositionInfo positionCourseAndTime) =>
                {
                    if (positionCourseAndTime.Valid)
                    {
                        Resolver.Log.Debug($"GNSS   - Position:    LAT: [{positionCourseAndTime.Position.Latitude}], LON: [{positionCourseAndTime.Position.Longitude}]");

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