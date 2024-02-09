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
        GnssPositionInfo lastGNSSPosition;
        DateTime lastGNSSPositionReportTime = DateTime.MinValue;

        protected DisplayController DisplayController { get; set; }

        protected IGnssTrackerHardware gnssTracker { get; set; }

        readonly TimeSpan GNSSPositionReportInterval = TimeSpan.FromSeconds(15);

        readonly TimeSpan sensorUpdateInterval = TimeSpan.FromSeconds(90);

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize hardware...");

            gnssTracker = GnssTracker.Create();

            if (gnssTracker.AtmosphericSensor is { } bme688)
            {
                bme688.Updated += Bme688Updated;
            }

            if (gnssTracker.EnvironmentalSensor is { } scd40)
            {
                scd40.Updated += Scd40_Updated;
            }

            if (gnssTracker.MotionSensor is { } bmi270)
            {
                bmi270.Updated += Bmi270_Updated;
            }

            if (gnssTracker.SolarVoltageInput is { } solarVoltage)
            {
                solarVoltage.Updated += SolarVoltageUpdated;
            }

            if (gnssTracker.BatteryVoltageInput is { } batteryVoltage)
            {
                batteryVoltage.Updated += BatteryVoltageUpdated; ;
            }

            if (gnssTracker.Gnss is { } gnss)
            {
                gnss.RmcReceived += GnssRmcReceived;
                gnss.GllReceived += GnssGllReceived;
            }

            if (gnssTracker.Display is { } display)
            {
                DisplayController = new DisplayController(display);
            }

            if (gnssTracker.OnboardLed is { } onboardLed)
            {
                onboardLed.IsOn = true;
            }

            Resolver.Log.Info("Initialization complete");

            return Task.CompletedTask;
        }

        private void Bmi270_Updated(object sender, IChangeResult<(Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature)> e)
        {
            Resolver.Log.Info($"BMI270:        X:{e.New.Acceleration3D.Value.X.Gravity:0.0}g, Y:{e.New.Acceleration3D.Value.Y.Gravity:0.0}g, Z:{e.New.Acceleration3D.Value.Z.Gravity:0.0}g");
        }

        private void Scd40_Updated(object sender, IChangeResult<(Concentration? Concentration, Temperature? Temperature, RelativeHumidity? Humidity)> e)
        {
            Resolver.Log.Info($"SCD40:         {(int)e.New.Temperature?.Celsius:0.0}C, {(int)e.New.Humidity?.Percent:0.#}%, {(int)e.New.Concentration?.PartsPerMillion:0.#}ppm");
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Info($"BME688:        {(int)e.New.Temperature?.Celsius:0.0}C, {(int)e.New.Humidity?.Percent:0.#}%, {(int)e.New.Pressure?.Millibar:0.#}mbar");

            //DisplayController.UpdateDisplay(e.New, lastGNSSPosition);
        }

        private void GnssRmcReceived(object sender, GnssPositionInfo e)
        {
            if (e.Valid)
            {
                ReportGNSSPosition(e);
                lastGNSSPosition = e;
            }
        }

        private void GnssGllReceived(object sender, GnssPositionInfo e)
        {
            if (e.Valid)
            {
                ReportGNSSPosition(e);
                lastGNSSPosition = e;
            }
        }

        private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            Resolver.Log.Info($"Solar Voltage: {e.New.Volts:0.#} volts");
        }

        private void BatteryVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            Resolver.Log.Info($"Battery Voltage: {e.New.Volts:0.#} volts");
        }

        private void ReportGNSSPosition(GnssPositionInfo e)
        {
            if (e.Valid)
            {
                if (DateTime.UtcNow - lastGNSSPositionReportTime >= GNSSPositionReportInterval)
                {
                    Resolver.Log.Info($"GNSS Position: lat: [{e.Position.Latitude}], long: [{e.Position.Longitude}]");

                    lastGNSSPositionReportTime = DateTime.UtcNow;
                }
            }
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            if (gnssTracker.AtmosphericSensor is { } bme688)
            {
                bme688.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.EnvironmentalSensor is { } scd40)
            {
                scd40.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.MotionSensor is { } bmi270)
            {
                bmi270.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.SolarVoltageInput is { } solarVoltage)
            {
                solarVoltage.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.BatteryVoltageInput is { } batteryVoltage)
            {
                batteryVoltage.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.Gnss is { } gnss)
            {
                gnss.StartUpdating();
            }

            return Task.CompletedTask;
        }
    }
}