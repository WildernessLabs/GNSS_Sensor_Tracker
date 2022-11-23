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
                bme.StartUpdating(TimeSpan.FromSeconds(20));
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
                    //Log.Info("*********************************************");
                    //Log.Info(location);
                    //Log.Info("*********************************************");
                };
                // GSA
                gnss.GsaReceived += (object sender, ActiveSatellites activeSatellites) =>
                {
                    //Log.Info("*********************************************");
                    //Log.Info(activeSatellites);
                    //Log.Info("*********************************************");
                };
                // RMC (recommended minimum)
                gnss.RmcReceived += (object sender, GnssPositionInfo positionCourseAndTime) =>
                {
                    //Log.Info("*********************************************");
                    //Log.Info(positionCourseAndTime);
                    //Log.Info("*********************************************");

                };
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
                    //Log.Info("*********************************************");
                    //Log.Info($"{satellites}");
                    //Log.Info("*********************************************");
                };

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

        void Bme_Updated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> result)
        {
            Log.Info($"  Temperature: {result.New.Temperature?.Celsius:N2}C,");
            Log.Info($"  Relative Humidity: {result.New.Humidity:N2}%, ");
            Log.Info($"  Pressure: {result.New.Pressure?.Millibar:N2}mbar ({result.New.Pressure?.Pascal:N2}Pa)");

            TrackingModel conditions = new TrackingModel
            {
                Temperature = result.New.Temperature,
                RelativeHumidity = result.New.Humidity,
                Pressure = result.New.Pressure,
                Timestamp = DateTime.Now
            };

            DisplayController.UpdateConditions(conditions);
            SaveConditions(conditions);
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
