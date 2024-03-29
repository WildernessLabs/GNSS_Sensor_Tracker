﻿using GnssTracker_SQLite_Demo.Controllers;
using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;

namespace GnssTracker_SQLite_Demo
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        protected MainTrackerController MainController { get; set; }

        public override async Task Initialize()
        {
            Resolver.Log.Info("Initialize hardware...");

            var gnssTracker = GnssTracker.Create();

            try
            {
                DatabaseController.ConfigureDatabase();
            }
            catch (Exception e)
            {
                Resolver.Log.Info($"Err bringing up database: {e.Message}");
            }

            MainController = new MainTrackerController();
            await MainController.Initialize(gnssTracker);
        }

        public override Task Run()
        {
            Resolver.Log.Info("Running");

            MainController.Run();

            return base.Run();
        }
    }
}