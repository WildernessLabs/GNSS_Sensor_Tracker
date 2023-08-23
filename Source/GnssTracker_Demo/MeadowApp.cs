using GnssTracker_Demo.Controllers;
using Meadow;
using Meadow.Devices;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using WildernessLabs.Hardware.GnssTracker;

namespace GnssTracker_Demo
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        GnssPositionInfo? _positionInfo;
        protected IGnssTrackerHardware gnssTracker { get; set; }

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize hardware...");

            gnssTracker = GnssTracker.Create();

            if (gnssTracker.AtmosphericSensor is { } bme688)
            {
                bme688.Updated += Bme688Updated;
            }

            if (gnssTracker.SolarVoltageInput is { } solarVoltage)
            {
                solarVoltage.Updated += SolarVoltageUpdated;
            }

            if (gnssTracker.Gnss is { } gnss)
            {
                //gnss.GsaReceived += GnssGsaReceived;
                //gnss.GsvReceived += GnssGsvReceived;
                //gnss.VtgReceived += GnssVtgReceived;
                gnss.RmcReceived += GnssRmcReceived;
                gnss.GllReceived += GnssGllReceived;
            }

            if (gnssTracker.Display is { } display)
            {
                DisplayController.Initialize(display);
            }

            if (gnssTracker.OnboardLed is { } onboardLed)
            {
                onboardLed.IsOn = true;
            }

            Resolver.Log.Info("Initialization complete");

            return base.Initialize();
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688:        {(int)e.New.Temperature?.Celsius:0.0}C, {(int)e.New.Humidity?.Percent:0.#}%, {(int)e.New.Pressure?.Millibar:0.#}mbar");

            DisplayController.UpdateDisplay(e.New, _positionInfo);
        }

        private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            Resolver.Log.Info($"Solar Voltage: {e.New.Volts:0.#} volts");
        }

        private void GnssRmcReceived(object sender, GnssPositionInfo e)
        {
            if (e.Valid)
            {
                Resolver.Log.Info($"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");
                _positionInfo = e;
            }
        }

        private void GnssGllReceived(object sender, GnssPositionInfo e)
        {
            if (e.Valid)
            {
                Resolver.Log.Info($"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");
            }
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            if (gnssTracker.AtmosphericSensor is { } bme688)
            {
                bme688.StartUpdating(TimeSpan.FromSeconds(30));
            }

            if (gnssTracker.SolarVoltageInput is { } solarVoltage)
            {
                solarVoltage.StartUpdating(TimeSpan.FromSeconds(30));
            }

            if (gnssTracker.Gnss is { } gnss)
            {
                gnss.StartUpdating();
            }

            return base.Run();
        }
    }
}