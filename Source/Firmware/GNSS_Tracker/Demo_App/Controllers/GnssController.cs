using System;
using Meadow;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.GnssTracker.Core.Models;
using Meadow.Logging;
using Meadow.Peripherals.Sensors.Location.Gnss;

namespace Demo_App.Controllers
{
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

                Resolver.Log.Debug("Starting GNSS");

                // GLL
                GnssDevice.GllReceived += (object sender, GnssPositionInfo location) =>
                {
                    Log.Info($"GNSS Position: lat: [{location.Position.Latitude}], long: [{location.Position.Longitude}]");
                };
                // GSA
                GnssDevice.GsaReceived += (object sender, ActiveSatellites activeSatellites) =>
                {
                    if (activeSatellites.SatellitesUsedForFix is { } sats)
                    {
                        Log.Info($"Number of active satellites: {sats.Length}");
                    }
                };
                // RMC (recommended minimum)
                GnssDevice.RmcReceived += Gnss_RecMinimumReceived;

                // VTG (course made good)
                GnssDevice.VtgReceived += (object sender, CourseOverGround courseAndVelocity) =>
                {
                    //Log.Info("*********************************************");
                    //Log.Info($"{courseAndVelocity}");
                    //Log.Info("*********************************************");
                };
                // GSV (satellites in view)
                GnssDevice.GsvReceived += (object sender, SatellitesInView satellites) =>
                {
                    //Log.Info($"Satellites in view: {satellites.Satellites.Length}");
                };

            }
        }

        static void Gnss_RecMinimumReceived(object sender, GnssPositionInfo positionCourseAndTime)
        {
            // update the property
            LastGnssPositionInfo = positionCourseAndTime;

            // raise the event TODO: should be IChangeResult
            GnssPositionInfoUpdated(sender, positionCourseAndTime);
        }

        public static void StartUpdating()
        {
            Log?.Info("GnssController.StartUpdating()");

            //---- start updating the GNSS receiver
            try
            {
                if (GnssDevice is { } gnss) {
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

