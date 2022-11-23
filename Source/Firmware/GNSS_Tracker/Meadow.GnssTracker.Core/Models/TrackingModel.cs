using System;
using Meadow.Peripherals.Sensors.Location.Gnss;
using Meadow.Units;

namespace Meadow.GnssTracker.Core.Models
{
    public class TrackingModel
    {
        public DateTime? Timestamp { get; set; }
        public Temperature? Temperature { get; set; }
        public RelativeHumidity? RelativeHumidity { get; set; }
        public Pressure? Pressure { get; set; }
        public GnssPositionInfo? PositionCourseAndTime { get; set; }

        //public TrackingModel() { }

        //public TrackingModel(TrackingModel model)
        //{
        //    if (model.Timestamp is { } time) { this.Timestamp = time; }
        //    if (model.Temperature is { } temp) { this.Temperature = temp; }
        //    if (model.RelativeHumidity is { } humidity) { this.RelativeHumidity = humidity; }
        //    if (model.Pressure is { } pressure) { this.Pressure = Pressure; }
        //    if (model.PositionCourseAndTime is { } position) { this.PositionCourseAndTime = position; }
        //}

        public void Update(TrackingModel model)
        {
            if (model.Timestamp is { } time) { this.Timestamp = time; }
            if (model.Temperature is { } temp) { this.Temperature = temp; }
            if (model.RelativeHumidity is { } humidity) { this.RelativeHumidity = humidity; }
            if (model.Pressure is { } pressure) { this.Pressure = Pressure; }
            if (model.PositionCourseAndTime is { } position) { this.PositionCourseAndTime = position; }
        }
    }
}