using Meadow.Hardware;
using Meadow.Peripherals.Leds;
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
        public override IRgbPwmLed? OnboardRgbLed => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public override ICO2ConcentrationSensor? CO2ConcentrationSensor => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public override IGyroscope? Gyroscope => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public override IAccelerometer? Accelerometer => throw new System.NotImplementedException();

        /// <inheritdoc/>
        public override IAnalogInputPort? BatteryVoltageInput => throw new System.NotImplementedException();

        /// <summary>
        /// Create a new GnssTrackerHardwareV1 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV1(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        {
            _device = device;

            I2cBus = i2cBus;
        }
    }
}