using Meadow.Foundation.Sensors.Atmospheric;
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
    /// Represents a Gnss Tracker Hardware V1
    /// </summary>
    public class GnssTrackerHardwareV1 : GnssTrackerHardwareBase
    {
        private ITemperatureSensor? _temperatureSensor;
        private IHumiditySensor? _humiditySensor;
        private IBarometricPressureSensor? _barometricPressureSensor;
        private IGasResistanceSensor? _gasResistanceSensor;

        /// <inheritdoc/>
        protected Logger? Logger = Resolver.Log;

        /// <inheritdoc/>
        public override IAnalogInputPort? BatteryVoltageInput { get => null; }

        /// <inheritdoc/>
        public override ITemperatureSensor? TemperatureSensor => GetTemperatureSensor();

        /// <inheritdoc/>
        public override IHumiditySensor? HumiditySensor => GetHumiditySensor();

        /// <inheritdoc/>
        public override IBarometricPressureSensor? BarometricPressureSensor => GetBarometricPressureSensor();

        /// <inheritdoc/>
        public override IGasResistanceSensor? GasResistanceSensor => GetGasResistanceSensor();

        /// <inheritdoc/>
        public override ICO2ConcentrationSensor? CO2ConcentrationSensor { get => null; }

        /// <inheritdoc/>
        public override IGyroscope? Gyroscope { get => null; }

        /// <inheritdoc/>
        public override IAccelerometer? Accelerometer { get => null; }

        /// <summary>
        /// Create a new GnssTrackerHardwareV1 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV1(IF7CoreComputeMeadowDevice device, II2cBus i2cBus) : base(device, i2cBus)
        { }

        private ITemperatureSensor? GetTemperatureSensor()
        {
            if (_temperatureSensor == null)
            {
                InitializeBme688();
            }

            return _temperatureSensor;
        }

        private IBarometricPressureSensor? GetBarometricPressureSensor()
        {
            if (_barometricPressureSensor == null)
            {
                InitializeBme688();
            }

            return _barometricPressureSensor;
        }

        private IHumiditySensor? GetHumiditySensor()
        {
            if (_humiditySensor == null)
            {
                InitializeBme688();
            }

            return _humiditySensor;
        }

        private IGasResistanceSensor? GetGasResistanceSensor()
        {
            if (_gasResistanceSensor == null)
            {
                InitializeBme688();
            }

            return _gasResistanceSensor;
        }

        private void InitializeBme688()
        {
            try
            {
                Logger?.Trace("Bme688: Initializing");
                var bme = new Bme688(I2cBus, (byte)Bme68x.Addresses.Address_0x76);
                _temperatureSensor = bme;
                _humiditySensor = bme;
                _barometricPressureSensor = bme;
                _gasResistanceSensor = bme;
                Resolver.SensorService.RegisterSensor(bme);
                Logger?.Trace("Bme688: Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the BME688 atmospheric sensor: {ex.Message}");
            }
        }
    }
}