using System;
using Meadow.Units;

namespace Meadow.GnssTracker.Core.Models
{
    public class TrackingModel
    {
        public DateTime? Timestamp { get; set; }
        public Temperature? Temperature { get; set; }
        public RelativeHumidity? RelativeHumidity { get; set; }
        public Pressure? Pressure { get; set; }
    }
}