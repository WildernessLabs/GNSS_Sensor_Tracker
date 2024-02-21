using GnssTracker_SQLite_Demo.Models.Logical;
using Meadow;
using Meadow.Devices;
using Meadow.Logging;
using Meadow.Peripherals.Sensors.Location.Gnss;
using System;
using System.Threading.Tasks;

namespace GnssTracker_SQLite_Demo.Controllers
{
    /// <summary>
    /// This is the main tracker application controller. It's responsible for
    /// orchestrating the entire operation of the application.
    /// </summary>
    public class MainTrackerController
    {
        GnssPositionInfo lastGNSSPosition;
        DateTime lastGNSSPositionReportTime = DateTime.MinValue;
        readonly TimeSpan GNSSPositionReportInterval = TimeSpan.FromSeconds(15);

        private TimeSpan UPDATE_INTERVAL = TimeSpan.FromMinutes(1);

        protected IGnssTrackerHardware GnssTracker { get; set; }

        protected Logger Log { get => Resolver.Log; }

        protected AtmosphericModel? LastAtmosphericConditions { get; set; }

        protected LocationModel? LastLocationInfo { get; set; }

        protected DisplayController DisplayController { get; set; }

        public MainTrackerController() { }

        public async Task Initialize(IGnssTrackerHardware gnssTracker)
        {
            GnssTracker = gnssTracker;

            LastLocationInfo = new LocationModel();
            LastAtmosphericConditions = new AtmosphericModel();

            if (gnssTracker.Display is { } display)
            {
                DisplayController = new DisplayController(display);
                await Task.Delay(TimeSpan.FromSeconds(20));
            }

            if (gnssTracker.Gnss is { } gnss)
            {
                gnss.RmcReceived += GnssRmcReceived;
                gnss.GllReceived += GnssGllReceived;
            }
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
                    Resolver.Log.Info($"GNSS POSITION: LAT: [{e.Position.Latitude}], LONG: [{e.Position.Longitude}]");

                    lastGNSSPositionReportTime = DateTime.UtcNow;
                }
            }
        }

        private void AtmosphericSensorUpdated()
        {
            Log.Info($"BME688 - Temperature: {GnssTracker.TemperatureSensor.Temperature.Value.Celsius:N2}C,");
            Log.Info($"BME688 - Humidity:    {GnssTracker.HumiditySensor.Humidity.Value.Percent:N2}%, ");
            Log.Info($"BME688 - Pressure:    {GnssTracker.BarometricPressureSensor.Pressure.Value.Millibar:N2}mbar ({GnssTracker.BarometricPressureSensor.Pressure.Value.StandardAtmosphere:N2}atm)");

            var newConditions = new AtmosphericModel
            {
                Temperature = GnssTracker.TemperatureSensor.Temperature,
                RelativeHumidity = GnssTracker.HumiditySensor.Humidity,
                Pressure = GnssTracker.BarometricPressureSensor.Pressure,
                Timestamp = DateTime.Now
            };
            LastAtmosphericConditions.Update(newConditions);

            DatabaseController.SaveAtmosphericLocations(LastAtmosphericConditions, LastLocationInfo);
            DisplayController.UpdateDisplay(LastAtmosphericConditions, LastLocationInfo);
        }

        public async void Run()
        {
            if (GnssTracker.TemperatureSensor is { } temperatureSensor)
            {
                temperatureSensor.StartUpdating(UPDATE_INTERVAL);
            }

            if (GnssTracker.BarometricPressureSensor is { } barometer)
            {
                barometer.StartUpdating(UPDATE_INTERVAL);
            }

            if (GnssTracker.HumiditySensor is { } humiditySensor)
            {
                humiditySensor.StartUpdating(UPDATE_INTERVAL);
            }

            if (GnssTracker.Gnss is { } gnss)
            {
                gnss.StartUpdating();
            }

            while (true)
            {
                AtmosphericSensorUpdated();

                await Task.Delay(UPDATE_INTERVAL);
            }
        }
    }
}