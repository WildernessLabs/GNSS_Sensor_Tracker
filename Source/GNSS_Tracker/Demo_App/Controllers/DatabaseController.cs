﻿using Meadow;
using SQLite;
using System.IO;
using Meadow.Logging;
using Meadow.GnssTracker.Core.Models.Data;
using Meadow.GnssTracker.Core.Models.Logical;

namespace Demo_App.Controllers
{
    public static class DatabaseController
    {
        public static SQLiteConnection Database { get; set; }
        public static Logger Log { get => Resolver.Log; }

        public static void ConfigureDatabase()
        {
            // by default, SQLite runs in `Serialized` mode, which is thread safe.
            // if you need to change the threading mode, you can do it with the
            // following API
            //SQLite3.Config(SQLite3.ConfigOption.SingleThread);

            // database files should go in the `DataDirectory`
            var databasePath = Path.Combine(MeadowOS.FileSystem.DataDirectory, "SensorReadings.db");

            // debug only:
            Log?.Info("Deleting old db.");
            File.Delete(databasePath);
            Log?.Info("Deleted.");

            Database = new SQLiteConnection(databasePath);
            Database.CreateTable<SensorDataModel>();
        }

        /// <summary>
        /// Saves the atmospheric conditions to the database
        /// </summary>
        /// <param name="conditions"></param>
        public static void SaveAtmosphericConditions(AtmosphericModel conditions)
        {
            var dataModel = SensorDataModel.From(conditions);

            Log.Info("Saving conditions to database.");
            Database.Insert(dataModel);
            Log.Info("Saved to database.");

            RetrieveData();
        }

        public static void SaveLocationInfo(LocationModel location)
        {
            var dataModel = SensorDataModel.From(location);

            Log.Info("Saving location info to database.");
            Database.Insert(dataModel);
            Log.Info("Saved to database.");

            RetrieveData();
        }

        /// <summary>
        /// retrieves the data from the database and reads them out to the console
        /// </summary>
        public static void RetrieveData()
        {
            Log.Info("Reading back the data...");
            var rows = Database.Table<SensorDataModel>();

            foreach (var r in rows)
            {
                Log.Info($"Reading was {r.TemperatureC:N2}C, @ {r.Latitude}/{r.Longitude} - {r.Timestamp.ToString("HH:mm:ss")} @");
            }
        }
    }
}