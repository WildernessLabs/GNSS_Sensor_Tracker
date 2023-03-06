using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Gnss;

namespace Meadow.GnssTracker.Core.Contracts
{
    public interface IGnssTrackerHardware
    {
        public PwmLed OnboardLed { get; }
        public Bme688 AtmosphericSensor { get; }
        //TODO: make an IGnssReceiver or something
        public NeoM8 Gnss { get; }
        public IGraphicsDisplay Display { get; }
    }
}