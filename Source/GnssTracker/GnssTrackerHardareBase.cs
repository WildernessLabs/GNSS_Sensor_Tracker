using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Units;
using System;

namespace WildernessLabs.Hardware.GnssTracker
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware base class
    /// </summary>
    public abstract class GnssTrackerHardwareBase : IGnssTrackerHardware
    {
        /// <inheritdoc/>
        protected Logger Log = Resolver.Log;

        /// <inheritdoc/>
        public II2cBus? I2cBus { get; protected set; }

        /// <inheritdoc/>
        public ISpiBus? SpiBus { get; protected set; }

        /// <inheritdoc/>
        public PwmLed? OnboardLed { get; protected set; }

        /// <inheritdoc/>
        public Bme688? AtmosphericSensor { get; protected set; }

        /// <inheritdoc/>
        public NeoM8? Gnss { get; protected set; }

        /// <inheritdoc/>
        public abstract Scd40? EnvironmentalSensor { get; protected set; }

        /// <inheritdoc/>
        public abstract Bmi270? MotionSensor { get; protected set; }

        /// <inheritdoc/>
        public IGraphicsDisplay? Display { get; protected set; }

        /// <inheritdoc/>
        public IAnalogInputPort? SolarVoltageInput { get; protected set; }

        /// <summary>
        /// Create a new GnssTrackerHardware base object
        /// </summary>
        /// <param name="device"></param>
        public GnssTrackerHardwareBase(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        {
            Log.Debug("Initialize hardware...");
            I2cBus = i2cBus;

            try
            {
                Log.Debug("Initializing Onboard LED");

                OnboardLed = new PwmLed(device.Pins.D20, TypicalForwardVoltage.Green);

                Log.Debug("Onboard LED initialized");
            }
            catch (Exception e)
            {
                Log.Error($"Err initializing onboard LED: {e.Message}");
            }

            try
            {
                Log.Debug("Initializing BME688");

                AtmosphericSensor = new Bme688(I2cBus, (byte)Bme688.Addresses.Address_0x76);

                Log.Debug("BME688 initialized");
            }
            catch (Exception e)
            {
                Log.Error($"Err initializing BME688: {e.Message}");
            }

            try
            {
                Resolver.Log.Debug("Initializing GNSS");

                Gnss = new NeoM8(device, device.PlatformOS.GetSerialPortName("COM4"), device.Pins.D09, device.Pins.D11);

                Resolver.Log.Debug("GNSS initialized");
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Err initializing GNSS: {e.Message}");
            }

            try
            {
                Resolver.Log.Debug("Initializing ePaper Display");

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

                Resolver.Log.Debug("ePaper Display initialized");
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Err initializing ePaper Display: {e.Message}");
            }

            try
            {
                Resolver.Log.Debug("Instantiating Solar Voltage Input");
                SolarVoltageInput = device.Pins.A00.CreateAnalogInputPort(5);
                Resolver.Log.Debug("Solar Voltage Input up");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unabled to create the Switching Anemometer: {ex.Message}");
            }
        }
    }
}