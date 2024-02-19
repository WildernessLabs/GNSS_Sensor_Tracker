using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Motion;
using System;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware base class
    /// </summary>
    public abstract class GnssTrackerHardwareBase : IGnssTrackerHardware
    {
        private IF7CoreComputeMeadowDevice _device;

        private Bme688? _atmosphericSensor;
        private ITemperatureSensor? _temperatureSensor;
        private IHumiditySensor? _humiditySensor;
        private IBarometricPressureSensor? _barometricPressureSensor;
        private IGasResistanceSensor? _gasResistanceSensor;

        private IConnector?[]? _connectors;

        /// <inheritdoc/>
        protected Logger? Logger = Resolver.Log;

        /// <inheritdoc/>
        public abstract II2cBus I2cBus { get; }

        /// <inheritdoc/>
        public ISpiBus? SpiBus { get; }

        /// <inheritdoc/>
        public IPwmLed? OnboardLed { get; }

        /// <inheritdoc/>
        public Bme688? AtmosphericSensor => GetAtmosphericSensor();

        /// <inheritdoc/>
        public ITemperatureSensor? TemperatureSensor => GetTemperatureSensor();

        /// <inheritdoc/>
        public IHumiditySensor? HumiditySensor => GetHumiditySensor();

        /// <inheritdoc/>
        public IBarometricPressureSensor? BarometricPressureSensor => GetBarometricPressureSensor();

        /// <inheritdoc/>
        public IGasResistanceSensor? GasResistanceSensor => GetGasResistanceSensor();

        /// <inheritdoc/>
        public NeoM8? Gnss { get; protected set; }

        /// <inheritdoc/>
        public IPixelDisplay? Display { get; protected set; }

        /// <inheritdoc/>
        public IAnalogInputPort? SolarVoltageInput { get; protected set; }

        /// <inheritdoc/>
        public abstract IGyroscope? Gyroscope { get; protected set; }

        /// <inheritdoc/>
        public abstract IAccelerometer? Accelerometer { get; protected set; }

        /// <inheritdoc/>
        public abstract Scd40? Scd40 { get; }

        /// <inheritdoc/>
        public abstract ICO2ConcentrationSensor? CO2ConcentrationSensor { get; }

        /// <inheritdoc/>
        public abstract IAnalogInputPort? BatteryVoltageInput { get; protected set; }

        /// <inheritdoc/>
        public I2cConnector I2cHeader => (I2cConnector)Connectors[1]!;

        /// <inheritdoc/>
        public UartConnector UartHeader => (UartConnector)Connectors[0]!;

        /// <inheritdoc/>
        public DisplayConnector DisplayHeader => (DisplayConnector)Connectors[2]!;

        /// <summary>
        /// Collection of connectors on the GNSS Tracker
        /// </summary>
        public IConnector?[] Connectors
        {
            get
            {
                if (_connectors == null)
                {
                    _connectors = new IConnector[3];
                    _connectors[0] = CreateUartConnector();
                    _connectors[1] = CreateI2cConnector();
                    _connectors[2] = CreateDisplayConnector();
                }

                return _connectors;
            }
        }

        internal UartConnector CreateUartConnector()
        {
            Logger?.Trace("Creating Uart connector");

            return new UartConnector(
               "Uart",
                new PinMapping
                {
                    new PinMapping.PinAlias(UartConnector.PinNames.RX, _device.Pins.PI9),
                    new PinMapping.PinAlias(UartConnector.PinNames.TX, _device.Pins.PH13),
                },
                _device.PlatformOS.GetSerialPortName("com4")!);
        }

        internal I2cConnector CreateI2cConnector()
        {
            Logger?.Trace("Creating I2C connector");

            return new I2cConnector(
            "I2C",
            new PinMapping
            {
                new PinMapping.PinAlias(I2cConnector.PinNames.SCL, _device.Pins.PB6),
                new PinMapping.PinAlias(I2cConnector.PinNames.SDA, _device.Pins.PB7),
            },
            new I2cBusMapping(_device, 1));
        }

        internal DisplayConnector CreateDisplayConnector()
        {
            Logger?.Trace("Creating display connector");

            return new DisplayConnector(
               "Display",
                new PinMapping
                {
                    new PinMapping.PinAlias(DisplayConnector.PinNames.CS, _device.Pins.PH10),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.RST, _device.Pins.PB9),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.DC, _device.Pins.PB8),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.BUSY, _device.Pins.PB4),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.CLK, _device.Pins.SCK),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.COPI, _device.Pins.COPI),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.CIPO, _device.Pins.CIPO),
                });
        }

        private Bme688? GetAtmosphericSensor()
        {
            if (_atmosphericSensor == null)
            {
                InitializeBme688();
            }

            return _atmosphericSensor;
        }

        private ITemperatureSensor? GetTemperatureSensor()
        {
            if (_temperatureSensor == null)
            {
                InitializeBme688();
            }

            return _temperatureSensor;
        }

        private IHumiditySensor? GetHumiditySensor()
        {
            if (_humiditySensor == null)
            {
                InitializeBme688();
            }

            return _humiditySensor;
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

        private void InitializeBme688()
        {
            try
            {
                Logger?.Trace("BME688 Initializing...");

                var bme = new Bme688(I2cBus, (byte)Bme688.Addresses.Address_0x76);
                _atmosphericSensor = bme;
                _temperatureSensor = bme;
                _humiditySensor = bme;
                _barometricPressureSensor = bme;
                _gasResistanceSensor = bme;
                Resolver.SensorService.RegisterSensor(bme);

                Logger?.Trace("BME688 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the BME688 atmospheric sensor: {ex.Message}");
            }
        }
    }
}