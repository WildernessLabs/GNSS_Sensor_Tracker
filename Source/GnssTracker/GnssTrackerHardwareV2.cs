using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Motion;
using System;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware V2
    /// </summary>
    public class GnssTrackerHardwareV2 : GnssTrackerHardwareBase
    {
        private readonly IF7CoreComputeMeadowDevice _device;

        private Scd40? _scd40;
        private ICO2ConcentrationSensor? _cO2ConcentrationSensor;

        /// <inheritdoc/>
        public sealed override II2cBus I2cBus { get; }

        /// <inheritdoc/>
        public override Scd40? Scd40 => GetScd40Sensor();

        /// <inheritdoc/>
        public override ICO2ConcentrationSensor? CO2ConcentrationSensor => GetCO2ConcentrationSensor();

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
        public GnssTrackerHardwareV2(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        {
            _device = device;

            I2cBus = i2cBus;

            try
            {
                Logger?.Trace("BMI270 Initializing...");
                var bmi = new Bmi270(I2cBus);
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

        private Scd40? GetScd40Sensor()
        {
            if (_scd40 == null)
            {
                InitializeScd40();
            }

            return _scd40;
        }

        private ICO2ConcentrationSensor? GetCO2ConcentrationSensor()
        {
            if (_cO2ConcentrationSensor == null)
            {
                InitializeScd40();
            }

            return _cO2ConcentrationSensor;
        }

        private void InitializeScd40()
        {
            try
            {
                Logger?.Trace("SCD40 Initializing...");

                var scd = new Scd40(I2cBus);
                _scd40 = scd;
                _cO2ConcentrationSensor = scd;
                Resolver.SensorService.RegisterSensor(scd);

                Logger?.Trace("SCD40 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the SCD40 IMU: {ex.Message}");
            }
        }
    }
}