using Meadow;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Hardware;

namespace WildernessLabs.Hardware.GnssTracker
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware V1
    /// </summary>
    public class GnssTrackerHardwareV1 : GnssTrackerHardwareBase
    {
        /// <inheritdoc/>
        public override Scd40? EnvironmentalSensor { get => null; protected set => throw new System.NotImplementedException(); }

        /// <inheritdoc/>
        public override Bmi270? MotionSensor { get => null; protected set => throw new System.NotImplementedException(); }

        /// <summary>
        /// Create a new GnssTrackerHardwareV1 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV1(IF7CoreComputeMeadowDevice device, II2cBus i2cBus) : base(device, i2cBus)
        { }
    }
}