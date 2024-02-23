using Meadow;
using Meadow.Devices;
using Power_Monitor.Controllers;
using System.Threading.Tasks;

namespace Power_Monitor
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var gnssTracker = GnssTracker.Create();

            var displayController = new DisplayController(gnssTracker.Display);

            return Task.CompletedTask;
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            Resolver.Log.Info("Hello, Meadow Core-Compute!");

            return Task.CompletedTask;
        }
    }
}