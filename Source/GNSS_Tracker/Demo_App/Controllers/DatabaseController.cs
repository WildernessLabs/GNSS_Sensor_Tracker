using Meadow;
using SQLite;
using System.IO;
using Meadow.Logging;
using GnssTracker_Demo.Models.Data;
using GnssTracker_Demo.Models.Logical;

namespace GnssTracker_Demo.Controllers
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
            Log?.Debug("Deleting old db.");
            File.Delete(databasePath);
            Log?.Debug("Deleted.");

            Database = new SQLiteConnection(databasePath);
            Database.CreateTable<SensorDataModel>();
        }

        /// <summary>
        /// Saves the atmospheric conditions to the database
        /// </summary>
        /// <param name="conditions"></param>
        public static void SaveAtmosphericLocations(AtmosphericModel conditions, LocationModel location)
        {
            var dataModel = SensorDataModel.From(conditions, location);

            Log.Debug("Saving conditions to database.");
            Database.Insert(dataModel);
            Log.Debug("Saved to database.");

            RetrieveData();
        }

        /// <summary>
        /// retrieves the data from the database and reads them out to the console
        /// </summary>
        public static void RetrieveData()
        {
            Log.Debug("Reading back the data...");
            var rows = Database.Table<SensorDataModel>();

            foreach (var r in rows)
            {
                Log.Debug($"{r.TemperatureC:N2}C, @ {r.Latitude}/{r.Longitude} - {r.Timestamp.ToString("HH:mm:ss")} @");
            }
        }
    }
}