using System;
using SQLite;

namespace Meadow.GnssTracker.Core.Models
{
    [Table("SensorReadings")]
    public class TrackingDataModel
    {
        public TrackingDataModel() { }

        public TrackingDataModel(TrackingModel model)
        {
            this.TemperatureC = model.Temperature?.Celsius;
            this.RelativeHumidityPercent = model.RelativeHumidity?.Percent;
            this.PressureAtmos = model.Pressure?.StandardAtmosphere;
            this.Timestamp = model.Timestamp.Value;
            // TODO: figure out how we want to save this data
            //this.Position = model.PositionCourseAndTime.Position.ToString();
        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public double? TemperatureC { get; set; }
        public double? RelativeHumidityPercent { get; set; }
        public double? PressureAtmos { get; set; }
        public string? Position { get; set; }
    }
}