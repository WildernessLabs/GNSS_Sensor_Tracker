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

        private Scd40? _scd40;
        private ICO2ConcentrationSensor? _cO2ConcentrationSensor;

        private Bmi270? _bmi270;
        private IGyroscope? _gyroscope;
        private IAccelerometer? _accelerometer;
        private IAnalogInputPort? _batteryVoltageInput;

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
            _device = device;

            I2cBus = i2cBus;
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
                    redPwmPin: _device.Pins.D09,
                    greenPwmPin: _device.Pins.D10,
                    bluePwmPin: _device.Pins.D11);

                Logger?.Debug("Onboard LED initialized");
            }
            catch (Exception e)
            {
                Logger?.Error($"Err initializing onboard LED: {e.Message}");
            }
        }

        private Scd40? GetScd40Sensor()
        {
            if (_scd40 == null)
            {
                InitializeScd40();
            }

            return _scd40;
        }

        private ICO2ConcentrationSensor? GetCO2ConcentrationSensor()
        {
            if (_cO2ConcentrationSensor == null)
            {
                InitializeScd40();
            }

            return _cO2ConcentrationSensor;
        }

        private void InitializeScd40()
        {
            try
            {
                Logger?.Trace("SCD40 Initializing...");

                //try to write a byte to the address of the SCD40 sensor
                I2cBus.Write(0x62, new byte[] { 0 });
                var scd = new Scd40(I2cBus, (byte)Scd40.Addresses.Default);
                var serialNum = scd.GetSerialNumber();
                Resolver.Log.Info($"Serial: {BitConverter.ToString(serialNum)}");
                _scd40 = scd;
                _cO2ConcentrationSensor = scd;
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
            if (_bmi270 == null)
            {
                InitializeBmi270();
            }

            return _bmi270;
        }

        private IGyroscope? GetGyroscope()
        {
            if (_gyroscope == null)
            {
                InitializeBmi270();
            }

            return _gyroscope;
        }

        private IAccelerometer? GetAccelerometer()
        {
            if (_accelerometer == null)
            {
                InitializeBmi270();
            }

            return _accelerometer;
        }

        private void InitializeBmi270()
        {
            try
            {
                Logger?.Trace("BMI270 Initializing...");

                var bmi = new Bmi270(I2cBus);
                _bmi270 = bmi;
                _gyroscope = bmi;
                _accelerometer = bmi;
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
            if (_batteryVoltageInput == null)
            {
                InitializeBatteryVoltage();
            }

            return _batteryVoltageInput;
        }

        private void InitializeBatteryVoltage()
        {
            try
            {
                Logger?.Debug("Battery Voltage Input Instantiating...");
                _batteryVoltageInput = _device.Pins.A04.CreateAnalogInputPort(5);
                Logger?.Debug("Battery Voltage Input up");
            }
            catch (Exception ex)
            {
                Logger?.Error($"Unable to create Battery Voltage Input: {ex.Message}");
            }
        }
    }
}