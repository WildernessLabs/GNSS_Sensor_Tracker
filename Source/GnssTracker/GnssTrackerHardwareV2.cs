using Meadow;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Hardware;
using System;

namespace WildernessLabs.Hardware.GnssTracker
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware V2
    /// </summary>
    public class GnssTrackerHardwareV2 : GnssTrackerHardwareBase
    {
        /// <inheritdoc/>
        public override Scd40? EnvironmentalSensor { get; protected set; }

        /// <inheritdoc/>
        public override Bmi270? MotionSensor { get; protected set; }

        /// <summary>
        /// Create a new GnssTrackerHardwareV2 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV2(IF7CoreComputeMeadowDevice device, II2cBus i2cBus) : base(device, i2cBus)
        {
            try
            {
                Log?.Trace("Initializing BMI270");
                MotionSensor = new Bmi270(i2cBus);
                Log?.Trace("BMI270 Initialized");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the BMI270 IMU: {ex.Message}");
            }

            try
            {
                Log?.Trace("Initializing SCD40");
                EnvironmentalSensor = new Scd40(i2cBus);
                Log?.Trace("SCD40 Initialized");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create the SCD40 IMU: {ex.Message}");
            }
        }
    }
}