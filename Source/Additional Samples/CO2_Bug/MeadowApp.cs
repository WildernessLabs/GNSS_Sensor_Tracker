using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;

namespace CO2_Bug;

public class MeadowApp : App<F7CoreComputeV2>
{
    IGnssTrackerHardware hardware;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        hardware = GnssTracker.Create();

        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        Resolver.Log.Info("Run...");

        while (true)
        {
            Resolver.Log.Info("Hello, Meadow Core-Compute!");

            if (hardware.CO2ConcentrationSensor is { } co2Sensor)
            {
                var co2 = await co2Sensor.Read();
                Resolver.Log.Info($"co2 = {co2.PartsPerMillion}");
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}