using Meadow;
using Meadow.Devices;
using Meadow.GnssTracker.Core;
using Meadow.Peripherals.Sensors.Location.Gnss;
using System;
using System.Threading.Tasks;
using Meadow.Logging;
using Meadow.GnssTracker.Core.Models;
using static SQLite.SQLite3;

namespace Demo_App
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        protected GnssTrackerHardware Hardware { get; set; }
        protected Logger Log { get => Resolver.Log; }
        protected TrackingModel? CurrentConditions { get; set; } 

        public override Task Initialize()
        {
            Log.Info("Initialize hardware...");

            //==== Bring up the hardware
            Hardware = new GnssTrackerHardware(Device);

            //==== Bring up our database
            try
            {
                DatabaseController.ConfigureDatabase();
            }
            catch (Exception e)
            {
                Log.Info($"Err bringing up database: {e.Message}");
            }

            //==== Initialize Display Controller
            DisplayController.Initialize(Hardware.EPaperDisplay);

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Running");

            Hardware.OnboardLed.StartPulse();

            if (Hardware.AtmosphericSensor is { } bme)
            {
                bme.Updated += Bme_Updated;
                bme.StartUpdating(TimeSpan.FromSeconds(10));
            }

            if (Hardware.Gnss is { } gnss)
            {
                Resolver.Log.Debug("Starting GNSS");

                gnss.GgaReceived += (object sender, GnssPositionInfo location) =>
                {
                    //                    Log.Info("*********************************************");
                    //                    Log.Info(location);
                    //                    Log.Info("*********************************************");
                };
                // GLL
                gnss.GllReceived += (object sender, GnssPositionInfo location) =>
                {
                    Log.Info($"GNSS Position: lat: [{location.Position.Latitude}], long: [{location.Position.Longitude}]");
                };
                // GSA
                gnss.GsaReceived += (object sender, ActiveSatellites activeSatellites) =>
                {
                    if (activeSatellites.SatellitesUsedForFix is { } sats) {
                        Log.Info($"Number of active satellites: {sats.Length}");
                    }
                };
                // RMC (recommended minimum)
                gnss.RmcReceived += Gnss_RmcReceived;

                // VTG (course made good)
                gnss.VtgReceived += (object sender, CourseOverGround courseAndVelocity) =>
                {
                    //Log.Info("*********************************************");
                    //Log.Info($"{courseAndVelocity}");
                    //Log.Info("*********************************************");
                };
                // GSV (satellites in view)
                gnss.GsvReceived += (object sender, SatellitesInView satellites) =>
                {
                    Log.Info($"Satellites in view: {satellites.Satellites.Length}");
                };

                //---- start updating the GNSS receiver
                try
                {
                    gnss.StartUpdating();
                }
                catch (Exception ex)
                {
                    Resolver.Log.Error($"{ex.Message}");
                }
            }

            return base.Run();
        }

        private void Gnss_RmcReceived(object sender, GnssPositionInfo positionCourseAndTime)
        {
            if (positionCourseAndTime.Position is { } pos)
            {
                if (pos.Latitude is { } lat && pos.Longitude is { } lon)
                {
                    Log.Info($"RM: lat: [{pos.Latitude}], long: [{pos.Longitude}]");

                    //TODO: consider updating system time
                    if (positionCourseAndTime.TimeOfReading is { } timeOfReading)
                    {
                        //Resolver.Device.SetClock(timeOfReading);
                        Log.Info($"UTC DateTime from GPS: {timeOfReading.ToShortDateString()} :: {timeOfReading.ToShortTimeString()}");
                        Log.Info($"Device DateTime: {DateTime.Now.ToShortDateString()} :: {DateTime.Now.ToShortTimeString()}");

                        if (timeOfReading.Date != DateTime.Now.Date) {
                            Log.Info($"Device date is different than GPS time. Updating.");
                            Resolver.Device.SetClock(timeOfReading);
                            Log.Info($"Device time set. {DateTime.Now.ToShortDateString()}");
                        }
                    }

                    //---- update CurrentConditions
                    if (CurrentConditions == null) { this.CurrentConditions = new TrackingModel(); }
                    var newConditions = new TrackingModel
                    {
                        PositionCourseAndTime = positionCourseAndTime
                        //TODO: set time:
                        //Timestamp = positionCourseAndTime.TimeOfReading
                    };
                    this.CurrentConditions.Update(newConditions);

                    //---- update display and save to database
                    DisplayController.UpdateConditions(this.CurrentConditions);
                    //TODO:
                    //SaveConditions(this.CurrentConditions);

                }
                else { Log.Info("RM Position lat/long empty."); }
            }
            else { Log.Info("RM Position not yet found."); }
        }

        void Bme_Updated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> result)
        {
            Log.Info($"  Temperature: {result.New.Temperature?.Celsius:N2}C,");
            Log.Info($"  Relative Humidity: {result.New.Humidity:N2}%, ");
            Log.Info($"  Pressure: {result.New.Pressure?.Millibar:N2}mbar ({result.New.Pressure?.Pascal:N2}Pa)");

            //---- update CurrentConditions
            if (CurrentConditions == null) { this.CurrentConditions = new TrackingModel(); }
            var newConditions = new TrackingModel
            {
                Temperature = result.New.Temperature,
                RelativeHumidity = result.New.Humidity,
                Pressure = result.New.Pressure,
                Timestamp = DateTime.Now
            };
            this.CurrentConditions.Update(newConditions);

            //---- update display and save to database
            DisplayController.UpdateConditions(this.CurrentConditions);
            SaveConditions(this.CurrentConditions);
        }

        protected void SaveConditions(TrackingModel conditions)
        {
            TrackingDataModel trackingModel = new TrackingDataModel(conditions);

            Log.Info("Saving conditions to database.");
            DatabaseController.Database.Insert(trackingModel);
            Log.Info("Saved sensor reading to database.");

            RetreiveData();
        }

        void RetreiveData()
        {
            Log.Info("Reading back the data...");
            var rows = DatabaseController.Database.Table<TrackingDataModel>();
            foreach (var r in rows)
            {
                Log.Info($"Reading was {r.TemperatureC:N2}C at {r.Timestamp.ToString("HH:mm:ss")}");
            }
        }
    }
}
