using Meadow;
using Meadow.Devices;
using Meadow.Units;
using Power_Monitor.Controllers;
using System;
using System.Threading.Tasks;

namespace Power_Monitor
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        readonly TimeSpan sensorUpdateInterval = TimeSpan.FromMinutes(5);

        private IGnssTrackerHardware gnssTracker;
        private DisplayController displayController;

        private Voltage? solarVoltage;
        private Voltage? batteryVoltage;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            gnssTracker = GnssTracker.Create();

            displayController = new DisplayController(gnssTracker.Display);

            if (gnssTracker.BatteryVoltageInput is { } batteryvoltage)
            {
                batteryvoltage.Updated += BatteryVoltageUpdated;
            }

            if (gnssTracker.SolarVoltageInput is { } solarVoltage)
            {
                solarVoltage.Updated += SolarVoltageUpdated;
            }

            return Task.CompletedTask;
        }

        private void BatteryVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            // Note: Battery Voltage input has a voltage divider, check schematics to learn more
            batteryVoltage = e.New * 1.60;
            Resolver.Log.Info($"BATTERY VOLTAGE:   {batteryVoltage:N2} volts");
        }

        private void SolarVoltageUpdated(object sender, IChangeResult<Voltage> e)
        {
            // Note: Solar Voltage input has a voltage divider, check schematics to learn more
            solarVoltage = e.New * 1.40;
            Resolver.Log.Info($"SOLAR VOLTAGE:     {solarVoltage:N2} volts");
        }

        public override async Task Run()
        {
            Resolver.Log.Info("Run...");

            if (gnssTracker.BatteryVoltageInput is { } batteryVoltageInput)
            {
                batteryVoltageInput.StartUpdating(sensorUpdateInterval);
            }

            if (gnssTracker.SolarVoltageInput is { } solarVoltageInput)
            {
                solarVoltageInput.StartUpdating(sensorUpdateInterval);
            }

            while (true)
            {
                if (batteryVoltage != null &&
                    solarVoltage != null)
                {
                    displayController.UpdateGraph(batteryVoltage.Value.Volts, solarVoltage.Value.Volts);

                    await Task.Delay(sensorUpdateInterval);
                }
                else
                {
                    Resolver.Log.Info("No readings yet, checking in 10s");

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}