using System;
using SQLite;

namespace Meadow.GnssTracker.Core.Models.Data
{
    [Table("SensorReadings")]
    public class SensorDataModel
    {
        //==== properties
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public double? TemperatureC { get; set; }
        public double? RelativeHumidityPercent { get; set; }
        public double? PressureAtmos { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        //public string? Altitude { get; set; }

        //==== ctors
        public SensorDataModel() { }

        // From Atmospheric model
        public static SensorDataModel From(Logical.AtmosphericModel model)
        {
            var dataModel = new SensorDataModel
            {
                TemperatureC = model.Temperature?.Celsius,
                RelativeHumidityPercent = model.RelativeHumidity?.Percent,
                PressureAtmos = model.Pressure?.StandardAtmosphere,
                Timestamp = model.Timestamp.Value,
            };

            return dataModel;
        }

        // From Location model
        public static SensorDataModel From(Logical.LocationModel model)
        {
            var dataModel = new SensorDataModel
            {
                Latitude = $"{model.PositionInformation?.Position?.Latitude?.Degrees} {model.PositionInformation?.Position?.Latitude?.Minutes}'{model.PositionInformation?.Position?.Latitude?.seconds}\"",
                Longitude = $"{model.PositionInformation?.Position?.Longitude?.Degrees} {model.PositionInformation?.Position?.Longitude?.Minutes}'{model.PositionInformation?.Position?.Longitude?.seconds}\"",
            };
            return dataModel;
        }
    }
}