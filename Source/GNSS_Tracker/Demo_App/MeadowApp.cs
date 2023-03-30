using System;
using System.Threading.Tasks;
using Demo_App.Controllers;
using Meadow;
using Meadow.Devices;
using Meadow.GnssTracker.Core;
using Meadow.GnssTracker.Core.Contracts;
using Meadow.Logging;

namespace Demo_App
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        protected IGnssTrackerHardware Hardware { get; set; }
        protected Logger Log { get => Resolver.Log; }
        protected MainTrackerController MainController { get; set; }

        public override Task Initialize()
        {
            Log.Info("Initialize hardware...");

            Hardware = new GnssTrackerV1Hardware();
            
            try 
            { 
                DatabaseController.ConfigureDatabase(); 
            }
            catch (Exception e) 
            { 
                Log.Info($"Err bringing up database: {e.Message}"); 
            }

            DisplayController.Initialize(Hardware.Display);

            GnssController.Initialize(Hardware.Gnss);

            MainController = new MainTrackerController(Hardware);

            return base.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Running");

            MainController.Start();

            return base.Run();
        }
    }
}