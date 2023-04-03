using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Gnss;

namespace WildernessLabs.Hardware.GnssTracker
{
    public interface IGnssTrackerHardware
    {
        public PwmLed? OnboardLed { get; }
        public Bme688? AtmosphericSensor { get; }
        public NeoM8? Gnss { get; }
        public IGraphicsDisplay Display { get; }
    }
}