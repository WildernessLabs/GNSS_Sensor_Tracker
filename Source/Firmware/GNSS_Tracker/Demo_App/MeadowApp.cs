using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.GnssTracker.Core;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Location.Gnss;
using SQLite;

namespace Demo_App
{
	// Change F7FeatherV2 to F7FeatherV1 for V1.x boards
	public class MeadowApp : App<F7CoreComputeV2>
	{
        //PwmLed onboardLed;
        protected GnssTrackerHardware Hardware { get; set; }

        public override Task Initialize()
        {
            //Console.WriteLine("Initialize hardware...");
            //onboardLed = new PwmLed(device: Device, Device.Pins.D20, TypicalForwardVoltage.Green);

            //==== Bring up the hardware
            Hardware = new GnssTrackerHardware(Device);

            //==== Bring up our database
            try
            {
                // configure the database and open a connection
                DatabaseController.ConfigureDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Err bringing up database: {e.Message}");
            }

            //==== Initialize Display Controller
            DisplayController.Initialize(Hardware.EPaperDisplay);

            return base.Initialize();
        }

        public override Task Run()
        {
            Console.WriteLine("Running");

            Hardware.OnboardLed.StartPulse();

            if (Hardware.Bme68X is { } bme) {
                bme.Updated += Bme_Updated;
                bme.StartUpdating(TimeSpan.FromSeconds(20));
            }

            if (Hardware.Gnss is { } gnss) {
                gnss.GgaReceived += (object sender, GnssPositionInfo location) => {
                    Console.WriteLine("*********************************************");
                    Console.WriteLine(location);
                    Console.WriteLine("*********************************************");
                };
                // GLL
                gnss.GllReceived += (object sender, GnssPositionInfo location) => {
                    Console.WriteLine("*********************************************");
                    Console.WriteLine(location);
                    Console.WriteLine("*********************************************");
                };
                // GSA
                gnss.GsaReceived += (object sender, ActiveSatellites activeSatellites) => {
                    Console.WriteLine("*********************************************");
                    Console.WriteLine(activeSatellites);
                    Console.WriteLine("*********************************************");
                };
                // RMC (recommended minimum)
                gnss.RmcReceived += (object sender, GnssPositionInfo positionCourseAndTime) => {
                    Console.WriteLine("*********************************************");
                    Console.WriteLine(positionCourseAndTime);
                    Console.WriteLine("*********************************************");

                };
                // VTG (course made good)
                gnss.VtgReceived += (object sender, CourseOverGround courseAndVelocity) => {
                    Console.WriteLine("*********************************************");
                    Console.WriteLine($"{courseAndVelocity}");
                    Console.WriteLine("*********************************************");
                };
                // GSV (satellites in view)
                gnss.GsvReceived += (object sender, SatellitesInView satellites) => {
                    Console.WriteLine("*********************************************");
                    Console.WriteLine($"{satellites}");
                    Console.WriteLine("*********************************************");
                };

                gnss.StartUpdating();
            }

            return base.Run();
        }

        void Bme_Updated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> result)
        {
            Console.Write($"  Temperature: {result.New.Temperature?.Celsius:N2}C,");
            Console.Write($"  Relative Humidity: {result.New.Humidity:N2}%, ");
            Console.WriteLine($"  Pressure: {result.New.Pressure?.Millibar:N2}mbar ({result.New.Pressure?.Pascal:N2}Pa)");

            TrackingModel trackingModel = new TrackingModel()
            {
                TemperatureC = result.New.Temperature?.Celsius,
                RelativeHumdityPercent = result.New.Humidity?.Percent,
                PressureAtmos = result.New.Pressure?.StandardAtmosphere
            };

            DatabaseController.Database.Insert(trackingModel);
            Console.WriteLine("Saved sensor reading to database.");

            RetreiveData();

            DisplayController.UpdateConditions(result);
        }

        void RetreiveData()
        {
            Console.WriteLine("Reading back the data...");
            var rows = DatabaseController.Database.Table<TrackingModel>();
            foreach (var r in rows)
            {
                Console.WriteLine($"Reading was {r.TemperatureC:N2}C at {r.Timestamp.ToString("HH:mm:ss")}");
            }
        }
    }
}
