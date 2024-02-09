using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;

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
        /// The SCD40 environmental sensor on the Clima board
        /// </summary>
        public Scd40? EnvironmentalSensor { get; }

        /// <summary>
        /// The BMI270 motion sensor on the Clima board
        /// </summary>
        public Bmi270? MotionSensor { get; }

        /// <summary>
        /// Gets the Neo GNSS sensor
        /// </summary>
        public NeoM8? Gnss { get; }

        /// <summary>
        /// Gets the e-paper display
        /// </summary>
        public IPixelDisplay? Display { get; }

        /// <summary>
        /// Gets the Solar Voltage Input
        /// </summary>
        public IAnalogInputPort? SolarVoltageInput { get; }

        /// <summary>
        /// Gets the Battery Voltage Input
        /// </summary>
        public IAnalogInputPort? BatteryVoltageInput { get; }

        /// <summary>
        /// Gets the I2C header connector
        /// </summary>
        public I2cConnector I2cHeader { get; }

        /// <summary>
        /// Gets the serial header connector
        /// </summary>
        public UartConnector UartHeader { get; }

        /// <summary>
        /// Gets the display header connector
        /// </summary>
        public DisplayConnector DisplayHeader { get; }
    }
}