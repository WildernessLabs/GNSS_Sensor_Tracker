using System;
using Meadow.Units;

namespace Meadow.GnssTracker.Core.Models.Logical
{
    public class AtmosphericModel
    {
        public DateTime? Timestamp { get; set; }
        public Temperature? Temperature { get; set; }
        public RelativeHumidity? RelativeHumidity { get; set; }
        public Pressure? Pressure { get; set; }

        public void Update(AtmosphericModel model)
        {
            if (model.Timestamp is { } time) { this.Timestamp = time; }
            if (model.Temperature is { } temp) { this.Temperature = temp; }
            if (model.RelativeHumidity is { } humidity) { this.RelativeHumidity = humidity; }
            if (model.Pressure is { } pressure) { this.Pressure = Pressure; }
        }
    }
}