using Meadow;
using Meadow.Units;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using Meadow.Logging;
using System;

namespace WildernessLabs.Hardware.GnssTracker
{
    public class GnssTrackerHardwareV1 : IGnssTrackerHardware
    {
        protected Logger Log = Resolver.Log;
        protected IF7CoreComputeMeadowDevice Device { get; }

        public II2cBus I2cBus { get; }
        public ISpiBus SpiBus { get; }
        public PwmLed OnboardLed { get; protected set; }
        public Bme688 AtmosphericSensor { get; protected set; }
        public NeoM8 Gnss { get; protected set; }
        public IGraphicsDisplay Display { get; protected set; }

        public GnssTrackerHardwareV1(IF7CoreComputeMeadowDevice device)
        {
            Device = device;

            Log.Debug("Initialize hardware...");

            try
            {
                Log.Debug("Initializing Onboard LED");

                OnboardLed = new PwmLed(Device.Pins.D20, TypicalForwardVoltage.Green);

                Log.Debug("Onboard LED initialized");
            }
            catch (Exception e)
            {
                Log.Error($"Err initializing onboard LED: {e.Message}");
            }
            
            try
            {
                Log.Debug("Initializing BME688");

                I2cBus = Device.CreateI2cBus();
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

                Gnss = new NeoM8(Device, Device.PlatformOS.GetSerialPortName("COM4"), Device.Pins.D09, Device.Pins.D11);

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
                SpiBus = Device.CreateSpiBus(
                    Device.Pins.SCK, 
                    Device.Pins.COPI,
                    Device.Pins.CIPO, 
                    config);
                Display = new Ssd1680(
                    spiBus: SpiBus,
                    chipSelectPin: Device.Pins.D02,
                    dcPin: Device.Pins.D03,
                    resetPin: Device.Pins.D04,
                    busyPin: Device.Pins.D05,
                    width: 122,
                    height: 250);

                Resolver.Log.Debug("ePaper Display initialized");
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Err initializing ePaper Display: {e.Message}");
            }
        }
    }
}