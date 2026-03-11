# Meadow.GnssTracker

**Convenience library for the Meadow GNSS Sensor Tracker**

The **GnssTracker** library is included in the **Meadow.GnssTracker** nuget package and is designed for the [Wilderness Labs](www.wildernesslabs.co) Meadow .NET IoT platform.

The GNSS Sensor Tracker is an open-source, sensor-rich, GNSS/GPS tracking board that provides a base design accelerator for custom Meadow-based GNSS tracking solutions.

For more information on developing for Meadow, visit [developer.wildernesslabs.co](http://developer.wildernesslabs.co/).

To view all Wilderness Labs open-source projects, including samples, visit [github.com/wildernesslabs](https://github.com/wildernesslabs/).

## Onboard Hardware

| Peripheral | Description |
|---|---|
| NEO-M8 | GNSS/GPS module with antenna connector |
| SSD1680 | 122x250 e-paper display |
| BME688 | Temperature, pressure, humidity, and gas resistance sensor |
| SCD40 | CO2, humidity, and temperature sensor |
| BMI270 | Inertial measurement unit / accelerometer |
| RGB LED | Onboard status LED |
| Solar input | 6V solar connector with battery charging |
| Battery | 3.7V LiPo rechargeable battery |

## Installation

You can install the library from within Visual Studio using the NuGet Package Manager or from the command line using the .NET CLI:

`dotnet add package Meadow.GnssTracker`

## Usage

```csharp
public class MeadowApp : App<F7CoreComputeV2>
{
    IGnssTrackerHardware gnssTracker;

    public override Task Initialize()
    {
        gnssTracker = GnssTracker.Create();

        if (gnssTracker.TemperatureSensor is { } temperatureSensor)
        {
            temperatureSensor.Updated += (s, e) =>
                Resolver.Log.Info($"Temperature: {e.New.Celsius:N1}C");
        }

        if (gnssTracker.Gnss is { } gnss)
        {
            gnss.RmcReceived += (s, e) =>
            {
                if (e.IsValid)
                    Resolver.Log.Info($"GNSS: LAT [{e.Position.Latitude}], LONG [{e.Position.Longitude}]");
            };
        }

        if (gnssTracker.Display is { } display)
        {
            var graphics = new MicroGraphics(display);
            graphics.DrawText(10, 10, "Hello, GNSS Tracker!");
            graphics.Show();
        }

        if (gnssTracker.OnboardRgbLed is { } led)
        {
            led.StartPulse(Color.Green);
        }

        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        if (gnssTracker.TemperatureSensor is { } temperatureSensor)
            temperatureSensor.StartUpdating(TimeSpan.FromMinutes(5));

        if (gnssTracker.Gnss is { } gnss)
            gnss.StartUpdating();

        await Task.Delay(Timeout.Infinite);
    }
}
```

## How to Contribute

- **Found a bug?** [Report an issue](https://github.com/WildernessLabs/Meadow_Issues/issues)
- Have a **feature idea or driver request?** [Open a new feature request](https://github.com/WildernessLabs/Meadow_Issues/issues)
- Want to **contribute code?** Fork the [GNSS_Sensor_Tracker](https://github.com/WildernessLabs/GNSS_Sensor_Tracker) repository and submit a pull request against the `develop` branch

## Need Help?

If you have questions or need assistance, please join the Wilderness Labs [community on Slack](http://slackinvite.wildernesslabs.co/).

## About Meadow

Meadow is a complete, IoT platform with defense-grade security that runs full .NET applications on embeddable microcontrollers and Linux single-board computers including Raspberry Pi and NVIDIA Jetson.

### Build

Use the full .NET platform and tooling such as Visual Studio and plug-and-play hardware drivers to painlessly build IoT solutions.

### Connect

Utilize native support for WiFi, Ethernet, and Cellular connectivity to send sensor data to the Cloud and remotely control your peripherals.

### Deploy

Instantly deploy and manage your fleet in the cloud for OtA, health-monitoring, logs, command + control, and enterprise backend integrations.
