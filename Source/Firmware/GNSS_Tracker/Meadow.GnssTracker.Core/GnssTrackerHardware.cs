using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using System;

namespace Meadow.GnssTracker.Core
{
    public class GnssTrackerHardware
    {
        protected F7CoreComputeV2 Device { get; }
        protected II2cBus? I2c { get; }
        protected ISpiBus Spi { get; }

        public PwmLed? OnboardLed { get; protected set; }
        public Bme688 Bme68X { get; protected set; }
        public NeoM8 Gnss { get; protected set; }
        //public Mt3339 Gnss { get; protected set; }
        public Epd2in13b EPaperDisplay { get; protected set; }

        private IDigitalOutputPort Reset { get; set; }

        public GnssTrackerHardware(F7CoreComputeV2 device)
        {
            this.Device = device;

            Console.WriteLine("Initialize hardware...");

            //==== Onboard LED
            Console.WriteLine("Initializing Onboard LED.");
            try
            {
                OnboardLed = new PwmLed(device: Device, Device.Pins.D20, TypicalForwardVoltage.Green);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing onboard LED: {e.Message}");
            }
            Console.WriteLine("Onboard LED initialized.");

            //==== I2C Bus
            Console.WriteLine("Initializing I2C Bus.");
            try
            {
                I2c = Device.CreateI2cBus();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing I2C Bus: {e.Message}");
            }
            Console.WriteLine("I2C initialized.");

            //==== BME688
            Console.WriteLine("Initializing BME688.");
            try
            {
                Bme68X = new Bme688(I2c, (byte)Bme688.Addresses.Address_0x76);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing BME688: {e.Message}");
            }
            Console.WriteLine("BME688 initialized.");

            ////==== GNSS
            Resolver.Log.Debug("Initializing GNSS.");
            try
            {
                //Reset = Device.CreateDigitalOutputPort(Device.Pins.D09, true);
                //Gnss = new Mt3339(Device, Device.SerialPortNames.Com4);
                Gnss = new NeoM8(Device, Device.SerialPortNames.Com4, device.Pins.D09, device.Pins.D11);
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Err initializing GNSS: {e.Message}");
            }
            Resolver.Log.Debug("GNSS initialized.");

            //==== SPI
            Console.WriteLine("Initializing SPI Bus.");
            try
            {
                Spi = Device.CreateSpiBus(new Units.Frequency(48, Units.Frequency.UnitType.Kilohertz));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing SPI: {e.Message}");
            }
            Console.WriteLine("SPI initialized.");

            //==== ePaper Display
            Console.WriteLine("Initializing ePaper Display");
            try
            {
                this.EPaperDisplay = new Epd2in13b(device: Device,
                    spiBus: Device.CreateSpiBus(),
                    chipSelectPin: Device.Pins.D02,
                    dcPin: Device.Pins.D03,
                    resetPin: Device.Pins.D04,
                    busyPin: Device.Pins.D05);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing ePaper Display: {e.Message}");
            }
            Console.WriteLine("ePaper Display initialized.");

        }
    }
}

