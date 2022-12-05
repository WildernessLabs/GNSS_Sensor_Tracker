using System;
using System.Threading.Tasks;
using Demo_App.Controllers;
using Meadow;
using Meadow.Devices;
using Meadow.GnssTracker.Core;
using Meadow.Logging;

namespace Demo_App
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        protected GnssTrackerHardware Hardware { get; set; }
        protected Logger Log { get => Resolver.Log; }
        protected MainTrackerController MainController { get; set; }

        public override Task Initialize()
        {
            Log.Info("Initialize hardware...");

            //==== Bring up the hardware
            Hardware = new GnssTrackerHardware(Device);

            //==== Bring up all the controllers
            //---- Database
            try { DatabaseController.ConfigureDatabase(); }
            catch (Exception e) { Log.Info($"Err bringing up database: {e.Message}"); }

            //---- Display Controller
            DisplayController.Initialize(Hardware.EPaperDisplay);

            //---- GNSS Controller
            GnssController.Initialize(Hardware.Gnss);

            //---- Main Tracker Controller (ties everything together)
            this.MainController = new MainTrackerController(Hardware);

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Running");

            //---- start the engines
            this.MainController.Start();

            return base.Run();
        }
    }
}