using Meadow;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Motion;
using System;

namespace WildernessLabs.Hardware.GnssTracker
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware V2
    /// </summary>
    public class GnssTrackerHardwareV2 : GnssTrackerHardwareBase
    {
        /// <inheritdoc/>
        public override ICO2ConcentrationSensor? CO2ConcentrationSensor { get; protected set; }

        /// <inheritdoc/>
        public override IGyroscope? Gyroscope { get; protected set; }

        /// <inheritdoc/>
        public override IAccelerometer? Accelerometer { get; protected set; }

        /// <inheritdoc/>
        public override IAnalogInputPort? BatteryVoltageInput { get; protected set; }

        /// <summary>
        /// Create a new GnssTrackerHardwareV2 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV2(IF7CoreComputeMeadowDevice device, II2cBus i2cBus) : base(device, i2cBus)
        {
            try
            {
                Logger?.Trace("SCD40 Initializing...");
                var scd = new Scd40(i2cBus);
                //TemperatureSensor = scd;
                //HumiditySensor = scd;
                CO2ConcentrationSensor = scd;
                Resolver.SensorService.RegisterSensor(scd);
                Logger?.Trace("SCD40 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the SCD40 IMU: {ex.Message}");
            }

            try
            {
                Logger?.Trace("BMI270 Initializing...");
                var bmi = new Bmi270(i2cBus);
                Gyroscope = bmi;
                Accelerometer = bmi;
                Resolver.SensorService.RegisterSensor(bmi);
                Logger?.Trace("BMI270 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the BMI270 IMU: {ex.Message}");
            }

            try
            {
                Logger?.Debug("Battery Voltage Input Instantiating...");
                BatteryVoltageInput = device.Pins.A04.CreateAnalogInputPort(5);
                Logger?.Debug("Battery Voltage Input up");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create Battery Voltage Input: {ex.Message}");
            }
        }
    }
}