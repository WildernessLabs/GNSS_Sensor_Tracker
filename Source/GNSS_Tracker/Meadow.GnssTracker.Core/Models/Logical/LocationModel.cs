using System;
using Meadow.Peripherals.Sensors.Location;
using Meadow.Peripherals.Sensors.Location.Gnss;

namespace Meadow.GnssTracker.Core.Models.Logical
{
    public class LocationModel
    {
        public GnssPositionInfo? PositionInformation { get; set; }
    }
}