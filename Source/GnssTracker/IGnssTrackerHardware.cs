using Meadow.Foundation.Sensors.Gnss;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Motion;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a GNSS Tracker Interface
    /// </summary>
    public interface IGnssTrackerHardware
    {
        /// <summary>
        /// Gets the PWM LED
        /// </summary>
        public IPwmLed? OnboardLed { get; }

        /// <summary>
        /// Gets the ITemperatureSensor on the Project Lab board.
        /// </summary>
        public ITemperatureSensor? TemperatureSensor { get; }

        /// <summary>
        /// Gets the IHumiditySensor on the Project Lab board.
        /// </summary>
        public IHumiditySensor? HumiditySensor { get; }

        /// <summary>
        /// Gets the IBarometricPressureSensor on the Project Lab board.
        /// </summary>
        public IBarometricPressureSensor? BarometricPressureSensor { get; }

        /// <summary>
        /// Gets the IGasResistanceSensor on the Project Lab board.
        /// </summary>
        public IGasResistanceSensor? GasResistanceSensor { get; }

        /// <summary>
        /// Gets the ICO2ConcentrationSensor on the Project Lab board
        /// </summary>
        public ICO2ConcentrationSensor? CO2ConcentrationSensor { get; }

        /// <summary>
        /// Gets the IGyroscope on the Project Lab board
        /// </summary>
        public IGyroscope? Gyroscope { get; }

        /// <summary>
        /// Gets the IAccelerometer on the Project Lab board
        /// </summary>
        public IAccelerometer? Accelerometer { get; }

        /// <summary>
        /// Gets the GNSS sensor
        /// </summary>
        public NeoM8? GnssSensor { get; }

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