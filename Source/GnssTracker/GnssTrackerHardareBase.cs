using Meadow.Foundation.Displays;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Motion;
using Meadow.Units;
using System;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware base class
    /// </summary>
    public abstract class GnssTrackerHardwareBase : IGnssTrackerHardware
    {
        private IPixelDisplay? _display;




        /// <inheritdoc/>
        protected Logger? Logger = Resolver.Log;

        /// <inheritdoc/>
        public II2cBus? I2cBus { get; protected set; }

        /// <inheritdoc/>
        public ISpiBus? SpiBus { get; protected set; }

        /// <inheritdoc/>
        public IPwmLed? OnboardLed { get; protected set; }

        /// <inheritdoc/>
        public NeoM8? GnssSensor { get; protected set; }

        /// <inheritdoc/>
        public IPixelDisplay? Display { get; protected set; }

        /// <inheritdoc/>
        public IAnalogInputPort? SolarVoltageInput { get; protected set; }

        /// <inheritdoc/>
        public I2cConnector I2cHeader => (I2cConnector)Connectors[1]!;

        /// <inheritdoc/>
        public UartConnector UartHeader => (UartConnector)Connectors[0]!;

        /// <inheritdoc/>
        public DisplayConnector DisplayHeader => (DisplayConnector)Connectors[2]!;

        /// <inheritdoc/>
        public abstract IAnalogInputPort? BatteryVoltageInput { get; }

        /// <inheritdoc/>
        public abstract ITemperatureSensor? TemperatureSensor { get; }

        /// <inheritdoc/>
        public abstract IHumiditySensor? HumiditySensor { get; }

        /// <inheritdoc/>
        public abstract IBarometricPressureSensor? BarometricPressureSensor { get; }

        /// <inheritdoc/>
        public abstract IGasResistanceSensor? GasResistanceSensor { get; }

        /// <inheritdoc/>
        public abstract ICO2ConcentrationSensor? CO2ConcentrationSensor { get; }

        /// <inheritdoc/>
        public abstract IGyroscope? Gyroscope { get; }

        /// <inheritdoc/>
        public abstract IAccelerometer? Accelerometer { get; }

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
        private IConnector?[]? _connectors;

        private readonly IF7CoreComputeMeadowDevice _device;

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

        /// <summary>
        /// Create a new GnssTrackerHardware base object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareBase(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        {
            Logger?.Debug("Initialize hardware...");
            _device = device;
            I2cBus = i2cBus;

            try
            {
                Logger?.Debug("Initializing Onboard LED");

                OnboardLed = new PwmLed(device.Pins.D20, TypicalForwardVoltage.Green);

                Logger?.Debug("Onboard LED initialized");
            }
            catch (Exception e)
            {
                Logger?.Error($"Err initializing onboard LED: {e.Message}");
            }

            try
            {
                Logger?.Debug("Initializing GNSS");

                GnssSensor = new NeoM8(device, device.PlatformOS.GetSerialPortName("COM4")!, device.Pins.D09, device.Pins.D11);

                Logger?.Debug("GNSS initialized");
            }
            catch (Exception e)
            {
                Logger?.Error($"Err initializing GNSS: {e.Message}");
            }

            try
            {
                Logger?.Debug("Initializing ePaper Display");

                var config = new SpiClockConfiguration(new Frequency(48000, Frequency.UnitType.Kilohertz), SpiClockConfiguration.Mode.Mode0);
                SpiBus = device.CreateSpiBus(
                    device.Pins.SCK,
                    device.Pins.COPI,
                    device.Pins.CIPO,
                    config);
                Display = new Ssd1680(
                    spiBus: SpiBus,
                    chipSelectPin: device.Pins.D02,
                    dcPin: device.Pins.D03,
                    resetPin: device.Pins.D04,
                    busyPin: device.Pins.D05,
                    width: 122,
                    height: 250);
                _display = Display;

                Logger?.Debug("ePaper Display initialized");
            }
            catch (Exception e)
            {
                Logger?.Error($"Err initializing ePaper Display: {e.Message}");
            }

            try
            {
                Logger?.Debug("initializing Solar Voltage Input");
                SolarVoltageInput = device.Pins.A00.CreateAnalogInputPort(5);
                Logger?.Debug("Solar Voltage Input initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unabled to create Solar Voltage Input: {ex.Message}");
            }
        }
    }
}