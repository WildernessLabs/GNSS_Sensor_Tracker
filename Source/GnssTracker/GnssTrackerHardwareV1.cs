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
        public override IRgbPwmLed? OnboardRgbLed => null;

        /// <inheritdoc/>
        public override ICO2ConcentrationSensor? CO2ConcentrationSensor => null;

        /// <inheritdoc/>
        public override IGyroscope? Gyroscope => null;

        /// <inheritdoc/>
        public override IAccelerometer? Accelerometer => null;

        /// <inheritdoc/>
        public override IObservableAnalogInputPort? BatteryVoltageInput => null;

        /// <summary>
        /// Create a new GnssTrackerHardwareV1 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV1(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        {
            base.device = device;

            I2cBus = i2cBus;
        }
    }
}