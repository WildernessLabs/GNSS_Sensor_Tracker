﻿using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.GnssTracker.Core.Contracts;
using Meadow.Hardware;
using Meadow.Logging;
using System;

namespace Meadow.GnssTracker.Core
{
    public class GnssTrackerV1Hardware : IGnssTrackerHardware
    {
        protected Logger Log = Resolver.Log;
        protected F7CoreComputeV2 Device { get; }

        public II2cBus I2cBus { get; }
        public ISpiBus SpiBus { get; }
        public PwmLed OnboardLed { get; protected set; }
        public Bme688 AtmosphericSensor { get; protected set; }
        public NeoM8 Gnss { get; protected set; }
        public IGraphicsDisplay Display { get; protected set; }

        public GnssTrackerV1Hardware()
        {
            Device = (F7CoreComputeV2) Resolver.Device;

            Console.WriteLine("Initialize hardware...");

            Console.WriteLine("Initializing Onboard LED");
            try
            {
                OnboardLed = new PwmLed(Device.Pins.D20, TypicalForwardVoltage.Green);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing onboard LED: {e.Message}");
            }
            Console.WriteLine("Onboard LED initialized");

            Console.WriteLine("Initializing BME688");
            try
            {
                I2cBus = Device.CreateI2cBus();
                AtmosphericSensor = new Bme688(I2cBus, (byte)Bme688.Addresses.Address_0x76);
                Console.WriteLine("BME688 initialized");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err initializing BME688: {e.Message}");
            }

            Resolver.Log.Debug("Initializing GNSS");
            try
            {
                Gnss = new NeoM8(Device, Device.PlatformOS.GetSerialPortName("COM4"), Device.Pins.D09, Device.Pins.D11);
                Resolver.Log.Debug("GNSS initialized");
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Err initializing GNSS: {e.Message}");
            }

            Resolver.Log.Debug("Initializing ePaper Display");
            try
            {
                SpiBus = Device.CreateSpiBus(new Units.Frequency(48, Units.Frequency.UnitType.Kilohertz));
                Display = new Ssd1680(
                    spiBus: Device.CreateSpiBus(),
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