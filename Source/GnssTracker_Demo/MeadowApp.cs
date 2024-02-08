using GnssTracker_Demo.Controllers;
using Meadow;
using Meadow.Devices;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace GnssTracker_Demo
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        GnssPositionInfo lastGNSSPosition;
        Voltage? BatteryVoltage;
        Voltage? SolarVoltage;
        DateTime lastGNSSPositionReportTime = DateTime.MinValue;

        protected DisplayController displayController { get; set; }

        protected IGnssTrackerHardware gnssTracker { get; set; }

        readonly TimeSpan GNSSPositionReportInterval = TimeSpan.FromSeconds(15);

        readonly TimeSpan sensorUpdateInterval = TimeSpan.FromSeconds(90);

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize hardware...");

            gnssTracker = GnssTracker.Create();

            if (gnssTracker.OnboardLed is { } onboardLed)
            {
                onboardLed.IsOn = true;
            }

            //if (gnssTracker.Display is { } display)
            //{
            //    displayController = new DisplayController(display);
            //}

            if (gnssTracker.TemperatureSensor is { } temperatureSensor)
            {
                temperatureSensor.Updated += TemperatureSensorUpdated;
            }

            if (gnssTracker.HumiditySensor is { } humiditySensor)
            {
                humiditySensor.Updated += HumiditySensorUpdated;
            }

            if (gnssTracker.CO2ConcentrationSensor is { } co2ConcentrationSensor)
            {
                co2ConcentrationSensor.Updated += Co2ConcentrationSensorUpdated;
            }

            if (gnssTracker.BarometricPressureSensor is { } barometricPressureSensor)
            {
                barometricPressureSensor.Updated += BarometricPressureSensorUpdated;
            }

            if (gnssTracker.GasResistanceSensor is { } gasResistanceSensor)
            {
                gasResistanceSensor.Updated += GasResistanceSensorUpdated;
            }

            if (gnssTracker.SolarVoltageInput is { } solarVoltage)
            {
                solarVoltage.Updated += SolarVoltageUpdated;
            }

            if (gnssTracker.BatteryVoltageInput is { } batteryVoltage)
            {
                batteryVoltage.Updated += BatteryVoltageUpdated; ;
            }

            if (gnssTracker.GnssSensor is { } gnss)
            {
                gnss.RmcReceived += GnssRmcReceived;
                gnss.GllReceived += GnssGllReceived;
            }



            Resolver.Log.Info("Initialization complete");

            return Task.CompletedTask;
        }

        private void TemperatureSensorUpdated(object sender, IChangeResult<Temperature> e)
        {
            Resolver.Log.Info($"Temperature: {e.New.Celsius:n1} °C");
        }

        private void HumiditySensorUpdated(object sender, IChangeResult<RelativeHumidity> e)
        {
            Resolver.Log.Info($"Humidity: {e.New.Percent:n1} %");
        }

        private void Co2ConcentrationSensorUpdated(object sender, IChangeResult<Concentration> e)
        {
            Resolver.Log.Info($"CO2 Levels: {e.New.PartsPerMillion:n1} PPM");
        }

        private void BarometricPressureSensorUpdated(object sender, IChangeResult<Pressure> e)
        {
            Resolver.Log.Info($"Humidity: {e.New.StandardAtmosphere:n1} ATM");
        }

        private void GasResistanceSensorUpdated(object sender, IChangeResult<Resistance> e)
        {
            Resolver.Log.Info($"Gas Resistance: {e.New.Milliohms:n1} MOHMS");
        }

        private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            SolarVoltage = e.New;
            Resolver.Log.Info($"Solar Voltage: {e.New.Volts:0.#} volts");
        }

        private void BatteryVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            BatteryVoltage = e.New;
            Resolver.Log.Info($"Battery Voltage: {e.New.Volts:0.#} volts");
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

            if (gnssTracker.TemperatureSensor is { } temperature)
            {
                temperature.StartUpdating(TimeSpan.FromSeconds(5));
                Resolver.Log.Info("1");
            }



            //if (gnssTracker.HumiditySensor is { } humidity)
            //{
            //    humidity.StartUpdating(sensorUpdateInterval);
            //}

            //Resolver.Log.Info("2");

            //if (gnssTracker.CO2ConcentrationSensor is { } c02Concentration)
            //{
            //    c02Concentration.StartUpdating(sensorUpdateInterval);
            //}

            //Resolver.Log.Info("3");

            //if (gnssTracker.BarometricPressureSensor is { } barometer)
            //{
            //    barometer.StartUpdating(sensorUpdateInterval);
            //}

            //Resolver.Log.Info("4");

            //if (gnssTracker.GasResistanceSensor is { } gasResistance)
            //{
            //    gasResistance.StartUpdating(sensorUpdateInterval);
            //}

            //Resolver.Log.Info("5");

            //if (gnssTracker.Accelerometer is { } accelerometer)
            //{
            //    accelerometer.StartUpdating(sensorUpdateInterval);
            //}

            //Resolver.Log.Info("6");

            //if (gnssTracker.Gyroscope is { } gyroscope)
            //{
            //    gyroscope.StartUpdating(sensorUpdateInterval);
            //}

            //Resolver.Log.Info("7");

            //if (gnssTracker.SolarVoltageInput is { } solarVoltage)
            //{
            //    solarVoltage.StartUpdating(sensorUpdateInterval);
            //}

            //Resolver.Log.Info("8");

            //if (gnssTracker.BatteryVoltageInput is { } batteryVoltage)
            //{
            //    batteryVoltage.StartUpdating(sensorUpdateInterval);
            //}

            //Resolver.Log.Info("9");

            //if (gnssTracker.GnssSensor is { } gnssSensor)
            //{
            //    gnssSensor.StartUpdating();
            //}

            //while (true)
            //{
            //    Resolver.Log.Info("while");

            //    displayController.UpdateDisplay(
            //        BatteryVoltage: BatteryVoltage,
            //        SolarVoltage: SolarVoltage,
            //        Temperature: gnssTracker.TemperatureSensor.Temperature,
            //        Humidity: gnssTracker.HumiditySensor.Humidity,
            //        Pressure: gnssTracker.BarometricPressureSensor.Pressure,
            //        Concentration: gnssTracker.CO2ConcentrationSensor.CO2Concentration,
            //        locationInfo: lastGNSSPosition);

            //    await Task.Delay(sensorUpdateInterval);
            //}

            return Task.CompletedTask;
        }
    }
}