using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;

namespace WildernessLabs.Hardware.GnssTracker
{
    /// <summary>
    /// Represents a GNSS Tracker Interface
    /// </summary>
    public interface IGnssTrackerHardware
    {
        /// <summary>
        /// Gets the PWM LED
        /// </summary>
        public PwmLed? OnboardLed { get; }

        /// <summary>
        /// Gets the AtmosphericSensor sensor
        /// </summary>
        public Bme688? AtmosphericSensor { get; }

        /// <summary>
        /// Gets the Neo GNSS sensor
        /// </summary>
        public NeoM8? Gnss { get; }

        /// <summary>
        /// Gets the e-paper display
        /// </summary>
        public IGraphicsDisplay? Display { get; }

        /// <summary>
        /// Gets the Solar Voltage Input
        /// </summary>
        public IAnalogInputPort? SolarVoltageInput { get; }
    }
}