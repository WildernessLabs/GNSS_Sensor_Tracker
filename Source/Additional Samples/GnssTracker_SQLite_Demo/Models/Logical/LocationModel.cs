using Meadow.Peripherals.Sensors.Location.Gnss;

namespace GnssTracker_SQLite_Demo.Models.Logical
{
    public class LocationModel
    {
        public GnssPositionInfo? PositionInformation { get; set; }
    }
}