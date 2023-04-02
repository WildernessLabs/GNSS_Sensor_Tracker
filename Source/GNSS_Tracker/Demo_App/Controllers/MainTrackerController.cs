using Meadow;
using Meadow.GnssTracker.Core.Contracts;
using Meadow.GnssTracker.Core.Models.Logical;
using Meadow.Logging;
using System;
using Meadow.Peripherals.Sensors.Location.Gnss;

namespace Demo_App.Controllers
{
    /// <summary>
    /// This is the main tracker application controller. It's responsible for
    /// orchestrating the entire operation of the application.
    /// </summary>
    public class MainTrackerController
    {
        private TimeSpan UPDATE_INTERVAL = TimeSpan.FromSeconds(30);

        protected Logger Log { get => Resolver.Log; }
        protected IGnssTrackerHardware Hardware { get; set; }
        protected AtmosphericModel? LastAtmosphericConditions { get; set; }
        protected LocationModel? LastLocationInfo { get; set; }

        public MainTrackerController(IGnssTrackerHardware hardware)
        {
            Hardware = hardware;

            LastLocationInfo = new LocationModel();
            LastAtmosphericConditions = new AtmosphericModel();

            GnssController.GnssPositionInfoUpdated += GnssPositionInfoUpdated;
        }

        /// <summary>
        /// Starts updating all the things
        /// </summary>
        public void Start()
        {
            Hardware.OnboardLed.StartPulse();

            if (Hardware.AtmosphericSensor is { } bme)
            {
                bme.Updated += AtmosphericSensorUpdated;
                bme.StartUpdating(UPDATE_INTERVAL);
            }

            GnssController.StartUpdating();
        }

        void GnssPositionInfoUpdated(object sender, GnssPositionInfo result)
        {
            LastLocationInfo.PositionInformation = result;

            if (result.Position is { } pos)
            {
                if (pos.Latitude is { } lat && pos.Longitude is { } lon)
                {
                    Log.Info($"RM: lat: [{pos.Latitude}], long: [{pos.Longitude}]");
                }
                else 
                { 
                    Log.Info("RM Position lat/long empty."); 
                }
            }
            else 
            { 
                Log.Info("RM Position not yet found."); 
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

        void AtmosphericSensorUpdated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> result)
        {
            Log.Info($"BME688: Temperature: {result.New.Temperature?.Celsius:N2}C,");
            Log.Info($"BME688: Relative Humidity: {result.New.Humidity:N2}%, ");
            Log.Info($"BME688: Pressure: {result.New.Pressure?.Millibar:N2}mbar ({result.New.Pressure?.StandardAtmosphere:N2}atm)");

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