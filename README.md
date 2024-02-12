<img src="Design/banner.jpg" style="margin-bottom:10px" />

# GNSS_Sensor_Tracker

The GNSS Sensor Tracker is an open-source, sensor-rich, GNSS/GPS tracking board intended to provide a base design accelerator for custom Meadow-based GNSS tracking solutions.

## Contents
* [Purchasing or Building](#purchasing-or-building)
* [Getting Started](#getting-started)
* [Hardware Specifications](#hardware-specifications)
* [Video Stream Design Series](#video-stream-design-series)
* [Industrial Enclosure Design](#industrial-enclosure-design)

## Purchasing or Building

<table width="100%">
    <tr>
        <td>
            <img src="Design/gnss-tracker-store.jpg" />
        </td>
        <td>
            <img src="Design/gnss-tracker-pcb.jpg" /> 
        </td>
    </tr>
    <tr>
        <td>
            You can get a Gnss Sensor Tracker fully assembled from the <a href="https://store.wildernesslabs.co/collections/frontpage/products/gnss-sensor-tracker">Wilderness Labs store</a>.
        </td>
        <td> 
            It's also designed so that it can be assembled at home for the adventurous. All design files can be found in the <a href="Hardware/Design%20Files/">Hardware Design</a> folder.
        </td>
    </tr>
</table>

## Getting Started

To simplify the way to use this Meadow-powered reference IoT product, we've created a NuGet package that instantiates and encapsulates the onboard hardware into a `GnssTracker` class.

1. Add the ProjectLab Nuget package your project: 
    - `dotnet add package Meadow.GnssTracker`, or
    - [Meadow.GnssTracker Nuget Package](https://www.nuget.org/packages/Meadow.GnssTracker)
    - [Explore in Fuget.org](https://www.fuget.org/packages/Meadow.GnssTracker/0.96.0/lib/netstandard2.1/GnssTracker.dll/WildernessLabs.Hardware.GnssTracker/GnssTracker?code=true)

2. Instantiate the `IGnssTrackerHardware` object:  
```csharp
public class MeadowApp : App<F7CoreComputeV2>
{
    IGnssTrackerHardware gnssTracker;

    public override Task Initialize()
    {
        gnssTracker = GnssTracker.Create();
        ...
```

3. To Access the `GNSS Tracker` onboard peripherals (Display, for example):
```csharp
    if (gnssTracker.Display is { } display)
    {
        microGraphics = new MicroGraphics(display);
        microGraphics.Clear();
        microGraphics.DrawText(10, 10, "Hello World");
        microGraphics.Show();
    }
```

## Hardware Specifications

<img src="Design/gnss-tracker-v2-specs.jpg" style="margin-top:10px;margin-bottom:10px" />

<table>
    <tr>
        <td><strong>3.7V LiPo Rechargeable Battery</strong></td>
        <td><strong>BMI270 Inertial Measurement Unit / Accelerometer</strong></td>
    </tr>
    <tr>
        <td><strong>SCD40 CO2, Humidity and Temperature sensor</strong></td>
        <td><strong>Off/On Switch</strong></td>
    </tr>
    <tr>
        <td><strong>SSD1680 122x250 Adafruit E-Paper Display</strong></td>
        <td><strong>USB-C, Boot, Reset Add-on Module</strong></td>
    </tr>
    <tr>
        <td><strong>Meadow F7 Core-Compute Module (CCM)</strong></td>
        <td><strong>Solar Power/Battery Charging Add-on Module</strong></td>
    </tr>
    <tr>
        <td><strong>6V Solar Input Connector</strong></td>
        <td><strong>BME688 Temp, Pressure and Humidity sensor</strong></td>
    </tr>
    <tr>
        <td><strong>GNSS/GPS Antenna</strong></td>
        <td><strong>NEO-M8 GNSS/GPS</strong></td>
    </tr>
</table>

You can find the schematics and other design files in the [Hardware folder](Hardware/Design%20Files/).

## Video Stream Design Series

This board was designed while streaming live, you can watch the videos [here](https://www.youtube.com/watch?v=L4MavM8ilkg&list=PLoP9Fu9zn7qY4rkFJjHBhnpI8mPlw8RfS).

<a href="https://www.youtube.com/watch?v=L4MavM8ilkg&list=PLoP9Fu9zn7qY4rkFJjHBhnpI8mPlw8RfS">
    <img src="Design/gnss-playlist.png" style="margin-top:10px;margin-bottom:10px" />
</a>

## Industrial Enclosure Design

The enclosure was designed in Autodesk Fusion 360. The source file can be found and STL files can be found [here](Hardware/Enclosure/).

![](Design/gnss-tracker-enclosure.png)