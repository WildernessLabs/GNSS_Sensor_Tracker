using SQLite;
using System;

namespace GnssTracker_SQLite_Demo.Models.Data
{
    [Table("SensorReadings")]
    public class SensorDataModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public double? TemperatureC { get; set; }
        public double? RelativeHumidityPercent { get; set; }
        public double? PressureAtmos { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

        public SensorDataModel() { }

        public static SensorDataModel From(Logical.AtmosphericModel atmospheric, Logical.LocationModel location)
        {
            var dataModel = new SensorDataModel
            {
                TemperatureC = atmospheric.Temperature?.Celsius,
                RelativeHumidityPercent = atmospheric.RelativeHumidity?.Percent,
                PressureAtmos = atmospheric.Pressure?.StandardAtmosphere,
                Timestamp = atmospheric.Timestamp.Value,
                Latitude = $"{location.PositionInformation?.Position?.Latitude?.Degrees} {location.PositionInformation?.Position?.Latitude?.Minutes}'{location.PositionInformation?.Position?.Latitude?.seconds}\"",
                Longitude = $"{location.PositionInformation?.Position?.Longitude?.Degrees} {location.PositionInformation?.Position?.Longitude?.Minutes}'{location.PositionInformation?.Position?.Longitude?.seconds}\"",
            };

            return dataModel;
        }
    }
}