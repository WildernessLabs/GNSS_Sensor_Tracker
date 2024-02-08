using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
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
        private ITemperatureSensor? _temperatureSensor;
        private IHumiditySensor? _humiditySensor;
        private IBarometricPressureSensor? _barometricPressureSensor;
        private IGasResistanceSensor? _gasResistanceSensor;
        private ICO2ConcentrationSensor? _cO2ConcentrationSensor;
        private IGyroscope? _gyroscope;
        private IAccelerometer? _accelerometer;

        /// <inheritdoc/>
        protected Logger? Logger = Resolver.Log;

        /// <inheritdoc/>
        public override IAnalogInputPort? BatteryVoltageInput { get; }

        /// <inheritdoc/>
        public override ITemperatureSensor? TemperatureSensor => GetTemperatureSensor();

        /// <inheritdoc/>
        public override IHumiditySensor? HumiditySensor => GetHumiditySensor();

        /// <inheritdoc/>
        public override IBarometricPressureSensor? BarometricPressureSensor => GetBarometricPressureSensor();

        /// <inheritdoc/>
        public override IGasResistanceSensor? GasResistanceSensor => GetGasResistanceSensor();

        /// <inheritdoc/>
        public override ICO2ConcentrationSensor? CO2ConcentrationSensor => GetCO2ConcentrationSensor();

        /// <inheritdoc/>
        public override IGyroscope? Gyroscope => GetGyroscope();

        /// <inheritdoc/>
        public override IAccelerometer? Accelerometer => GetAccelerometer();

        /// <summary>
        /// Create a new GnssTrackerHardwareV2 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV2(IF7CoreComputeMeadowDevice device, II2cBus i2cBus) : base(device, i2cBus)
        {
            try
            {
                Logger?.Debug("Instantiating Battery Voltage Input");
                BatteryVoltageInput = device.Pins.A04.CreateAnalogInputPort(5);
                Logger?.Debug("Battery Voltage Input up");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unabled to create Battery Voltage Input: {ex.Message}");
            }
        }

        private ITemperatureSensor? GetTemperatureSensor()
        {
            if (_temperatureSensor == null)
            {
                InitializeScd40();
            }

            return _temperatureSensor;
        }

        private IHumiditySensor? GetHumiditySensor()
        {
            if (_humiditySensor == null)
            {
                InitializeScd40();
            }

            return _humiditySensor;
        }

        private ICO2ConcentrationSensor? GetCO2ConcentrationSensor()
        {
            if (_cO2ConcentrationSensor == null)
            {
                InitializeScd40();
            }

            return _cO2ConcentrationSensor;
        }

        private IBarometricPressureSensor? GetBarometricPressureSensor()
        {
            if (_barometricPressureSensor == null)
            {
                InitializeBme688();
            }

            return _barometricPressureSensor;
        }

        private IGasResistanceSensor? GetGasResistanceSensor()
        {
            if (_gasResistanceSensor == null)
            {
                InitializeBme688();
            }

            return _gasResistanceSensor;
        }

        private IAccelerometer? GetAccelerometer()
        {
            if (_accelerometer == null)
            {
                InitializeBmi270();
            }

            return _accelerometer;
        }

        private IGyroscope? GetGyroscope()
        {
            if (_gyroscope == null)
            {
                InitializeBmi270();
            }

            return _gyroscope;
        }

        private void InitializeScd40()
        {
            try
            {
                Logger?.Trace("Scd40 Initializing");
                var scd = new Scd40(I2cBus);
                _temperatureSensor = scd;
                _humiditySensor = scd;
                _cO2ConcentrationSensor = scd;
                //Resolver.SensorService.RegisterSensor(scd);
                Logger?.Trace("Scd40 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the Scd40 IMU: {ex.Message}");
            }
        }

        private void InitializeBme688()
        {
            try
            {
                Logger?.Trace("Bme688 Initializing");
                var bme = new Bme688(I2cBus, (byte)Bme68x.Addresses.Address_0x76);
                _barometricPressureSensor = bme;
                _gasResistanceSensor = bme;
                Resolver.SensorService.RegisterSensor(bme);
                Logger?.Trace("Bme688 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the Bme688 atmospheric sensor: {ex.Message}");
            }
        }

        private void InitializeBmi270()
        {
            try
            {
                Logger?.Trace("Bmi270 Initializing");
                var bmi = new Bmi270(I2cBus);
                _gyroscope = bmi;
                _accelerometer = bmi;
                Resolver.SensorService.RegisterSensor(bmi);
                Logger?.Trace("Bmi270 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the BMI270 IMU: {ex.Message}");
            }
        }
    }
}