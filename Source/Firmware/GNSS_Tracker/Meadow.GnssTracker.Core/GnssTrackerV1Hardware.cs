using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.GnssTracker.Core.Contracts;
using Meadow.Hardware;
using System;

namespace Meadow.GnssTracker.Core
{
    public class GnssTrackerV1Hardware : IGnssTrackerHardware
    {
        protected F7CoreComputeV2 Device { get; }

        public II2cBus I2cBus { get; }
        public ISpiBus SpiBus { get; }
        public PwmLed OnboardLed { get; protected set; }
        public Bme688 AtmosphericSensor { get; protected set; }
        public NeoM8 Gnss { get; protected set; }
        public IGraphicsDisplay Display { get; protected set; }

        public GnssTrackerV1Hardware()
        {
            F7CoreComputeV2 device = (F7CoreComputeV2)Resolver.Device;

            Console.WriteLine("Initialize hardware...");

            //==== Onboard LED
            Console.WriteLine("Initializing Onboard LED");
            try
            {
                OnboardLed = new PwmLed(device: Device, Device.Pins.D20, TypicalForwardVoltage.Green);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing onboard LED: {e.Message}");
            }
            Console.WriteLine("Onboard LED initialized");

            //==== I2C Bus
            Console.WriteLine("Initializing I2C Bus");
            try
            {
                I2cBus = Device.CreateI2cBus();
                Console.WriteLine("I2C initialized");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing I2C Bus: {e.Message}");
            }
            

            //==== BME688
            Console.WriteLine("Initializing BME688");
            try
            {
                AtmosphericSensor = new Bme688(I2cBus, (byte)Bme688.Addresses.Address_0x76);
                Console.WriteLine("BME688 initialized");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing BME688: {e.Message}");
            }

            //==== GNSS
            Resolver.Log.Debug("Initializing GNSS");
            try
            {
                Gnss = new NeoM8(Device, Device.SerialPortNames.Com4, device.Pins.D09, device.Pins.D11);
                Resolver.Log.Debug("GNSS initialized");
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Err initializing GNSS: {e.Message}");
            }
            
            //==== SPI
            Console.WriteLine("Initializing SPI Bus");
            try
            {
                SpiBus = Device.CreateSpiBus(new Units.Frequency(48, Units.Frequency.UnitType.Kilohertz));
                Console.WriteLine("SPI initialized");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing SPI: {e.Message}");
            }

            //==== ePaper Display
            Console.WriteLine("Initializing ePaper Display");
            try
            {
                Display = new Ssd1680(device: Device,
                    spiBus: Device.CreateSpiBus(),
                    chipSelectPin: Device.Pins.D02,
                    dcPin: Device.Pins.D03,
                    resetPin: Device.Pins.D04,
                    busyPin: Device.Pins.D05,
                    width: 122,
                    height: 250);
                Console.WriteLine("ePaper Display initialized");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing ePaper Display: {e.Message}");
            }
        }
    }
}