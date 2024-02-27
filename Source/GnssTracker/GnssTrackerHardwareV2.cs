using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Environmental;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors.Environmental;
using Meadow.Peripherals.Sensors.Motion;
using System;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Gnss Tracker Hardware V2
    /// </summary>
    public class GnssTrackerHardwareV2 : GnssTrackerHardwareBase
    {
        private IRgbPwmLed? _onboardRgbLed;

        private Scd40? scd40;
        private ICO2ConcentrationSensor? cO2ConcentrationSensor;

        private Bmi270? bmi270;
        private IGyroscope? gyroscope;
        private IAccelerometer? accelerometer;
        private IAnalogInputPort? batteryVoltageInput;

        /// <inheritdoc/>
        public sealed override II2cBus I2cBus { get; }

        /// <inheritdoc/>
        public override IRgbPwmLed? OnboardRgbLed => GetOnboardRgbLed();

        /// <inheritdoc/>
        public Scd40? Scd40 => GetScd40Sensor();

        /// <inheritdoc/>
        public override ICO2ConcentrationSensor? CO2ConcentrationSensor => GetCO2ConcentrationSensor();

        /// <inheritdoc/>
        public Bmi270? Bmi270 => GetBmi270Sensor();

        /// <inheritdoc/>
        public override IGyroscope? Gyroscope => GetGyroscope();

        /// <inheritdoc/>
        public override IAccelerometer? Accelerometer => GetAccelerometer();

        /// <inheritdoc/>
        public override IAnalogInputPort? BatteryVoltageInput => GetBatteryVoltage();

        /// <summary>
        /// Create a new GnssTrackerHardwareV2 object
        /// </summary>
        /// <param name="device">The Meadow device</param>
        /// <param name="i2cBus">The I2C bus</param>
        public GnssTrackerHardwareV2(IF7CoreComputeMeadowDevice device, II2cBus i2cBus)
        {
            base.device = device;

            I2cBus = i2cBus;

            // TODO: Workaround for https://github.com/WildernessLabs/Meadow_Issues/issues/293
            InitializeOnboardRgbLed();
        }

        private IRgbPwmLed? GetOnboardRgbLed()
        {
            if (_onboardRgbLed == null)
            {
                InitializeOnboardRgbLed();
            }

            return _onboardRgbLed;
        }

        private void InitializeOnboardRgbLed()
        {
            try
            {
                Logger?.Debug("Onboard LED Initializing...");

                _onboardRgbLed = new RgbPwmLed(
                    redPwmPin: device.Pins.D09,
                    greenPwmPin: device.Pins.D10,
                    bluePwmPin: device.Pins.D11);

                Logger?.Debug("Onboard LED initialized");
            }
            catch (Exception e)
            {
                Logger?.Error($"Err initializing onboard LED: {e.Message}");
            }
        }

        private Scd40? GetScd40Sensor()
        {
            if (scd40 == null)
            {
                InitializeScd40();
            }

            return scd40;
        }

        private ICO2ConcentrationSensor? GetCO2ConcentrationSensor()
        {
            if (cO2ConcentrationSensor == null)
            {
                InitializeScd40();
            }

            return cO2ConcentrationSensor;
        }

        private void InitializeScd40()
        {
            try
            {
                Logger?.Trace("SCD40 Initializing...");
                var scd = new Scd40(I2cBus, (byte)Scd40.Addresses.Default);
                scd40 = scd;
                cO2ConcentrationSensor = scd;
                Resolver.SensorService.RegisterSensor(scd);

                Logger?.Trace("SCD40 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the SCD40 IMU: {ex.Message}");
            }
        }

        private Bmi270? GetBmi270Sensor()
        {
            if (bmi270 == null)
            {
                InitializeBmi270();
            }

            return bmi270;
        }

        private IGyroscope? GetGyroscope()
        {
            if (gyroscope == null)
            {
                InitializeBmi270();
            }

            return gyroscope;
        }

        private IAccelerometer? GetAccelerometer()
        {
            if (accelerometer == null)
            {
                InitializeBmi270();
            }

            return accelerometer;
        }

        private void InitializeBmi270()
        {
            try
            {
                Logger?.Trace("BMI270 Initializing...");

                var bmi = new Bmi270(I2cBus);
                bmi270 = bmi;
                gyroscope = bmi;
                accelerometer = bmi;
                Resolver.SensorService.RegisterSensor(bmi);

                Logger?.Trace("BMI270 Initialized");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create the BMI270 IMU: {ex.Message}");
            }
        }

        private IAnalogInputPort? GetBatteryVoltage()
        {
            if (batteryVoltageInput == null)
            {
                InitializeBatteryVoltage();
            }

            return batteryVoltageInput;
        }

        private void InitializeBatteryVoltage()
        {
            try
            {
                Logger?.Debug("Battery Voltage Input Instantiating...");
                batteryVoltageInput = device.Pins.A04.CreateAnalogInputPort(5);
                Logger?.Debug("Battery Voltage Input up");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create Battery Voltage Input: {ex.Message}");
            }
        }
    }
}