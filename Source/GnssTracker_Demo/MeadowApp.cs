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

        readonly TimeSpan sensorUpdateInterval = TimeSpan.FromSeconds(5);

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize hardware...");

            gnssTracker = GnssTracker.Create();

            if (gnssTracker.TemperatureSensor is { } temperatureSensor)
            {
                temperatureSensor.Updated += TemperatureSensorUpdated;
            }

            if (gnssTracker.HumiditySensor is { } humiditySensor)
            {
                humiditySensor.Updated += HumiditySensorUpdated;
            }

            if (gnssTracker.BarometricPressureSensor is { } barometer)
            {
                barometer.Updated += BarometerUpdated;
            }

            if (gnssTracker.GasResistanceSensor is { } gasResistanceSensor)
            {
                gasResistanceSensor.Updated += GasResistanceSensorUpdated;
            }

            if (gnssTracker.CO2ConcentrationSensor is { } cO2ConcentrationSensor)
            {
                cO2ConcentrationSensor.Updated += CO2ConcentrationSensorUpdated; ;
            }

            if (gnssTracker.Gyroscope is { } gyroscope)
            {
                gyroscope.Updated += GyroscopeUpdated;
            }

            if (gnssTracker.Accelerometer is { } accelerometer)
            {
                accelerometer.Updated += AccelerometerUpdated; ;
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

        private void TemperatureSensorUpdated(object sender, IChangeResult<Temperature> e)
        {
            Resolver.Log.Info($"TEMPERATURE:     {e.New.Celsius:N1}C");
        }

        private void HumiditySensorUpdated(object sender, IChangeResult<RelativeHumidity> e)
        {
            Resolver.Log.Info($"HUMIDITY:        {e.New.Percent:N1}%");
        }

        private void BarometerUpdated(object sender, IChangeResult<Pressure> e)
        {
            Resolver.Log.Info($"PRESSURE:        {e.New.Millibar:N1}mbar");
        }

        private void GasResistanceSensorUpdated(object sender, IChangeResult<Resistance> e)
        {
            Resolver.Log.Info($"RESISTANCE:      {e.New.Megaohms:N1}MΩ");
        }

        private void CO2ConcentrationSensorUpdated(object sender, IChangeResult<Concentration> e)
        {
            Resolver.Log.Info($"CO2 LEVELS:      {e.New.PartsPerMillion:N1}ppm");
        }

        private void GyroscopeUpdated(object sender, IChangeResult<AngularVelocity3D> e)
        {
            Resolver.Log.Info($"GYROSCOPE:       X:{e.New.X.DegreesPerSecond:N1}°/s, Y:{e.New.Y.DegreesPerSecond:N1}°/s, Z:{e.New.Z.DegreesPerSecond:N1}°/s");
        }

        private void AccelerometerUpdated(object sender, IChangeResult<Acceleration3D> e)
        {
            Resolver.Log.Info($"ACCELEROMETER:   X:{e.New.X.Gravity:N1}g, Y:{e.New.Y.Gravity:N1}g, Z:{e.New.Z.Gravity:N1}g");
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
            Resolver.Log.Info($"Solar Voltage:   {e.New.Volts:0.#} volts");
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

            if (gnssTracker.TemperatureSensor is { } temperatureSensor)
            {
                temperatureSensor.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.HumiditySensor is { } humiditySensor)
            {
                humiditySensor.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.BarometricPressureSensor is { } barometer)
            {
                barometer.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.GasResistanceSensor is { } gasResistanceSensor)
            {
                gasResistanceSensor.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.CO2ConcentrationSensor is { } cO2ConcentrationSensor)
            {
                cO2ConcentrationSensor.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.Gyroscope is { } gyroscope)
            {
                gyroscope.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.Accelerometer is { } accelerometer)
            {
                accelerometer.StartUpdating(sensorUpdateInterval);
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