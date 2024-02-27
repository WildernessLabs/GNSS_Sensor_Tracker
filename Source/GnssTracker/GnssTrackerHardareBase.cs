using Meadow.Foundation.Displays;
using Meadow.Foundation.Sensors.Atmospheric;
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
        protected IF7CoreComputeMeadowDevice device;

        private Bme688? atmosphericSensor;
        private ITemperatureSensor? temperatureSensor;
        private IHumiditySensor? humiditySensor;
        private IBarometricPressureSensor? barometricPressureSensor;
        private IGasResistanceSensor? gasResistanceSensor;

        private NeoM8? gnss;

        private IPixelDisplay? display;

        private IAnalogInputPort? solarVoltageInput;

        private IConnector?[]? connectors;

        /// <inheritdoc/>
        protected Logger? Logger = Resolver.Log;

        /// <inheritdoc/>
        public abstract II2cBus I2cBus { get; }

        /// <inheritdoc/>
        public ISpiBus? SpiBus { get; protected set; }

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
        public NeoM8? Gnss => GetGnss();

        /// <inheritdoc/>
        public IPixelDisplay? Display => GetEPaperDisplay();

        /// <inheritdoc/>
        public abstract IRgbPwmLed? OnboardRgbLed { get; }

        /// <inheritdoc/>
        public abstract IGyroscope? Gyroscope { get; }

        /// <inheritdoc/>
        public abstract IAccelerometer? Accelerometer { get; }

        /// <inheritdoc/>
        public abstract ICO2ConcentrationSensor? CO2ConcentrationSensor { get; }

        /// <inheritdoc/>
        public abstract IAnalogInputPort? BatteryVoltageInput { get; }

        /// <inheritdoc/>
        public IAnalogInputPort? SolarVoltageInput => GetSolarVoltageInput();

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
                if (connectors == null)
                {
                    connectors = new IConnector[3];
                    connectors[0] = CreateUartConnector();
                    connectors[1] = CreateI2cConnector();
                    connectors[2] = CreateDisplayConnector();
                }

                return connectors;
            }
        }

        internal UartConnector CreateUartConnector()
        {
            Logger?.Trace("Creating Uart connector");

            return new UartConnector(
               "Uart",
                new PinMapping
                {
                    new PinMapping.PinAlias(UartConnector.PinNames.RX, device.Pins.PI9),
                    new PinMapping.PinAlias(UartConnector.PinNames.TX, device.Pins.PH13),
                },
                device.PlatformOS.GetSerialPortName("com4")!);
        }

        internal I2cConnector CreateI2cConnector()
        {
            Logger?.Trace("Creating I2C connector");

            return new I2cConnector(
            "I2C",
            new PinMapping
            {
                new PinMapping.PinAlias(I2cConnector.PinNames.SCL, device.Pins.PB6),
                new PinMapping.PinAlias(I2cConnector.PinNames.SDA, device.Pins.PB7),
            },
            new I2cBusMapping(device, 1));
        }

        internal DisplayConnector CreateDisplayConnector()
        {
            Logger?.Trace("Creating display connector");

            return new DisplayConnector(
               "Display",
                new PinMapping
                {
                    new PinMapping.PinAlias(DisplayConnector.PinNames.CS, device.Pins.PH10),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.RST, device.Pins.PB9),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.DC, device.Pins.PB8),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.BUSY, device.Pins.PB4),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.CLK, device.Pins.SCK),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.COPI, device.Pins.COPI),
                    new PinMapping.PinAlias(DisplayConnector.PinNames.CIPO, device.Pins.CIPO),
                });
        }

        private Bme688? GetAtmosphericSensor()
        {
            if (atmosphericSensor == null)
            {
                InitializeBme688();
            }

            return atmosphericSensor;
        }

        private ITemperatureSensor? GetTemperatureSensor()
        {
            if (temperatureSensor == null)
            {
                InitializeBme688();
            }

            return temperatureSensor;
        }

        private IHumiditySensor? GetHumiditySensor()
        {
            if (humiditySensor == null)
            {
                InitializeBme688();
            }

            return humiditySensor;
        }

        private IBarometricPressureSensor? GetBarometricPressureSensor()
        {
            if (barometricPressureSensor == null)
            {
                InitializeBme688();
            }

            return barometricPressureSensor;
        }

        private IGasResistanceSensor? GetGasResistanceSensor()
        {
            if (gasResistanceSensor == null)
            {
                InitializeBme688();
            }

            return gasResistanceSensor;
        }

        private void InitializeBme688()
        {
            try
            {
                Logger?.Trace("BME688 Initializing...");

                var bme = new Bme688(I2cBus, (byte)Bme688.Addresses.Address_0x76);
                atmosphericSensor = bme;
                temperatureSensor = bme;
                humiditySensor = bme;
                barometricPressureSensor = bme;
                gasResistanceSensor = bme;
                Resolver.SensorService.RegisterSensor(bme);

                Logger?.Trace("BME688 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the BME688 atmospheric sensor: {ex.Message}");
            }
        }

        private IAnalogInputPort? GetSolarVoltageInput()
        {
            if (solarVoltageInput == null)
            {
                InitializeSolarVoltageInput();
            }

            return solarVoltageInput;
        }

        private void InitializeSolarVoltageInput()
        {
            try
            {
                Logger?.Debug("Solar Voltage Input Initializing...");
                solarVoltageInput = device.Pins.A00.CreateAnalogInputPort(5);
                Logger?.Debug("Solar Voltage initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unabled to create Solar Voltage Input: {ex.Message}");
            }
        }

        private IPixelDisplay? GetEPaperDisplay()
        {
            if (display == null)
            {
                InitializeSsd1680Display();
            }

            return display;
        }

        private void InitializeSsd1680Display()
        {
            try
            {
                Logger?.Debug("ePaper Display Initializing...");

                var config = new SpiClockConfiguration(new Frequency(48000, Frequency.UnitType.Kilohertz), SpiClockConfiguration.Mode.Mode0);
                SpiBus = device.CreateSpiBus(
                    device.Pins.SCK,
                    device.Pins.COPI,
                    device.Pins.CIPO,
                    config);
                display = new Ssd1680(
                spiBus: SpiBus,
                chipSelectPin: device.Pins.D02,
                dcPin: device.Pins.D03,
                    resetPin: device.Pins.D04,
                    busyPin: device.Pins.D05,
                    width: 122,
                    height: 250);

                Logger?.Debug("ePaper Display initialized");
            }
            catch (Exception e)
            {
                Logger?.Error($"Err initializing ePaper Display: {e.Message}");
            }
        }

        private NeoM8? GetGnss()
        {
            if (gnss == null)
            {
                InitializeGnss();
            }

            return gnss;
        }

        private void InitializeGnss()
        {
            try
            {
                Logger?.Debug("GNSS Initializing...");
                gnss = new NeoM8(device, device.PlatformOS.GetSerialPortName("COM4")!, device.Pins.D18);

                Logger?.Debug("GNSS initialized");
            }
            catch (Exception e)
            {
                Logger?.Error($"Err initializing GNSS: {e.Message}");
            }
        }
    }
}