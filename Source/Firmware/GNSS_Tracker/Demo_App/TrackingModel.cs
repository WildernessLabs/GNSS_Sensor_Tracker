using System;
using SQLite;

namespace Demo_App
{
    [Table("SensorReadings")]
    public class TrackingModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public double? TemperatureC { get; set; }
        public double? RelativeHumdityPercent { get; set; }
        public double? PressureAtmos { get; set; }
    }
}