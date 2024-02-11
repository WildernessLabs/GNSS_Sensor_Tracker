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

            GnssController.Initialize(gnssTracker.Gnss);
            GnssController.GnssPositionInfoUpdated += GnssPositionInfoUpdated;
        }

        public void Run()
        {
            if (GnssTracker.AtmosphericSensor is { } bme)
            {
                bme.Updated += AtmosphericSensorUpdated;
                bme.StartUpdating(UPDATE_INTERVAL);
            }

            GnssController.StartUpdating();
        }

        private void GnssPositionInfoUpdated(object sender, GnssPositionInfo result)
        {
            LastLocationInfo.PositionInformation = result;

            if (result.Position is { } pos)
            {
                if (pos.Latitude is { } lat && pos.Longitude is { } lon)
                {
                    Log.Debug($"RM: lat: [{pos.Latitude}], long: [{pos.Longitude}]");
                }
                else
                {
                    Log.Debug("RM Position lat/long empty.");
                }
            }
            else
            {
                Log.Debug("RM Position not yet found.");
            }

            if (result.TimeOfReading is { } timeOfReading)
            {
                //Log.Info($"UTC DateTime from GPS: {timeOfReading.ToShortDateString()} :: {timeOfReading.ToShortTimeString()}");
                //Log.Info($"Device DateTime: {DateTime.Now.ToShortDateString()} :: {DateTime.Now.ToShortTimeString()}");

                if (timeOfReading.Date != DateTime.Now.Date)
                {
                    Resolver.Device.PlatformOS.SetClock(timeOfReading);
                }
            }
        }

        private void AtmosphericSensorUpdated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> result)
        {
            Log.Info($"BME688 - Temperature: {result.New.Temperature?.Celsius:N2}C,");
            Log.Info($"BME688 - Humidity:    {result.New.Humidity:N2}%, ");
            Log.Info($"BME688 - Pressure:    {result.New.Pressure?.Millibar:N2}mbar ({result.New.Pressure?.StandardAtmosphere:N2}atm)");

            var newConditions = new AtmosphericModel
            {
                Temperature = result.New.Temperature,
                RelativeHumidity = result.New.Humidity,
                Pressure = result.New.Pressure,
                Timestamp = DateTime.Now
            };
            LastAtmosphericConditions.Update(newConditions);

            DatabaseController.SaveAtmosphericLocations(LastAtmosphericConditions, LastLocationInfo);
            DisplayController.UpdateDisplay(LastAtmosphericConditions, LastLocationInfo);
        }
    }
}