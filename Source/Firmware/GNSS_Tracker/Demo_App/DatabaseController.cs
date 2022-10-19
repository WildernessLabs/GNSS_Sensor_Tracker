using System;
using Meadow;
using SQLite;
using System.IO;

namespace Demo_App
{
    public static class DatabaseController
    {
        public static SQLiteConnection Database { get; set; }


        public static void ConfigureDatabase()
        {
            Console.WriteLine("Bringing up database.");

            // by default, SQLite runs in `Serialized` mode, which is thread safe.
            // if you need to change the threading mode, you can do it with the
            // following API
            //SQLite3.Config(SQLite3.ConfigOption.SingleThread);

            // database files should go in the `DataDirectory`
            var databasePath = Path.Combine(MeadowOS.FileSystem.DataDirectory, "MySqliteDatabase.db");
            // make the connection
            Database = new SQLiteConnection(databasePath);
            // add table(s)
            Database.CreateTable<TrackingModel>();

            Console.WriteLine("Database up!");
        }

    }
}

