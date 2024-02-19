using Meadow.Foundation.Sensors.Environmental;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Motion;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware V1
    /// </summary>
    public class GnssTrackerHardwareV1 : GnssTrackerHardwareBase
    {
        /// <inheritdoc/>
        public sealed override II2cBus I2cBus { get; }

        /// <inheritdoc/>
        public override ICO2ConcentrationSensor? CO2ConcentrationSensor { get => null; }

        /// <inheritdoc/>
        public override IGyroscope? Gyroscope { get => null; protected set => throw new System.NotImplementedException(); }

        /// <inheritdoc/>
        public override IAccelerometer? Accelerometer { get => null; protected set => throw new System.NotImplementedException(); }

        /// <inheritdoc/>
        public override IAnalogInputPort? BatteryVoltageInput { get => null; protected set => throw new System.NotImplementedException(); }

        public override Scd40? Scd40 => throw new System.NotImplementedException();

        /// <summary>
        /// Create a new GnssTrackerHardwareV1 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV1(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        { }
    }
}