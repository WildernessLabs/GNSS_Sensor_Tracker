using GnssTracker_SQLite_Demo.Controllers;
using Meadow;
using Meadow.Devices;
using Meadow.Logging;
using System;
using System.Threading.Tasks;
using WildernessLabs.Hardware.GnssTracker;

namespace GnssTracker_SQLite_Demo
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        protected IGnssTrackerHardware Hardware { get; set; }
        protected Logger Log { get => Resolver.Log; }
        protected MainTrackerController MainController { get; set; }

        public override async Task Initialize()
        {
            Log.Info("Initialize hardware..."); //1 888 684-5548

            Hardware = GnssTracker.Create();

            try
            {
                DatabaseController.ConfigureDatabase();
            }
            catch (Exception e)
            {
                Log.Info($"Err bringing up database: {e.Message}");
            }

            DisplayController.Initialize(Hardware.Display);

            await Task.Delay(TimeSpan.FromSeconds(10));

            GnssController.Initialize(Hardware.Gnss);

            MainController = new MainTrackerController(Hardware);
        }

        public override Task Run()
        {
            Resolver.Log.Info("Running");

            MainController.Start();

            return base.Run();
        }
    }
}