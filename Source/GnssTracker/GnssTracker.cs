using Meadow.Hardware;
using Meadow.Logging;
using System;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Gnss Tracker IoT acclerator
    /// </summary>
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

            II2cBus i2cBus;

            logger?.Debug("Initializing GnssTracker...");

            var device = Resolver.Device;

            if (device == null)
            {
                var msg = "GnssTracker instance must be created no earlier than App.Initialize()";
                logger?.Error(msg);
                throw new Exception(msg);
            }

            try
            {
                logger?.Debug("I2CBus Initializing...");

                i2cBus = device.CreateI2cBus();

                logger?.Debug("I2CBus initialized");
            }
            catch (Exception e)
            {
                logger?.Error($"Err initializing I2CBus: {e.Message}");
                throw;
            }

            if (device is IF7CoreComputeMeadowDevice { } ccm)
            {
                try
                {
                    logger?.Info("Instantiating GnssTracker v2 hardware");
                    hardware = new GnssTrackerHardwareV2(ccm, i2cBus);
                }
                catch
                {
                    logger?.Info("Instantiating GnssTracker v1 hardware");
                    hardware = new GnssTrackerHardwareV1(ccm, i2cBus);
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            return hardware;
        }
    }
}