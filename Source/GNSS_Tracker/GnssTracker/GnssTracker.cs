using Meadow.Logging;
using Meadow;
using System;

namespace WildernessLabs.Hardware.GnssTracker
{
    public class GnssTracker
    {
        private GnssTracker() { }

        /// <summary>
        /// Create an instance of the GnssTracker class
        /// </summary>
        public static IGnssTrackerHardware Create()
        {
            IGnssTrackerHardware hardware;
            Logger? logger = Resolver.Log;

            logger?.Debug("Initializing GnssTracker...");

            var device = Resolver.Device;

            if (Resolver.Device == null)
            {
                var msg = "GnssTracker instance must be created no earlier than App.Initialize()";
                logger?.Error(msg);
                throw new Exception(msg);
            }

            if (device is IF7CoreComputeMeadowDevice { } ccm)
            {
                logger?.Info("Instantiating GnssTracker v1 hardware");
                hardware = new GnssTrackerHardwareV1(ccm);
            }
            else
            {
                throw new NotSupportedException();
            }

            return hardware;
        }
    }
}